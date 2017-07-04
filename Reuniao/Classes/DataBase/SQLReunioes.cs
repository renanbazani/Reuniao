using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Reuniao.DataBase
{
    class SQLReunioes
    {
        #region Propriedades e Atributos

        SQLiteConnection conn = null;
        bool manterConexao = false;

        #endregion

        #region Contrutor Lógico

        public SQLReunioes()
        {
            this.conn = new SQLiteConnection(string.Format(@"Data Source={0}\Arquivos\Dados.sqlite;Version=3;", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)));
        }

        #endregion

        #region Métodos Privados

        #endregion

        #region Métodos Publicos

        public List<Reuniao> Listar(DateTime dataReuniao)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var count = new SQLiteCommand(string.Format("select count(0) from reuniao where DataReuniao = date('{0}')", dataReuniao.ToString("yyyy-MM-dd")), conn).ExecuteScalar();

                DataTable dt = new DataTable();
                var da = new SQLiteDataAdapter(string.Format("select * from reuniao where DataReuniao = date('{0}') order by Sequencia", dataReuniao.ToString("yyyy-MM-dd")), conn);
                da.Fill(dt);

                List<Reuniao> list = new List<Reuniao>();

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new Reuniao()
                    {
                        Chave = row["Chave"].ToString(),
                        DataReuniao = Convert.ToDateTime(row["DataReuniao"]),
                        Sequencia = int.Parse(row["Sequencia"].ToString()),
                        Tipo = row["Tipo"].ToString(),
                        Valor = row["Valor"].ToString(),
                        Descricao = row["Descricao"].ToString(),
                        Reproduzido = bool.Parse(row["Reproduzido"].ToString()),
                        UltimoNaSequencia = (count.ToString().Equals(row["Sequencia"].ToString()))
                    });
                }

                return list;
            }
            finally
            {
                if (!this.manterConexao)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }
        }

        public void Adicionar(Reuniao objReuniao)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var chaveSeq = new SQLiteCommand("select ifnull(max(chave) + 1, 1) from reuniao", conn).ExecuteScalar();
                var proxSeq = new SQLiteCommand(string.Format("select ifnull(max(Sequencia) + 1, 1) from reuniao where DataReuniao = date('{0}')", ((DateTime)objReuniao.DataReuniao).ToString("yyyy-MM-dd")), conn).ExecuteScalar();

                new SQLiteCommand(string.Format("insert into reuniao values ({0}, {1}, '{2}', '{3}', '{4}', 'false', '{5}')",
                                                chaveSeq.ToString(),
                                                proxSeq.ToString(),
                                                objReuniao.Tipo,
                                                objReuniao.Valor,
                                                ((DateTime)objReuniao.DataReuniao).ToString("yyyy-MM-dd"),
                                                objReuniao.Descricao), conn).ExecuteNonQuery();
            }
            finally
            {
                if (!this.manterConexao)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }
        }

        public void Remover(int chave)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                new SQLiteCommand(string.Format("delete from reuniao where Chave = {0}", chave), conn).ExecuteNonQuery();
            }
            finally
            {
                if (!this.manterConexao)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }
        }

        public void Alterar(Reuniao objReuniao)
        {
            try
            {
                SQLiteCommand comm = new SQLiteCommand(conn);

                comm.CommandText = string.Format("update reuniao set DataReuniao = '{0}', Sequencia = {1}, Tipo = '{2}', Valor = '{3}', Descricao = '{4}', Reproduzido = '{5}' where Chave = {6}",
                                                ((DateTime)objReuniao.DataReuniao).ToString("yyyy-MM-dd"),
                                                objReuniao.Sequencia,
                                                objReuniao.Tipo,
                                                objReuniao.Valor,
                                                objReuniao.Descricao,
                                                objReuniao.Reproduzido.ToString().ToLower(),
                                                objReuniao.Chave);

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                comm.ExecuteNonQuery();
            }
            finally
            {
                if (!this.manterConexao)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }
        }

        public void AlterarSequencia(Reuniao objReuniao, bool avancar)
        {
            this.manterConexao = true;

            List<Reuniao> list = this.Listar((DateTime)objReuniao.DataReuniao);

            for (int i = 1; i <= list.Count; i++)
            {
                if (objReuniao.Sequencia == i)
                {
                    if (avancar)
                    {
                        objReuniao.Sequencia = objReuniao.Sequencia + 1;
                        this.Alterar(objReuniao);

                        list[i].Sequencia = list[i].Sequencia - 1;
                        this.Alterar(list[i]);

                        break;
                    }
                    else
                    {
                        objReuniao.Sequencia = objReuniao.Sequencia - 1;
                        this.Alterar(objReuniao);

                        list[i - 2].Sequencia = list[i - 2].Sequencia + 1;
                        this.Alterar(list[i - 2]);

                        break;
                    }
                }
            }
        }

        #endregion
    }
}
