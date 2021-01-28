using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proyecto_cliente
{
    class Ficha //Estructura ficha con sus constructores
    {
        int id;
        string color;
        int posx;
        int posy;
        int dama;
        public void Setid(int identificador)
        {
            this.id = identificador;        
        }
        public void Setcolor(string color)
        {
            this.color = color;
        }
        public void Setposx(int posx)
        {
            this.posx = posx;
        }
        public void Setposy(int posy)
        {
            this.posy = posy;
        }
        public void Setdama(int dama)
        {
            this.dama = dama;
        }
        public int Getid()
        {
            return this.id; 
        }
        public string GetColor()
        {
            return this.color;
        }
        public int Getpos()
        {
            return this.posx;
        }
        public int Getposy()
        {
            return this.posy;
        }
        public int Getdama()
        {
            return this.dama;
        }
    }
}
