using System;

namespace Reuniao.DataBase
{
    public class Reuniao
    {
        private string chave;
        public string Chave
        {
            get { return chave; }
            set { chave = value; }
        }

        private int sequencia;
        public int Sequencia
        {
            get { return sequencia; }
            set { sequencia = value; }
        }

        private string tipo;
        public string Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        private string valor;
        public string Valor
        {
            get { return valor; }
            set { valor = value; }
        }

        private DateTime? dataReuniao = null;
        public DateTime? DataReuniao
        {
            get { return dataReuniao; }
            set { dataReuniao = value; }
        }

        //Campos que nao estao no Banco de Dados
        private bool ultimoNaSequencia = false;
        public bool UltimoNaSequencia
        {
            get { return ultimoNaSequencia; }
            set { ultimoNaSequencia = value; }
        }
    }
}
