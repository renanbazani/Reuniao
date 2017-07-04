using Reuniao.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Reuniao
{
    public partial class ucConteudo : UserControl
    {
        #region Propriedades e Atributos

        private bool conteudoIndisponivel = false;

        private bool conteudoPrincipal = false;
        public bool ConteudoPrincipal
        {
            get
            {
                return conteudoPrincipal;
            }

            set
            {
                conteudoPrincipal = value;
            }
        }

        public static readonly DependencyProperty ConteudoProperty = DependencyProperty.Register("Conteudo", typeof(Reuniao.DataBase.Reuniao), typeof(ucConteudo), new PropertyMetadata(new Reuniao.DataBase.Reuniao()));
        public Reuniao.DataBase.Reuniao Conteudo
        {
            get { return (Reuniao.DataBase.Reuniao)GetValue(ConteudoProperty); }
            set { SetValue(TipoConteudoProperty, value); }
        }

        public static readonly DependencyProperty TipoConteudoProperty = DependencyProperty.Register("TipoConteudo", typeof(ConteudoReuniao), typeof(ucConteudo), new PropertyMetadata(ConteudoReuniao.NENHUM));
        public ConteudoReuniao TipoConteudo
        {
            get { return (ConteudoReuniao)GetValue(TipoConteudoProperty); }
            set { SetValue(TipoConteudoProperty, value); }
        }

        public static readonly DependencyProperty ValorConteudoProperty = DependencyProperty.Register("ValorConteudo", typeof(string), typeof(ucConteudo), new PropertyMetadata(string.Empty));
        public string ValorConteudo
        {
            get { return (string)GetValue(ValorConteudoProperty); }
            set { SetValue(ValorConteudoProperty, value); }
        }

        DispatcherTimer timerMedia;

        public event EventHandler AlterarConteudo;
        public event EventHandler ExcluirConteudo;
        public event EventHandler AvancarSequencia;
        public event EventHandler RetrocederSequencia;
        public event EventHandler ConteudoAtualizado;

        #endregion

        #region Contrutor Lógico

        public ucConteudo()
        {
            InitializeComponent();

            this.MouseDoubleClick += ucConteudo_MouseDoubleClick;
        }

        #endregion

        #region Métodos Auxiliares

        public void TransmiteCantico(int numero)
        {
            try
            {
                if (numero == 0)
                {
                    vbCantico.Visibility = Visibility.Visible;
                    cbCantico.SelectionChanged += cbCantico_SelectionChanged;
                    gridCanticoThumb.Visibility = System.Windows.Visibility.Visible;
                    return;
                }

                string pathArquivo = string.Format(@"{0}\{1}.mp4", Helper.PathCanticos, numero);

                this.ToolTip = string.Format("Cântico {0}", numero);

                if (this.ConteudoPrincipal)
                {
                    gridCanticoThumb.Visibility = System.Windows.Visibility.Collapsed;
                    gridCantico.Visibility = System.Windows.Visibility.Visible;

                    meCantico.Visibility = System.Windows.Visibility.Visible;
                    meCantico.LoadedBehavior = MediaState.Manual;
                    meCantico.UnloadedBehavior = MediaState.Manual;
                    meCantico.Source = new Uri(pathArquivo);
                    Helper.MidiaAtual = meCantico;
                }
                else
                {
                    tbNumeroThummb.Text = string.Format("{0}", numero);
                    gridCanticoThumb.Visibility = System.Windows.Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void TransmiteVideo(string nome)
        {
            try
            {
                gridVideo.Visibility = System.Windows.Visibility.Visible;

                string pathArquivo = string.Format(@"{0}\{1}", Helper.PathVideos, nome);

                if (!File.Exists(pathArquivo))
                {
                    videoThumb.Visibility = System.Windows.Visibility.Collapsed;
                    videoThumbErro.Visibility = System.Windows.Visibility.Visible;

                    this.conteudoIndisponivel = true;
                    return;
                }

                this.ToolTip = nome;

                if (this.ConteudoPrincipal)
                {
                    meVideo.Visibility = System.Windows.Visibility.Visible;
                    meVideo.LoadedBehavior = MediaState.Manual;
                    meVideo.UnloadedBehavior = MediaState.Manual;
                    meVideo.Source = new Uri(pathArquivo);
                    Helper.MidiaAtual = meVideo;
                }
                else
                {
                    gridVideo.Visibility = System.Windows.Visibility.Collapsed;
                    gridVideoThumb.Visibility = System.Windows.Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void TransmiteAudio(string nome)
        {
            try
            {
                gridAudioThumb.Visibility = System.Windows.Visibility.Visible;

                string pathArquivo = string.Format(@"{0}\{1}", Helper.PathAudios, nome);

                if (!File.Exists(pathArquivo))
                {
                    audioThumb.Visibility = System.Windows.Visibility.Collapsed;
                    audioThumbErro.Visibility = System.Windows.Visibility.Visible;

                    this.conteudoIndisponivel = true;
                    return;
                }

                this.ToolTip = nome;

                if (this.ConteudoPrincipal)
                {
                    meAudio.LoadedBehavior = MediaState.Manual;
                    meAudio.UnloadedBehavior = MediaState.Manual;
                    meAudio.Source = new Uri(pathArquivo);
                    Helper.MidiaAtual = meAudio;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void TransmiteImagem(string nome)
        {
            try
            {
                string pathArquivo = string.Format(@"{0}\{1}", Helper.PathImagens, nome);

                if (!File.Exists(pathArquivo))
                {
                    this.conteudoIndisponivel = true;
                    return;
                }

                ibImagem.Source = new BitmapImage(new Uri(pathArquivo));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Atencao: Metodo chamado pelo Helper 'FecharConteudoAtivo()'
        //Tomar cuidado para nao chamar ele aqui dentro e nao dar loop
        public void EncerrarExecucao()
        {
            try
            {
                if (timerMedia != null)
                {
                    timerMedia.Stop();
                    timerMedia = null;
                }

                pbMedia.Value = 0;
                gridControleMidia.Visibility = System.Windows.Visibility.Collapsed;

                Helper.FecharTransmissaoAtiva();

                if (Helper.MidiaAtual != null)
                {
                    Helper.MidiaAtual.Stop();
                    Helper.MidiaAtual = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void PopulaCombos()
        {
            try
            {
                //Cantico
                List<DataBase.Cantico> listCanticos = new DataBase.SQLCanticos().Listar();

                if (listCanticos != null)
                {
                    cbCantico.ItemsSource = null;
                    cbCantico.DisplayMemberPath = "Chave";
                    cbCantico.SelectedValuePath = "Chave";
                    cbCantico.ItemsSource = listCanticos;
                }
                //
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Eventos da UI

        private void ucConteudo_Loaded(object sender, RoutedEventArgs e)
        {
            gridImagem.Visibility = System.Windows.Visibility.Hidden;
            gridCantico.Visibility = System.Windows.Visibility.Hidden;
            gridCanticoThumb.Visibility = System.Windows.Visibility.Hidden;
            gridVideo.Visibility = System.Windows.Visibility.Hidden;
            gridAudioThumb.Visibility = System.Windows.Visibility.Hidden;
            gridControleMidia.Visibility = System.Windows.Visibility.Hidden;

            try
            {
                switch (this.TipoConteudo)
                {
                    case ConteudoReuniao.CANTICO:
                        this.TransmiteCantico(int.Parse(this.ValorConteudo));
                        break;
                    case ConteudoReuniao.VIDEO:
                        this.TransmiteVideo(this.ValorConteudo);
                        break;
                    case ConteudoReuniao.AUDIO:
                        this.TransmiteAudio(this.ValorConteudo);
                        break;
                    case ConteudoReuniao.IMAGEM:
                        gridImagem.Visibility = System.Windows.Visibility.Visible;
                        this.TransmiteImagem(this.ValorConteudo);
                        break;
                    case ConteudoReuniao.NENHUM:
                        break;
                    default:
                        break;
                }

                if (this.Conteudo.Reproduzido)
                {
                    this.gridInfoReproduzido.Visibility = Visibility.Visible;
                    this.miMarcar.Header = "Marcar como 'Pendente'";
                    this.MouseDoubleClick -= ucConteudo_MouseDoubleClick;
                }

                this.PopulaCombos();

                //Habilitar ou Desabilitar Botoes de Sequencia
                miRetrocederSequencia.IsEnabled = (this.Conteudo.Sequencia != 1);
                miAvancarSequencia.IsEnabled = !(this.Conteudo.UltimoNaSequencia);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ucConteudo_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (this.conteudoIndisponivel)
                {
                    MessageBox.Show("Conteúdo não disponivel na biblioteca!", "Atenção!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Helper.FecharConteudoAtivo();

                Helper.ConteudoAtual = this;

                gridControleMidia.Visibility = System.Windows.Visibility.Visible;

                if (!this.ConteudoPrincipal)
                {
                    switch (this.TipoConteudo)
                    {
                        case ConteudoReuniao.CANTICO:

                            Helper.TransmitirConteudo(this.TipoConteudo, this.ValorConteudo);

                            btnPlay.Visibility = Visibility.Visible;
                            btnStop.Visibility = Visibility.Visible;
                            break;
                        case ConteudoReuniao.VIDEO:

                            Helper.TransmitirConteudo(this.TipoConteudo, this.ValorConteudo);

                            btnPlay.Visibility = Visibility.Visible;
                            btnStop.Visibility = Visibility.Visible;
                            break;
                        case ConteudoReuniao.AUDIO:

                            Helper.TransmitirConteudo(this.TipoConteudo, this.ValorConteudo, true);

                            btnPlay.Visibility = Visibility.Visible;
                            btnStop.Visibility = Visibility.Visible;
                            break;
                        case ConteudoReuniao.IMAGEM:

                            Helper.TransmitirConteudo(this.TipoConteudo, this.ValorConteudo);

                            btnStop.Visibility = Visibility.Visible;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void miMarcar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Conteudo.Reproduzido = !this.Conteudo.Reproduzido;

                new SQLReunioes().Alterar(this.Conteudo);

                if (this.ConteudoAtualizado != null)
                    this.ConteudoAtualizado(this, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void miAlterarConteudo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (this.TipoConteudo)
                {
                    case ConteudoReuniao.CANTICO:
                        this.MouseDoubleClick -= ucConteudo_MouseDoubleClick;
                        vbCantico.Visibility = Visibility.Visible;
                        cbCantico.SelectionChanged += cbCantico_SelectionChanged;
                        gridCanticoThumb.Visibility = System.Windows.Visibility.Visible;
                        cmMenu.IsEnabled = true;
                        break;
                    case ConteudoReuniao.VIDEO:
                        if (this.AlterarConteudo != null)
                            this.AlterarConteudo(this, e);
                        break;
                    case ConteudoReuniao.AUDIO:
                        if (this.AlterarConteudo != null)
                            this.AlterarConteudo(this, e);
                        break;
                    case ConteudoReuniao.IMAGEM:
                        if (this.AlterarConteudo != null)
                            this.AlterarConteudo(this, e);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void miExcluirConteudo_Click(object sender, RoutedEventArgs e)
        {
            if (this.ExcluirConteudo != null)
                this.ExcluirConteudo(this, e);
        }

        private void miRetrocederSequencia_Click(object sender, RoutedEventArgs e)
        {
            if (this.RetrocederSequencia != null)
                this.RetrocederSequencia(this, e);
        }

        private void miAvancarSequencia_Click(object sender, RoutedEventArgs e)
        {
            if (this.AvancarSequencia != null)
                this.AvancarSequencia(this, e);
        }

        private void cbCantico_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbCantico == null || cbCantico.ItemsSource == null)
                    return;

                int valorSelecionado = (e.AddedItems[0] as DataBase.Cantico).Chave;

                if (valorSelecionado == 0)
                    return;

                string pathArquivo = string.Format(@"{0}\{1}.mp4", Helper.PathCanticos, valorSelecionado);

                if (!File.Exists(pathArquivo))
                {
                    MessageBox.Show("Arquivo de Vídeo do Cântico Indisponível!", "Atenção!", MessageBoxButton.OK, MessageBoxImage.Error);
                    PopulaCombos();
                    return;
                }

                this.Conteudo.Valor = valorSelecionado.ToString();

                //Gravar a selecao no banco
                new SQLReunioes().Alterar(this.Conteudo);

                vbCantico.Visibility = System.Windows.Visibility.Collapsed;
                this.MouseDoubleClick += ucConteudo_MouseDoubleClick;
                this.ValorConteudo = valorSelecionado.ToString();
                this.TransmiteCantico(valorSelecionado);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Controle de Midia

        private void btnPlay_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            try
            {
                if (Helper.MidiaAtual != null)
                {
                    btnPlay.Visibility = Visibility.Collapsed;
                    btnPause.Visibility = Visibility.Visible;
                    btnStop.Visibility = Visibility.Visible;
                    pbMedia.Visibility = Visibility.Visible;

                    if (this.TipoConteudo == ConteudoReuniao.VIDEO)
                        Helper.TransmissaoAtiva.UserControlConteudo.videoThumb.Visibility = System.Windows.Visibility.Collapsed;

                    if (timerMedia == null)
                    {
                        timerMedia = new DispatcherTimer();
                        timerMedia.Interval = TimeSpan.FromMilliseconds(200);
                        timerMedia.Tick += timer_Tick;
                    }

                    timerMedia.Start();

                    Helper.MidiaAtual.MediaOpened += me_MediaOpened;
                    Helper.MidiaAtual.MediaEnded += me_MediaEnded;

                    Helper.MidiaAtual.Play();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnPause_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            try
            {
                if (Helper.MidiaAtual != null)
                {
                    btnPlay.Visibility = Visibility.Visible;
                    btnPause.Visibility = Visibility.Collapsed;

                    if (timerMedia != null)
                        timerMedia.Stop();

                    Helper.MidiaAtual.Pause();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnStop_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            try
            {
                btnPlay.Visibility = Visibility.Collapsed;
                btnPause.Visibility = Visibility.Collapsed;
                btnStop.Visibility = Visibility.Collapsed;
                pbMedia.Visibility = Visibility.Collapsed;

                Helper.FecharConteudoAtivo();

                if (MessageBox.Show("Deseja marcar o conteúdo como 'Transmitido'?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    miMarcar_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                pbMedia.Value = Helper.MidiaAtual.Position.TotalSeconds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void me_MediaOpened(object sender, RoutedEventArgs e)
        {
            try
            {
                TimeSpan ts = Helper.MidiaAtual.NaturalDuration.TimeSpan;
                pbMedia.Maximum = ts.TotalSeconds;
                pbMedia.SmallChange = 1;
                pbMedia.LargeChange = Math.Min(10, ts.Seconds / 10);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void me_MediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                btnPlay.Visibility = Visibility.Collapsed;
                btnPause.Visibility = Visibility.Collapsed;
                btnStop.Visibility = Visibility.Collapsed;
                pbMedia.Visibility = Visibility.Collapsed;

                Helper.FecharConteudoAtivo();

                miMarcar_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #endregion
    }
}