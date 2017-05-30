using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Reuniao.DataBase
{
    class SQLCanticos
    {
        #region Propriedades e Atributos

        SQLiteConnection conn = null;

        #endregion

        #region Contrutor Lógico

        public SQLCanticos()
        {
            this.conn = new SQLiteConnection(string.Format(@"Data Source={0}\Arquivos\Dados.sqlite;Version=3;", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)));
        }

        #endregion

        #region Métodos Privados

        #endregion

        #region Métodos Publicos

        public List<Cantico> Listar()
        {
            try
            {
                conn.Open();
                DataTable dt = new DataTable();
                var da = new SQLiteDataAdapter("select * from cantico", conn);
                da.Fill(dt);

                List<Cantico> list = new List<Cantico>();

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new Cantico()
                    {
                        Chave = int.Parse(row["Chave"].ToString()),
                        Tema = row["Tema"].ToString()
                    });
                }

                return list;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
                conn = null;
            }
        }

        public Cantico Listar(int chave)
        {
            try
            {
                conn.Open();
                DataTable dt = new DataTable();
                var da = new SQLiteDataAdapter(string.Format("select * from cantico where Chave = {0}", chave), conn);
                da.Fill(dt);

                Cantico obj = new Cantico();

                foreach (DataRow row in dt.Rows)
                {
                    obj = new Cantico()
                    {
                        Chave = int.Parse(row["Chave"].ToString()),
                        Tema = row["Tema"].ToString()
                    };
                }

                return obj;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
                conn = null;
            }
        }

        #endregion
    }
}
