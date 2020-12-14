using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Proyecto_cliente
{
    public partial class Form3 : Form
    {
        Fllist Lista_fichas = new Fllist();
        PictureBox[] Fitxes = new PictureBox[80];
        public Form3()
        {
            InitializeComponent();
            
            int i = 0;
            Lista_fichas.Afegirfitxes();
            string identificador;
            while (i < Lista_fichas.Getnum())
            {

                Ficha a = Lista_fichas.Getfitxa(i);
                PictureBox Fitxa = new PictureBox();
                Fitxa.ClientSize = new Size(50, 50);
                Fitxa.Location = new Point(Convert.ToInt32(a.Getposx()), Convert.ToInt32(a.Getposy()));
                //Fitxa.Location = new Point(0, 0);
                // Adjust the image size to 20x20
                Fitxa.SizeMode = PictureBoxSizeMode.StretchImage;
                // The file should be in the debug folder

                identificador = a.Getid();
               
                if (identificador == "N")
                {
                    Bitmap image = new Bitmap("Nfitxa.jpeg");
                    Fitxa.Image = (Image)image;
                }
                if (identificador == "V")
                {
                    Bitmap image = new Bitmap("Vfitxa.jpeg");
                    Fitxa.Image = (Image)image;
                }
                panel1.Controls.Add(Fitxa);
                Fitxes[i] = Fitxa;
                Fitxa.Tag = i;
                //Fitxa.Click += new System.EventHandler(this.evento);
                i++;

            }

        }

        private void Form3_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
