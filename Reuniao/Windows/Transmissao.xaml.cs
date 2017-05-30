using System.Windows;

namespace Reuniao
{
    public partial class Transmissao : Window
    {
        #region Propriedades e Atributos

        public ucConteudo UserControlConteudo
        {
            get { return this.uccConteudo; }
        }

        #endregion

        #region Contrutor Lógico

        public Transmissao(ConteudoReuniao tipoConteudo, string valorConteudo)
        {
            InitializeComponent();

            this.uccConteudo.TipoConteudo = tipoConteudo;
            this.uccConteudo.ValorConteudo = valorConteudo;
        }

        #endregion

        #region Métodos Auxiliares

        #endregion

        #region Eventos da UI

        #endregion
    }
}
