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
        int invitacio = 0;//para saber si tenemos invitacion o no
        Socket server;
        Thread atender;
        delegate void DelegadoParaPonerTexto(string texto);
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public void Invitacio(string respuesta)
        {
            this.label4.Text = respuesta;//Notificacion de invitacion
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
                    case 1:// REGISTRAR
                        if (Convert.ToInt32(trozos[1]) == 0)
                        {
                            MessageBox.Show("Registrado correctamente");
                        }
                        else
                        {
                            MessageBox.Show("Error al registrar");
                        }
                        TUsername.Clear();
                        Password.Clear();
                        break;
                    case 2://LOGIN
                        if (Convert.ToInt32(trozos[1]) == 0)
                        {
                            MessageBox.Show("Login correcto");
                        }
                        else
                            MessageBox.Show("Persona no registrada");
                        //TUsername.Clear();
                        //Password.Clear();
                        break;
                    case 3://JUGADOR CON MAS PARTIDAS
                        if(Convert.ToInt32(trozos[1])== 0)
                        {
                            MessageBox.Show("El ganador es:" + trozos[2]);
                        }
                        else
                        {
                            MessageBox.Show("Error al consultar");
                        }
                        break;
                    case 4://PARTIDA MAS LARGA,SU DURACION
                        if(Convert.ToInt32(trozos[1])== -1)
                        {
                            MessageBox.Show("La duración de la partida es:" + Convert.ToInt32(trozos[2]));
                        }
                        else
                        {
                            MessageBox.Show("Error al consultar");
                        }
                        break;
                    case 5://PARTIDAS GANADAS DE UN JUGADOR
                        if(Convert.ToInt32(trozos[1])== -1)
                        {
                            MessageBox.Show("El jugador ha ganado:" + Convert.ToInt32(trozos[2])+ "partidas");
                        }
                        else
                        {
                            MessageBox.Show("Error al consultar");
                        }
                        break;

                    case 6://MOSTRAR LISTA CONECTADOS
                        if (Convert.ToInt32(trozos[1]) == 0)
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

                        }
                        else
                        {
                            MessageBox.Show("Error al consultar");
                        }
                        break;
                    case 7://INVITAR          ***********************  N U E V O Version4
                        if(Convert.ToInt32(trozos[1])==0)
                        {
                            MessageBox.Show("Invitación aceptada");//ACEPTADA
                            Form3 F3 = new Form3();
                            F3.ShowDialog();
                            
                        }
                        else if (Convert.ToInt32(trozos[1]) == 1)//RECHAZADO
                        {
                            //MessageBox.Show("Invitación rechazada");
                            DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                            this.Invoke(del, new Object[] { "Te han rechazado" });
                        }
                        else if (Convert.ToInt32(trozos[1]) == 2)//NOTIFICACION DE INVITACION
                        {
                            //MessageBox.Show("Te han invitado");
                            DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                            this.Invoke (del, new Object[]{ "Te han invitado a jugar"});
                            invitacio = 1;
                        
                        }
                        else if (Convert.ToInt32(trozos[1])==3)
                        {
                            MessageBox.Show("Jugador no encontrado");
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
            IPAddress direc = IPAddress.Parse("192.168.56.103");
            IPEndPoint ipep = new IPEndPoint(direc, 9770);


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
            atender.Abort();
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
        }

       

        private void Registro_Click(object sender, EventArgs e)
        {
            // Envia el nombre y la constraseña del registro con el código 1 y separado por /
            string mensaje = "1/" + Convert.ToString(TUsername.Text) + "/" + Convert.ToString(Password.Text);
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        private void button3_Click(object sender, EventArgs e)//Invitar
        {
            string mensaje = "7/" + Convert.ToString(textBox2.Text);
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

        }

        /*******Consultas en el menu de opcione********************************************************************************/
        private void consula1ToolStripMenuItem_Click(object sender, EventArgs e)//Jugador amb més victories
        {
            // Quiere saber la longitud
            string mensaje = "3/";
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje); // cojo el string y lo convierto a un vect de bytes
            server.Send(msg);
        }

        private void consulta2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Quiere saber la longitud
            string mensaje = "4/";
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje); // cojo el string y lo convierto a un vect de bytes
            server.Send(msg);
        }

        private void consulta3ToolStripMenuItem_Click(object sender, EventArgs e)//   Consulta 3
        {
            string mensaje = "5/" + textBox1.Text;
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Escribe el nombre del jugador en la casilla correspondiente");
            }
            else
            {
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
        }


        /***************************************Aceptar o rechazar invitaciones*********************************************************************/
        private void button2_Click(object sender, EventArgs e)//enviamos respuesta positiva a una invitacion
        {
            if (invitacio==0)
                MessageBox.Show("No tienes ninguna invitación para jugar aún");
            else
            {
                string mensaje = "8/0";
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                Form3 F3 = new Form3();//abrimos el form
                F3.ShowDialog();
                invitacio = 0;
            }
        }

        private void button4_Click(object sender, EventArgs e)//boton de rechazar invitacion
        {
            if (invitacio == 0)
                MessageBox.Show("No tienes ninguna invitación para jugar aún");
            else
            {
                string mensaje = "8/1";
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                invitacio = 0;
            }
        }

       
        

        //label4


    }
}
