using System;

namespace Reuniao.DataBase
{
    public class Cantico
    {
        private int chave = 0;
        public int Chave
        {
            get { return chave; }
            set { chave = value; }
        }
        
        private string tema;
        public string Tema
        {
            get { return tema; }
            set { tema = value; }
        }
    }
}
