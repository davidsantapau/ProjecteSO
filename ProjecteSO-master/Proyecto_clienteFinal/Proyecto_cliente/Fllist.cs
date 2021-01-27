using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Proyecto_cliente
{
    class Fllist //Lista de fichas con sus funciones
    {
        int num = 0;
        List<Ficha> Lfichas = new List<Ficha>();
        public int Getnum()
        {
            return Lfichas.Count;

        }
        public int Getp(int id, int px, int py, int k)//Funcion movimiento
            // "-1" para movimiento incorrecto, "0" si es simple y "id" si el movimiento es doble
        {
            Ficha a = new Ficha();
            Ficha b = new Ficha();
            a = Lfichas[id-1];
            int i = 0;
            bool trobat = false;
            if (k == 0)//funcion en proceso para matar dos fichas seguidas
            {
                if (a.GetColor() == "N")//si la ficha es negra
                {
                    if (a.Getdama() == 0)//si es DAMA (si lo es puede moverse hacia atras)
                    {
                        if ((py == a.Getposy() + 1) && ((px == a.Getpos() + 1) || (px == a.Getpos() - 1)))//movimiento simple
                        //si llega al final de l tablero, la ficha se convierte en DAMA
                        {
                            a.Setposx(px);
                            a.Setposy(py);
                            if (a.Getposy() == 7)
                                a.Setdama(1);
                            return 0;
                        }
                        else//movimiento con muerte
                            //si llega al final de l tablero, la ficha se convierte en DAMA
                        {
                            if ((py == a.Getposy() + 2) && ((px == a.Getpos() - 2) || (px == a.Getpos() + 2)))
                            {

                                while ((i < Lfichas.Count) && (!trobat))//buscamos si hay ficha rival entre la posicion deonde estamos y la posicion destino
                                {
                                    b = Lfichas[i];
                                    if ((b.Getpos() == px - 1) && (b.Getposy() == py - 1) && (b.GetColor() == "V") && (px == a.Getpos() + 2))
                                    {
                                        trobat = true;
                                    }
                                    else if ((b.Getpos() == px + 1) && (b.Getposy() == py - 1) && (b.GetColor() == "V") && (px == a.Getpos() - 2))
                                    {
                                        trobat = true;
                                    }
                                    else
                                        i++;
                                }
                                if (trobat)//retornamos la posicion de la ficha que matamos
                                {

                                    a.Setposx(px);
                                    a.Setposy(py);
                                    if (a.Getposy() == 7)
                                        a.Setdama(1);
                                    return i;
                                }

                                else
                                    return -1;
                            }
                            else
                                return -1;
                        }
                    }
                    else//si la ficha es DAMA
                    {
                        if (((py == a.Getposy() + 1) && ((px == a.Getpos() + 1) || (px == a.Getpos() - 1))) || ((py == a.Getposy() - 1) && ((px == a.Getpos() + 1) || (px == a.Getpos() - 1))))
                        {
                            a.Setposx(px);
                            a.Setposy(py);
                            if (a.Getposy() == 7)
                                a.Setdama(1);
                            return 0;
                        }
                        else
                        {
                            if (((py == a.Getposy() + 2) && ((px == a.Getpos() - 2) || (px == a.Getpos() + 2))) || (((py == a.Getposy() - 2) && ((px == a.Getpos() - 2) || (px == a.Getpos() + 2)))))
                            {
                                while ((i < Lfichas.Count) && (!trobat))
                                {
                                    if ((((b.Getpos() == px - 1) && (b.Getposy() == py - 1) && (b.GetColor() == "V") && (px == a.Getpos() + 2)) || (((b.Getpos() == px + 1) && (b.Getposy() == py - 1) && (b.GetColor() == "V") && (px == a.Getpos() - 2))) || ((b.Getpos() == px - 1) && (b.Getposy() == py + 1) && (b.GetColor() == "V") && (px == a.Getpos() + 2))) || (((b.Getpos() == px + 1) && (b.Getposy() == py + 1) && (b.GetColor() == "V") && (px == a.Getpos() - 2))))
                                    {
                                        trobat = true;
                                    }
                                    i++;
                                }
                                if (trobat)
                                    return i;
                                if (a.Getposy() == 7)
                                    a.Setdama(1);
                                else
                                    return -1;
                            }
                            else
                            {
                                return -1;
                            }
                        }
                    }

                }


                //mismas condiciones (con matices distintos) si la ficha es roja

                if (a.GetColor() == "V")
                {
                    if (a.Getdama() == 0)
                    {
                        if ((py == a.Getposy() - 1) && ((px == a.Getpos() + 1) || (px == a.Getpos() - 1)))
                        {
                            a.Setposx(px);
                            a.Setposy(py);
                            if (a.Getposy() == 0)
                                a.Setdama(1);
                            return 0;
                        }
                        else
                        {
                            if ((py == a.Getposy() - 2) && ((px == a.Getpos() - 2) || (px == a.Getpos() + 2)))
                            {

                                while ((i < Lfichas.Count) && (!trobat))
                                {
                                    b = Lfichas[i];
                                    if ((b.Getpos() == px - 1) && (b.Getposy() == py + 1) && (b.GetColor() == "N") && (px == a.Getpos() + 2))
                                    {
                                        trobat = true;
                                    }
                                    else if ((b.Getpos() == px + 1) && (b.Getposy() == py + 1) && (b.GetColor() == "N") && (px == a.Getpos() - 2))
                                    {
                                        trobat = true;
                                    }
                                    else
                                        i++;
                                }
                                if (trobat)
                                {

                                    a.Setposx(px);
                                    a.Setposy(py);
                                    if (a.Getposy() == 0)
                                        a.Setdama(1);
                                    return i;
                                }
                                else
                                    return -1;
                            }
                            else
                                return -1;
                        }


                    }
                    else
                    {
                        if (((py == a.Getposy() + 1) && ((px == a.Getpos() + 1) || (px == a.Getpos() - 1))) || ((py == a.Getposy() - 1) && ((px == a.Getpos() + 1) || (px == a.Getpos() - 1))))
                        {
                            a.Setposx(px);
                            a.Setposy(py);
                            if (a.Getposy() == 0)
                                a.Setdama(1);
                            return 0;
                        }
                        if (((py == a.Getposy() + 2) && ((px == a.Getpos() - 2) || (px == a.Getpos() + 2))) || (((py == a.Getposy() - 2) && ((px == a.Getpos() - 2) || (px == a.Getpos() + 2)))))
                        {
                            while ((i < Lfichas.Count) && (!trobat))
                            {
                                if ((((b.Getpos() == px - 1) && (b.Getposy() == py - 1) && (b.GetColor() == "N") && (px == a.Getpos() + 2)) || (((b.Getpos() == px + 1) && (b.Getposy() == py - 1) && (b.GetColor() == "N") && (px == a.Getpos() - 2))) || ((b.Getpos() == px - 1) && (b.Getposy() == py + 1) && (b.GetColor() == "N") && (px == a.Getpos() + 2))) || (((b.Getpos() == px + 1) && (b.Getposy() == py + 1) && (b.GetColor() == "N") && (px == a.Getpos() - 2))))
                                {
                                    trobat = true;
                                }
                                i++;
                            }
                            if (trobat)
                            {
                                a.Setposx(px);
                                a.Setposy(py);
                                if (a.Getposy() == 0)
                                    a.Setdama(1);
                                return i;
                            }
                            else
                                return -1;
                        }
                        else
                            return -1;
                    }

                }
                else
                    return -1;
            }
            else // k == 1, funcion en curso
            {
                if (a.GetColor() == "N")
                {
                    if ((py == a.Getposy() + 2) && ((px == a.Getpos() - 2) || (px == a.Getpos() + 2)))
                    {

                        while ((i < Lfichas.Count) && (!trobat))
                        {
                            b = Lfichas[i];
                            if ((b.Getpos() == px - 1) && (b.Getposy() == py - 1) && (b.GetColor() == "V") && (px == a.Getpos() + 2))
                            {
                                trobat = true;
                            }
                            else if ((b.Getpos() == px + 1) && (b.Getposy() == py - 1) && (b.GetColor() == "V") && (px == a.Getpos() - 2))
                            {
                                trobat = true;
                            }
                            else
                                i++;
                        }
                        if (trobat)
                        {

                            a.Setposx(px);
                            a.Setposy(py);
                            if (a.Getposy() == 7)
                                a.Setdama(1);
                            return i;
                        }

                        else
                            return -1;
                    }
                    else
                        return -1;
                }
                else
                {
                    if ((py == a.Getposy() - 2) && ((px == a.Getpos() - 2) || (px == a.Getpos() + 2)))
                    {

                        while ((i < Lfichas.Count) && (!trobat))
                        {
                            b = Lfichas[i];
                            if ((b.Getpos() == px - 1) && (b.Getposy() == py - 1) && (b.GetColor() == "N") && (px == a.Getpos() + 2))
                            {
                                trobat = true;
                            }
                            else if ((b.Getpos() == px + 1) && (b.Getposy() == py - 1) && (b.GetColor() == "N") && (px == a.Getpos() - 2))
                            {
                                trobat = true;
                            }
                            else
                                i++;
                        }
                        if (trobat)
                        {

                            a.Setposx(px);
                            a.Setposy(py);
                            if (a.Getposy() == 0)
                                a.Setdama(1);
                            return i;
                        }

                        else
                            return -1;
                    }
                    else
                        return -1;
                }
                }


             
                
            
        }
         public int guanyador(string col)//devuelve el contador de fichas vivas de cada color
        {
            Ficha a =new Ficha();
            int i=0;
            int cont=0;
            while (i < Lfichas.Count)
            {
                a = Lfichas[i];
                if ((col == "N") && (a.GetColor() == "N"))

                    cont++;
                else if ((col == "V") && (a.GetColor() == "V"))
                    cont++;
                
                    i++;
            }
             return cont;
        }
         public void eliminar_fitxa(int id)//cambia el color a "muerto" para no contarla
         {
             Ficha a = new Ficha();
             a= Lfichas[id];
             a.Setcolor("Morta");

         }
        public int csegonmoviment(int px, int py)//Contar si se puede hacer un 2do movimiento (funcion en curso))
        {
            int i = 0;
            bool trobat =false;
            bool trobat2=false;
            int camí=0;
            Ficha b= new Ficha();
            while ((i < Lfichas.Count) && (!trobat))
            {
                b = Lfichas[i];
                if ((px == b.Getpos() + 1) && (py == b.Getposy() + 1))
                {
                    trobat = true;
                    camí = 0;
                }
                else if ((px == b.Getpos() - 1) && (py == b.Getposy() + 1))
                {
                    trobat = true;
                    camí = 1;
                }
                else
                    i++;
            }
            if (trobat == true)
            {
                while ((i < Lfichas.Count) && (!trobat))
                {
                    b = Lfichas[i];
                    if ((px == b.Getpos() + 2) && (py == b.Getposy() + 2)&&(camí==0))
                        trobat = true;
                    else if ((px == b.Getpos() - 2) && (py == b.Getposy() + 2)&&(camí==1))
                    {
                        trobat = true;
                    }
                    else
                        i++;
                }
            }
            if ((trobat == true) && (trobat2 == false))

                return 1;
            else
                return 0;
        
        }
        public string Get_Col(int id)//de que color es la fichas seleccionada
        {
            string col;
            int i = 0;
            bool trobat = false;
            Ficha a = new Ficha();
            while ((i < Lfichas.Count) && (!trobat))
            {


                a = Lfichas[i];
                if ((a.Getid() == id))
                {
                    trobat = true;
                    return a.GetColor();
                }

                i++;
            }
            return "col";
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
        {//Carga del fichero que esta en BIN/DEBUG las fichas

           
            StreamReader A = new StreamReader("Fitxa.txt");

            int i = 0;
            string s;
            num = 0;
            s = A.ReadLine();


            while (s != null)
            {
                Ficha f = new Ficha();

                string[] trozos = s.Split(',');

                f.Setid(Convert.ToInt32(trozos[0]));
                f.Setcolor(trozos[1]);
                f.Setposx(Convert.ToInt32(trozos[3]));
                f.Setposy(Convert.ToInt32(trozos[2]));
                f.Setdama(0);
                

                Lfichas.Add(f);
                num++;
                s= A.ReadLine();
                i++;
            }
            A.Close();

        }
    }
}

