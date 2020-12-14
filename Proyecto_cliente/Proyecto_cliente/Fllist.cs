using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Proyecto_cliente
{
    class Fllist
    {
        int num = 0;
        List<Ficha> Lfichas = new List<Ficha>();
        public int Getnum()
        {
            return Lfichas.Count;

        }
        public Ficha Getfitxa(int i)
        {
            if (i < 0 || i >= Lfichas.Count)
                return null;
            Ficha a = new Ficha();
            a = Lfichas[i];
            return a;
        }
        public void Afegirfitxes()
        {

           
            StreamReader A = new StreamReader("Fitxa.txt");

            int i = 0;
            string s;
            num = 0;
            s = A.ReadLine();


            while (s != null)
            {
                Ficha f = new Ficha();

                string[] trozos = s.Split(',');

                f.Setid(trozos[0]);
                f.Setposx(Convert.ToInt32(trozos[1]));
                f.Setposy(Convert.ToInt32(trozos[2]));
                Lfichas.Add(f);
                num++;
                s= A.ReadLine();
                i++;
            }
            A.Close();

        }
    }
}

