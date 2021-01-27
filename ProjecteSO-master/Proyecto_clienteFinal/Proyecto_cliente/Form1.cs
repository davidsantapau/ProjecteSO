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
        //VARIABLES GLOBALES
        int invitacio = 0;
        Socket server;
        Thread atender;
        delegate void DelegadoParaPonerTexto(string texto);
        string company;
        int acceptat = 0;

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public void Invitacio(string respuesta)//Estado de la invitacion
        {
            this.label4.Text = respuesta;
        }

        public void Consultas(string resultado)
        {
            this.label2.Text = resultado;
        }
        public void Benvinguda(string missatge)
        {
            this.label1.Text = missatge;
        }
        private void Atenderservidor()//*************************************************************************ATENDER SERVIDOR********************//
        {
            while (true)
            {
                DelegadoParaPonerTexto delegao = new DelegadoParaPonerTexto(Consultas);
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');//troceamos mensaje del servidor
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
                        
                        break;
                    case 2://LOGIN
                        if (Convert.ToInt32(trozos[1]) == 0)
                        {
                            MessageBox.Show("Login correcto");
                        }
                        else
                            MessageBox.Show("Persona no registrada");
                       
                        break;
                    case 3://JUGADOR CON MAS PARTIDAS
                        if(Convert.ToInt32(trozos[1])== 0)
                        {
                            this.Invoke(delegao, new Object[] { "El ganador es:" + trozos[2] }); 
                          
                        }
                        else
                        {
                            this.Invoke(delegao, new Object[] { "Error al consultar" });
                           
                        }
                        break;
                    case 4://PARTIDA MAS LARGA,SU DURACION
                        if(Convert.ToInt32(trozos[1])== -1)
                        {
                            this.Invoke(delegao, new Object[] { "La duración de la partida es:" + Convert.ToInt32(trozos[2]) });
                            
                        }
                        else
                        {
                            this.Invoke(delegao, new Object[] { "Error al consultar" });
                            
                        }
                        break;
                    case 5://PARTIDAS GANADAS DE UN JUGADOR
                        if(Convert.ToInt32(trozos[1])== -1)
                        {
                            this.Invoke(delegao, new Object[] { "El jugador ha ganado:" + Convert.ToInt32(trozos[2]) + "partidas" });
                            
                        }
                        else
                        {
                            this.Invoke(delegao, new Object[] { "Error al consultar" });
                            
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
                                dataGridView1.Rows.Add(trozos[i + 3]);
                                i++;
                            }

                        }
                        else
                        {
                            this.Invoke(delegao, new Object[] { "Error al consultar" });
                        }
                        break;
                    case 7://INVITAR          
                        if (Convert.ToInt32(trozos[1]) == 0)//han aceptado la invitacion
                        {
                            DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                            this.Invoke(del, new Object[] { "Te han aceptado" }); 
                            acceptat = 1;
                            
                        }
                        else if (Convert.ToInt32(trozos[1]) == 1)// HAS SIDO RECHAZADO
                        {
                            
                            DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                            this.Invoke(del, new Object[] { "Te han rechazado" });
                        }
                        else if (Convert.ToInt32(trozos[1]) == 2)//NOTIFICACION DE INVITACION
                        {
                            string label = trozos[3];//mostramos "usuarioX: te ha invitado"
                            
                            DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                            this.Invoke (del, new Object[]{label});
                            invitacio = 1;
                            company = trozos[2];//guardamos el nombre del usuario que manda la invitacion
                        
                        }
                        else if (Convert.ToInt32(trozos[1])==3)
                        {
                            DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                            this.Invoke(del, new Object[] { "Jugador no encontrado" });
                        }
                        break;
                    case 8:
                        
                            //Mensaje de desconexión
                            string mensaje = "0/";
                            dataGridView1.Rows.Clear();
                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                            server.Send(msg);
                            Close();
                            break;
                   

                    case 10:

                            //"10/Numero mensajes/Nombre de quien lo envia/mensaje"
                            int a = Convert.ToInt32(trozos[1]);//Numero de mensaje
                            int num = 0;
                            dataGridView2.Rows.Clear();

                            while (num < a)
                            {
                               
                                dataGridView2.Rows.Add(trozos[num + 2]);
                                num=num +1;
                            }
                            break;

                       
                        

                }
            }
        }
        string rusername, rpassword;

        public void SetUsername(string usuario)
        {
            rusername = usuario;
        }
        public void SetPassword(string contraseña)
        {
            rpassword = contraseña;
        }

        public int Connectar()
        {
            {
                //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
                //al que deseamos conectarnos
                IPAddress direc = IPAddress.Parse("147.83.117.22");
                IPEndPoint ipep = new IPEndPoint(direc, 50066);


                //Creamos el socket 
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    server.Connect(ipep);//Intentamos conectar el socket
                    this.BackColor = Color.Pink;
                    

                }
                catch (SocketException)
                {
                    //Si hay excepcion imprimimos error y salimos del programa con return 
                   
                    return 1;
                }
                ThreadStart ts = delegate { Atenderservidor(); };
                atender = new Thread(ts);
                atender.Start();
                return 0;

            }
        }
      


        public void Desconectar_Click(object sender, EventArgs e)
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
            this.Owner.Show();
            
        }


        public int Login(string tusername, string tpassword)
        {
            string mensaje = "2/" + tusername + "/" + tpassword;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);


            byte[] msg2 = new byte[80];
            server.Receive(msg2);
            string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
            if (Convert.ToInt32(trozos[1]) == 0)
            {
                SetUsername(tusername);
                return 1;
            }
            else
                return 0;
        }



        public int Registrarse(string tusername, string tpassword)//Llamado dede Form.cs : "1" si se ha registrado correctament, "0" si no.
        {
            // Envia el nombre y la constraseña del registro con el código 1 y separado por /
            string mensaje = "1/" + tusername + "/" + tpassword;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            byte[] msg2 = new byte[80];
            server.Receive(msg2);
            string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
            if (Convert.ToInt32(trozos[1]) == 0)
            {
                return 1;
            }
            else
                return 0;
        }
            

        /*******Consultas en el menu de opciones********************************************************************************/
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

        private void consulta3ToolStripMenuItem_Click(object sender, EventArgs e)//Nº victorias de un jugador introducido por teclado
        {
            string mensaje = "5/" + textBox1.Text;
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                DelegadoParaPonerTexto delegao = new DelegadoParaPonerTexto(Consultas);
                this.Invoke(delegao, new Object[] { "Escribe el nombre del jugador en la casilla correspondiente" });
            }
            else
            {
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
        }


        /***************************************Aceptar o rechazar invitaciones*********************************************************************/
        private void button2_Click(object sender, EventArgs e)//si invitacion = 1, enviamos respuesta positiva y luego ponemos invitacion = 0.
        {
            if (invitacio==0)
            {
                DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                this.Invoke(del, new Object[] { "No tienes ninguna invitacion para jugar aún" });
            }
            else//enviamos respuesta positiva a una invitacion
            {
                string mensaje = "8/0";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                Form3 F3 = new Form3(company,server,1,"N");//abrimos el form
                F3.thread();
                this.Hide();
                F3.ShowDialog();
                this.Show();
                invitacio = 0;
            }
        }

        private void button4_Click(object sender, EventArgs e)//si invitacion = 1, enviamos respuesta negativa y luego ponemos invitacion = 0.
        {
            if (invitacio == 0)
            {
                DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                this.Invoke(del, new Object[] { "No tienes ninguna invitacion aún" });
            }
            else
            {
                string mensaje = "8/1";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                invitacio = 0;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)//Enviamos invitacion seleccionando usuario del datagrid
        {
            string usergrid = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            usergrid = usergrid.TrimEnd('\0');
            company = usergrid;

            if (rusername == usergrid)//se puede poner desde el servidor
            {
                DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                this.Invoke(del, new Object[] { "No te puedes invitr a ti mismo" });
            }
            else
            {
                string mensaje = "7/" + usergrid+"/"+ rusername;//construimos el mensaje para el servidor
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);//lo preparamos
                server.Send(msg);//enviamos el mensaje
            }
        }

      
       
        private void Desconectar_Click_1(object sender, EventArgs e)
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
            Close();
        }
      
       
        


        private void Xat_Click(object sender, EventArgs e)//"10/nom del usuario/mensaje"
        {
            string mensaje = "10/"+rusername+"/" + textBox2.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

        }

        private void button3_Click(object sender, EventArgs e)// Entrar en la partida siendo el que invita
        {
            if (acceptat == 0)//no nos aceptan la invitacion
            {
                DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                this.Invoke(del, new Object[] { "No han aceptado tu invitacion aún" });
            }
            else
            {
                Form3 F3 = new Form3(company, server, 0,"V");//abrimos el form
                F3.thread();
                this.Hide();
                acceptat = 0;
                F3.ShowDialog();
                this.Show();
            }
        }
        
    }
}
