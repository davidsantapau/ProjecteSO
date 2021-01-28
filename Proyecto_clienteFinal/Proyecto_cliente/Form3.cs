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
    public partial class Form3 : Form
    {
        //VARIABLES GLOBALES
        int p;
        int torn;
        string company;
        Form1 F2 = new Form1();
        Socket server;
        int nposx;
        int nposy;
        Fllist Lista_fichas = new Fllist();
        int r;// id
        string fColor;//color de la ficha
        int px;
        int py;
        int codi;
        Thread actualitzar;
        PictureBox[] Fitxes = new PictureBox[80];
        delegate void DelegadoParaPonerTexto(string texto);
        string col;//color
        int k = 0;
        public Form3(string company, Socket server, int torn, string col)//Inicializamos Form3 y las variables server,col,company y torn
        {
            InitializeComponent();
            this.server = server;
            this.company = company;
            this.torn = torn;
            int i = 0;
            this.col = col;
            Lista_fichas.Afegirfitxes();
            string identificador;
            while (i < Lista_fichas.Getnum())//cargamos la lista de fichas
            {

                Ficha a = Lista_fichas.Getfitxa(i);
                PictureBox Fitxa = new PictureBox();
                Fitxa.ClientSize = new Size(50, 50);
                Fitxa.Location = new Point(Convert.ToInt32(a.Getpos() * 50), Convert.ToInt32(a.Getposy() * 50));
                Fitxa.SizeMode = PictureBoxSizeMode.StretchImage;
                identificador = a.GetColor();

                if (identificador == "N")
                {
                    Bitmap image = new Bitmap("Nfitxa.jpeg");//ficha negra
                    Fitxa.Image = (Image)image;
                }
                if (identificador == "V")
                {
                    Bitmap image = new Bitmap("Vfitxa.jpeg");//ficha roja
                    Fitxa.Image = (Image)image;
                }
                panel1.Controls.Add(Fitxa);
                Fitxes[i] = Fitxa;
                Fitxa.Tag = i;
                Fitxa.Click += new System.EventHandler(this.evento);

                i++;

            }




        }
        public void thread()//Ponemos en marcha el thread de actualizaciones
        {
            ThreadStart ts = delegate { Actualitzacions(); };
            actualitzar = new Thread(ts);
            actualitzar.Start();
        }
        private void Actualitzacions()//actualiza la posicion de la ficha movida por el rival
        {
            while (true)
            {
                DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Notificacion);
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                int codigo = Convert.ToInt32(trozos[0]);

                switch (codigo)
                {
                    case 9:// "9/idficha/nueva posicion X/ nueva posicion Y"

                        r = Convert.ToInt32(trozos[1]);
                        nposx = Convert.ToInt32(trozos[2]);
                        nposy = Convert.ToInt32(trozos[3]);
                        p = Lista_fichas.Getp(r, nposx, nposy,k);

                        fColor = Lista_fichas.Get_Col(r);
                        PictureBox Fitxa = Fitxes[r - 1];
                        Fitxa.Location = new Point((nposx * 50), (nposy * 50));
                        Fitxa.SizeMode = PictureBoxSizeMode.StretchImage;
                        if (fColor == "N")
                        {
                            Bitmap image = new Bitmap("Nfitxa.jpeg");
                            Fitxa.Image = (Image)image;
                        }
                        else
                        {
                            Bitmap image = new Bitmap("Vfitxa.jpeg");
                            Fitxa.Image = (Image)image;
                        }
                        panel1.Controls.Add(Fitxa);

                        if (p != 0)
                        {

                            Fitxa.Click += new System.EventHandler(this.evento);
                            PictureBox fitxo = Fitxes[p];
                            panel1.Controls.Remove(fitxo);
                            Lista_fichas.eliminar_fitxa(p);
                        }
                        
                            torn = 1;
                            this.Invoke(del, new Object[] { "Tu turno" });
                            break;
                        
                }
            }
        }


        private void evento(object sender, EventArgs e)//recogemos la informacion de la ficha seleccionada
        {
            PictureBox p = (PictureBox)sender;
            int tag = (int)p.Tag;
            r = Lista_fichas.Getfitxa(tag).Getid();
            fColor = Lista_fichas.Getfitxa(tag).GetColor();
            px = Lista_fichas.Getfitxa(tag).Getpos();
            py = Lista_fichas.Getfitxa(tag).Getposy();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)//Boton de rendirse que cierra el juego y el thread de actualizaciones
        {
            actualitzar.Abort();
            Close();
        }


        public void Notificacion(string respuesta)//Imprime en pantalla informacion del juego como el turno o movimientos erroneos
        {
            this.torn_lbl.Text = respuesta;
        }
        private void panel1_Click(object sender, MouseEventArgs e)
        {


            string mensaje;
            if (torn == 1)//turno del cliente
            {
                    fColor = Lista_fichas.Get_Col(r);
                    if (col == fColor)//color asignado es igual al que selecciona el mismo usuario
                    {
                        nposx = e.Location.X / 50;
                        nposy = e.Location.Y / 50;
                        p = Lista_fichas.Getp(r,nposx, nposy,k);//tipo de movimiento
                        if (p == -1)//movimiento prohibido
                        {
                            DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Notificacion);
                            this.Invoke(del, new Object[] { "Moviment no permés" });
                        }
                        else 
                        {
                            PictureBox Fitxa = Fitxes[r-1];//correccion del identificador porque empieza en 1
                            Fitxa.Location = new Point((nposx * 50), (nposy * 50));
                            Fitxa.SizeMode = PictureBoxSizeMode.StretchImage;
                            if (fColor == "N")
                            {
                                Bitmap image = new Bitmap("Nfitxa.jpeg");
                                Fitxa.Image = (Image)image;
                            }
                            else
                            {
                                Bitmap image = new Bitmap("Vfitxa.jpeg");
                                Fitxa.Image = (Image)image;
                            }
                            panel1.Controls.Add(Fitxa);


                            Fitxa.Click += new System.EventHandler(this.evento);
                            
                            if (p == 0)//la ficha se mueve una posicion sin mas
                            {
                                mensaje = "9/" + company + "/" + r + "/" + nposx + "/" + nposy + "/" + p;
                                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                                server.Send(msg);
                            }
                            else//la ficha se ha comido una ficha del rival
                            {
                                
                                k = Lista_fichas.csegonmoviment(nposx, nposy);//funcion en curso
                                mensaje = "9/" + company + "/" + r + "/" + nposx + "/" + nposy;
                                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                                server.Send(msg);
                                //Eliminar ficha en el tablero
                                PictureBox Fitxo = Fitxes[p];
                                Lista_fichas.eliminar_fitxa(p);
                                panel1.Controls.Remove(Fitxo);
                                //busca ganadores
                                if (col == "N")
                                {
                                    int g = Lista_fichas.guanyador("V");
                                    
                                    if (g == 0)
                                    {
                                        DelegadoParaPonerTexto dele = new DelegadoParaPonerTexto(Notificacion);
                                        this.Invoke(dele, new Object[] { "Ha guanyat" });
                                    }
                                }
                                else
                                {
                                    int g = Lista_fichas.guanyador("N");
                                    if (g == 0)
                                    {
                                        DelegadoParaPonerTexto dele = new DelegadoParaPonerTexto(Notificacion);
                                        this.Invoke(dele, new Object[] { "Ha guanyat" });
                                    }
                                }
                                
                            }
                                torn = 0;
                                DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Notificacion);
                                this.Invoke(del, new Object[] { "Torn de: " + company });
                          
                        }
                    }

                    else
                    {
                        DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Notificacion);
                        this.Invoke(del, new Object[] { "Esta ficha no te pertece" });
                    }
                
            }

            else
            {
                DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Notificacion);
                this.Invoke(del, new Object[] { "Espera tu turno" });
            }
        }


       
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }


    }
}
