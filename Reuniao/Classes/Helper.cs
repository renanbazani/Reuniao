using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace Reuniao
{
    public static class Helper
    {
        #region Propriedades e Atributos

        private static string pathCanticos = string.Format(@"{0}Arquivos\Cantico", System.AppDomain.CurrentDomain.BaseDirectory);
        public static string PathCanticos
        {
            get
            {
                if (!Directory.Exists(Helper.pathCanticos))
                    Directory.CreateDirectory(Helper.pathCanticos);

                return Helper.pathCanticos;
            }
        }

        private static string pathVideos = string.Format(@"{0}Arquivos\Video", System.AppDomain.CurrentDomain.BaseDirectory);
        public static string PathVideos
        {
            get
            {
                if (!Directory.Exists(Helper.pathVideos))
                    Directory.CreateDirectory(Helper.pathVideos);

                return Helper.pathVideos;
            }
        }

        private static string pathAudios = string.Format(@"{0}Arquivos\Audio", System.AppDomain.CurrentDomain.BaseDirectory);
        public static string PathAudios
        {
            get
            {
                if (!Directory.Exists(Helper.pathAudios))
                    Directory.CreateDirectory(Helper.pathAudios);

                return Helper.pathAudios;
            }
        }

        private static string pathImagens = string.Format(@"{0}Arquivos\Imagem", System.AppDomain.CurrentDomain.BaseDirectory);
        public static string PathImagens
        {
            get
            {
                if (!Directory.Exists(Helper.pathImagens))
                    Directory.CreateDirectory(Helper.pathImagens);

                return Helper.pathImagens;
            }
        }

        private static Transmissao transmissaoAtiva = null;
        public static Transmissao TransmissaoAtiva
        {
            get { return Helper.transmissaoAtiva; }
            set { Helper.transmissaoAtiva = value; }
        }

        private static MediaElement midiaAtual = null;
        public static MediaElement MidiaAtual
        {
            get { return Helper.midiaAtual; }
            set { Helper.midiaAtual = value; }
        }

        private static ucConteudo conteudoAtual = null;
        public static ucConteudo ConteudoAtual
        {
            get { return Helper.conteudoAtual; }
            set { Helper.conteudoAtual = value; }
        }

        private static double? height = null;
        private static double? top = null;

        #endregion

        #region Métodos Auxiliares

        public static void TransmitirConteudo(ConteudoReuniao tipoConteudo, string valorConteudo, bool janelaInativa = false)
        {
            //Fecha a transmissao ativa
            FecharTransmissaoAtiva();

            //Instancia Novamente a Window
            transmissaoAtiva = new Transmissao(tipoConteudo, valorConteudo);

            //Pega sempre o Segundo monitor encontrado
            var screen = System.Windows.Forms.Screen.AllScreens.Where(s => !s.Primary).FirstOrDefault();

            //Caso nao possua um monitor secundario, transmite para o monitor padrao
            if (screen == null)
                screen = System.Windows.Forms.Screen.AllScreens.Where(s => s.Primary).FirstOrDefault();

            //Define o tamanho da window de destino, de acordo com o tamanho do monitor
            var workingArea = screen.WorkingArea;

            /* SOMENTE PARA TESTES (Comentar as 2 linhas abaixo para poder testar redimensionando a janela de transmissao)
            transmissaoAtiva.WindowState = WindowState.Minimized;
             */

            transmissaoAtiva.WindowStyle = WindowStyle.None;
            transmissaoAtiva.ResizeMode = ResizeMode.NoResize;

            if (tipoConteudo == ConteudoReuniao.VIDEO || tipoConteudo == ConteudoReuniao.CANTICO)
                Helper.TransmissaoAtiva.UserControlConteudo.JWWatermark.Visibility = Visibility.Visible;

            if (janelaInativa)
            {
                transmissaoAtiva.Left = 0;
                transmissaoAtiva.Top = 0;
                transmissaoAtiva.Width = 0;
                transmissaoAtiva.Height = 0;
            }
            else
            {
                transmissaoAtiva.Left = workingArea.Left;
                transmissaoAtiva.Top = (top != null) ? (double)top : workingArea.Top;
                transmissaoAtiva.Width = workingArea.Width;
                transmissaoAtiva.Height = (height != null) ? (double)height : workingArea.Height;
            }

            transmissaoAtiva.Show();
            transmissaoAtiva.Activate();
        }

        public static void FecharTransmissaoAtiva()
        {
            //Fecha a transmissao ativa
            if (transmissaoAtiva != null)
                transmissaoAtiva.Close();
        }

        public static void FecharConteudoAtivo()
        {
            //Fecha a transmissao ativa
            if (conteudoAtual != null)
            {
                conteudoAtual.EncerrarExecucao();
                conteudoAtual = null;
            }
        }

        /// <summary>
        /// Obtem a data da reunião corrente.
        /// Se a data do parametro corresponder a umn dia na semana o método retornará o dia correspondente a quarta-feira. 
        /// Ou se caso a data do parametro corresponda a um doa no final-de-semana, o método retornará o dia correspondente ao sábado correspondente. 
        /// </summary>
        /// <param name="dataReuniao">Data da semana onde se deseja obter a programação da Reuniao.</param>
        /// <returns></returns>
        public static DateTime ObterDataReuniao(DateTime dataReuniao)
        {
            DateTime dataRetorno = dataReuniao;

            switch (dataReuniao.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    //Segunda é o inicio, não faz nenhum calculo
                    break;
                case DayOfWeek.Tuesday:
                    dataRetorno = dataReuniao.AddDays(-1);
                    break;
                case DayOfWeek.Wednesday:
                    dataRetorno = dataReuniao.AddDays(-2);
                    break;
                case DayOfWeek.Thursday:
                    dataRetorno = dataReuniao.AddDays(-3);
                    break;
                case DayOfWeek.Friday:
                    dataRetorno = dataReuniao.AddDays(-4);
                    break;
                case DayOfWeek.Saturday:
                    //Sábado é o inicio do Final de Semana, não faz nenhum calculo
                    break;
                case DayOfWeek.Sunday:
                    dataRetorno = dataReuniao.AddDays(-1);
                    break;
                default:
                    break;
            }

            return dataRetorno.Date;
        }

        #endregion
    }

    #region Converters

    public class ConteudoReuniaoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "null";

            return (ConteudoReuniao)Enum.Parse(typeof(ConteudoReuniao), value.ToString(), true);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    #endregion

    #region Entities e Enums

    public enum ConteudoReuniao
    {
        CANTICO = 1,
        VIDEO = 2,
        AUDIO = 3,
        IMAGEM = 4,
        NENHUM = 5
    }

    #endregion
}
