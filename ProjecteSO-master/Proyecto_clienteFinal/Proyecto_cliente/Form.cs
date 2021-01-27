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
    public partial class Form2 : Form
    {
        //Form3 F3 = new Form3();
        Form1 F1 = new Form1();
        public Form2()
        {
            InitializeComponent();
        }
        public Form1 Getf1()
        {
            return F1;
        }

        //Funció Login. Obre el menu del joc si la persona està registrada retorna error si no ho està
        public void button1_Click(object sender, EventArgs e)
        {
            
            string tusername = textBox1.Text;
            string tpassword = textBox2.Text;
            if ((string.IsNullOrEmpty(textBox1.Text) || (string.IsNullOrEmpty(textBox2.Text))))
            {
                MessageBox.Show("Asegurese de rellenar todas las casillas");
            }
            else
            {
                int er = F1.Connectar();
                if (er == 0)
                {
                    int c;
                    c = F1.Login(tusername, tpassword);
                    if (c == 1)
                    {
                        //F1.Benvinguda();
                        this.Hide();
                        F1.ShowDialog();
                        this.Show();

                    }
                    else
                        MessageBox.Show("Usuario o contraseña incorrecta");
                }
                else
                    MessageBox.Show("No se ha podido establecer conexión");
            }
        }

        //Funció de registre
        private void button2_Click(object sender, EventArgs e)
        {
            Form1 F1 = new Form1();
            string tusername = textBox1.Text;
            string tpassword = textBox2.Text;
            if ((string.IsNullOrEmpty(textBox1.Text)||(string.IsNullOrEmpty(textBox2.Text))))
            {
                MessageBox.Show("Asegurese de rellenar todas las casillas");
            }
            else
            {
                F1.Connectar();
                int c;
                c = F1.Registrarse(tusername, tpassword);
                if (c == 1)
                {
                    MessageBox.Show("Registro completado");
                }
                else
                    MessageBox.Show("Usuario existente. Elija otro");
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

       
    }
}
