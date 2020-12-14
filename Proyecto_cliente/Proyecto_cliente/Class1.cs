using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proyecto_cliente
{
    class Ficha
    {
        string id;
        int posx;
        int posy;
        public void Setid(string identificador)
        {
            this.id = identificador;        
        }
        public void Setposx(int posx)
        {
            this.posx = posx;
        }
        public void Setposy(int posy)
        {
            this.posy = posy;
        }
        public string Getid()
        {
            return this.id;
 
        }
        public int Getposx()
        {
            return (this.posx);
        }
        public int Getposy()
        {
            return (this.posy);
        }


    }
}
