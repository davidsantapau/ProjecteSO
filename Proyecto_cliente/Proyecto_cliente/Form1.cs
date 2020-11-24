using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Proyecto_cliente
{
    public partial class Form1 : Form
    {
        Socket server;
        Thread atender;
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void Atenderservidor()
        {
            while (true)
            {
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                int codigo = Convert.ToInt32(trozos[0]);

                switch (codigo)
                {
                    case 1:
                        if (Convert.ToInt32(trozos[1]) == 0)
                        {
                            MessageBox.Show("Registrat correctament");
                        }
                        else
                        {
                            MessageBox.Show("Error al registrar");
                        }
                        TUsername.Clear();
                        Password.Clear();
                        break;
                    case 2:
                        if (Convert.ToInt32(trozos[1]) == 0)
                        {
                            MessageBox.Show("Login correcto");
                        }
                        else
                            MessageBox.Show("Persona no registrado");
                        TUsername.Clear();
                        Password.Clear();
                        break;
                    case 3:
                        if(Convert.ToInt32(trozos[1])== 0)
                        {
                            MessageBox.Show("El ganador es:" + trozos[2]);
                        }
                        else
                        {
                            MessageBox.Show("Error al consultar");
                        }
                        break;
                    case 4:
                        if(Convert.ToInt32(trozos[1])== -1)
                        {
                            MessageBox.Show("La duración de la partida es:" + Convert.ToInt32(trozos[2]));
                        }
                        else
                        {
                            MessageBox.Show("Error al consultar");
                        }
                        break;
                    case 5:
                        if(Convert.ToInt32(trozos[1])== -1)
                        {
                            MessageBox.Show("El jugador ha ganado:" + Convert.ToInt32(trozos[2])+ "partidas");
                        }
                        else
                        {
                            MessageBox.Show("Error al consultar");
                        }
                        break;
                    case 6:
                        if(Convert.ToInt32(trozos[1])== 0)
                        {
                            int f = Convert.ToInt32(trozos[2]);
                            int i = 0;
                            dataGridView1.Rows.Clear();

                            while (i < f)
                            {
                                //MessageBox.Show(trozos[i+3]);
                                dataGridView1.Rows.Add(trozos[i + 3]);
                                i++;
                            }
                            //MessageBox.Show("soy LIBRE");
                            //int b = Convert.ToInt32(trozos[2]);
                            //int i = 0;
                            //if (dataGridView1.Rows.Count != 0)
                            //    dataGridView1.Rows.Clear();
                            //while(i<b)
                            //{
                            //    dataGridView1.Rows.Add();
                            //    dataGridView1.Rows[i].Cells[0].Value = trozos[i + 3];
                            //    i++;
                            //}
                        }
                        else
                        {
                            MessageBox.Show("Error al consultar");
                        }
                        break;

                }
            }
        }
        //string username;
        //string username2;
        //string password;
        string rusername, rpassword;
        //bool finalizado = false;
        //// int fila;
        //// string invitacion;
        //delegate void DelegadoParaEscribir(string text);
        //delegate void DelegadoParaActualizarLista(string[] nombres, int num);
        //delegate void DelegadoParaGroupBox();

        public void SetUsername(string usuario)
        {
            rusername = usuario;
        }
        public void SetPassword(string contraseña)
        {
            rpassword = contraseña;
        }

      
        private void Conectar_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.101");
            IPEndPoint ipep = new IPEndPoint(direc, 9050);


            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                this.BackColor = Color.Pink;
                MessageBox.Show("Conectado");

            }
            catch (SocketException)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }
            ThreadStart ts = delegate { Atenderservidor(); };
            atender = new Thread(ts);
            atender.Start();

        }

        private void Desconectar_Click(object sender, EventArgs e)
        {
            //Mensaje de desconexión
            string mensaje = "0/";
            dataGridView1.Rows.Clear();
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Nos desconectamos
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();
        }

        private void IniciarSesion_Click(object sender, EventArgs e)
        {
            //codigo 1 inciar sesion
            
           
                string mensaje = "2/" + TUsername.Text + "/" + Password.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                ////Recibimos la respuesta del servidor
                //byte[] msg2 = new byte[500];
                //server.Receive(msg2);
                //mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
               
                //int registrado = Convert.ToInt32(mensaje);
                //if (registrado == 1)
                //{
                //    MessageBox.Show("Usuario logeado correctamente");
                //}
                //else
                //{
                //    MessageBox.Show("Este usuario no esta registrado");
                //    finalizado = true;
                //}                    
        }

        private void Aceptar_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked) // CONSULTA 1. Jugador con mas victorias
            {
                // Quiere saber la longitud
                string mensaje = "3/";
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje); // cojo el string y lo convierto a un vect de bytes
                server.Send(msg);

                //Recibimos la respuesta del servidor
                //byte[] msg2 = new byte[80];
                //server.Receive(msg2); // deja la respuesta
                //mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0]; //convierto el vect en un string quiero q se quede con el 1r trozo
                //MessageBox.Show("El jugador con mas victorias es: " + mensaje);
            }
            else if (MasLarga.Checked) //CONSULTA 2. 
            {
                string mensaje = "4/";
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                ////Recibimos la respuesta del servidor
                //byte[] msg2 = new byte[80];
                //server.Receive(msg2);
                //mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                //MessageBox.Show("La partida más larga ha durado: " + mensaje);
            
            }
            else // CONSULTA 3. Partidas ganadas por el jugador introducido por teclado
            {
                string mensaje = "5/" + textBox1.Text ;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                ////Recibimos la respuesta del servidor
                //byte[] msg2 = new byte[80];
                //server.Receive(msg2);
                //mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                //MessageBox.Show(textBox1.Text + " ha ganado " + mensaje + "partidas");
                

            }
        }

        private void Registro_Click(object sender, EventArgs e)
        {
            // Envia el nombre y la constraseña del registro con el código 1 y separado por /
            string mensaje = "1/" + Convert.ToString(TUsername.Text) + "/" + Convert.ToString(Password.Text);
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

        //    byte[] msg2 = new byte[80];
        //    server.Receive(msg2);
        //    mensaje = Encoding.ASCII.GetString(msg2).Split('/')[0];
        //    if (Convert.ToInt32(mensaje) == 1)
        //    {
        //        MessageBox.Show("Registrado correctament");
        //    }
        //    else 
        //    { 
        //        MessageBox.Show("Error al registrarse"); 
        //    }
        //    TUsername.Clear();
        //    Password.Clear();
        //}

        //private void Conectados_Click(object sender, EventArgs e)
        //{
        //    string mensaje = "6/";
        //    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
        //    server.Send(msg);


        //    byte[] msg2 = new byte[80];
        //    server.Receive(msg2);
        //    mensaje = Encoding.ASCII.GetString(msg2);
        //    mensaje = mensaje.TrimEnd('\0');
        //    string[] trozos = mensaje.Split('/');
        //    if (Convert.ToInt32(trozos[0]) == 6)
        //    {
        //        int f = Convert.ToInt32(trozos[1]);
        //        dataGridView1.RowCount = f;
        //        int i = 0;
        //        while (i < f)
        //        {
        //            dataGridView1.Rows[i].Cells[0].Value = trozos[i + 2];
        //            i++;
        //        }
        //    }
        //    else 
        //    { 
        //        MessageBox.Show("peticion fallida"); 
        //    }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

       
        

        


    }
}
