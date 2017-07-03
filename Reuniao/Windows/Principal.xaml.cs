using Reuniao.DataBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Reuniao
{
    public partial class Principal : Window
    {
        #region Propriedades e Atributos

        #endregion

        #region Contrutor Lógico

        public Principal()
        {
            InitializeComponent();

            this.Closed += Principal_Closed;
        }

        #endregion

        #region Métodos Auxiliares

        private void GerenciaMenu()
        {
            miNovoConteudo.IsEnabled = (cbiDataReunioes.SelectedValue != null);
        }

        private void PreencherComboDataReunioes()
        {
            cbiDataReunioes.DisplayMemberPath = "Key";
            cbiDataReunioes.SelectedValuePath = "Value";
            cbiDataReunioes.Items.Add(new KeyValuePair<string, DateTime?>("Selecione a Data da Reunião...", null));

            for (int i = 0; i < 2; i++)
            {
                for (int j = 1; j <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.AddMonths(i).Month); j++)
                {
                    DateTime date = new DateTime(DateTime.Now.AddMonths(i).Year, DateTime.Now.AddMonths(i).Month, j);

                    if (date.DayOfWeek == DayOfWeek.Monday)
                    {
                        cbiDataReunioes.Items.Add(
                                new KeyValuePair<string, DateTime?>(string.Format("Semana ({0} à {1})", date.ToString("dd/MMM"), date.AddDays(4).ToString("dd/MMM")), date));
                    }
                    else if (date.DayOfWeek == DayOfWeek.Saturday)
                    {
                        cbiDataReunioes.Items.Add(
                                new KeyValuePair<string, DateTime?>(string.Format("Final de Semana ({0} e {1})", date.ToString("dd/MMM"), date.AddDays(1).ToString("dd/MMM")), date));
                    }
                }

            }

            cbiDataReunioes.SelectedValue = null;
        }

        private void CarregarConteudoReuniao(DateTime? dataReuniao)
        {
            //Inicializa Conteudos sempre como 'Collapsed'
            lvQuadros.Visibility = System.Windows.Visibility.Collapsed;
            bdProgramacaoNaoDisponivel.Visibility = System.Windows.Visibility.Collapsed;

            if (dataReuniao == null)
                return;

            List<DataBase.Reuniao> listReuniao = new SQLReunioes().Listar((DateTime)dataReuniao);

            lvQuadros.ItemsSource = listReuniao;
            lvQuadros.Visibility = System.Windows.Visibility.Visible;
        }

        #endregion

        #region Eventos da UI

        private void Principal_Loaded(object sender, RoutedEventArgs e)
        {
            this.GerenciaMenu();
            this.PreencherComboDataReunioes();
        }

        private void Principal_Closed(object sender, EventArgs e)
        {
            //Fecha todas as janelas e instancias ativas
            Application.Current.Shutdown();
            System.Environment.Exit(0);
        }

        private void btnSelecionarDataCorrente_Click(object sender, RoutedEventArgs e)
        {
            //Fecha qualquer conteudo, caso exista um em andamento
            Helper.FecharConteudoAtivo();

            cbiDataReunioes.SelectedValue = Helper.ObterDataReuniao(DateTime.Now);
            this.CarregarConteudoReuniao((DateTime)cbiDataReunioes.SelectedValue);

            this.GerenciaMenu();
        }

        private void cbiDataReunioes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //Fecha qualquer conteudo, caso exista um em andamento
                Helper.FecharConteudoAtivo();

                this.CarregarConteudoReuniao((DateTime?)(((ComboBox)e.Source).SelectedValue));

                this.GerenciaMenu();
            }
            catch (Exception ex)
            {
                lvQuadros.Visibility = System.Windows.Visibility.Collapsed;

                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AdicionarCantico_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dataSelecionada = (DateTime)cbiDataReunioes.SelectedValue;

                new SQLReunioes().Adicionar(new DataBase.Reuniao()
                {
                    DataReuniao = dataSelecionada,
                    Tipo = "CANTICO",
                    Valor = "0"
                });

                lvQuadros.ItemsSource = new SQLReunioes().Listar(dataSelecionada);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AdicionarVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dataSelecionada = (DateTime)cbiDataReunioes.SelectedValue;

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".mp4";
                dlg.Filter = "Arquivos de Video |*.wmv; *.avi; *.mkv; *.mov; *.mp4; *.mpeg;";

                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    File.Copy(dlg.FileName, string.Format(@"{0}\{1}", Helper.PathVideos, Path.GetFileName(dlg.FileName)), true);

                    new SQLReunioes().Adicionar(new DataBase.Reuniao()
                    {
                        DataReuniao = dataSelecionada,
                        Tipo = "VIDEO",
                        Valor = Path.GetFileName(dlg.FileName)
                    });

                    lvQuadros.ItemsSource = new SQLReunioes().Listar(dataSelecionada);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AdicionarAudio_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dataSelecionada = (DateTime)cbiDataReunioes.SelectedValue;

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".mp3";
                dlg.Filter = "Arquivos de Audio (.mp3) |*.mp3;";

                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    File.Copy(dlg.FileName, string.Format(@"{0}\{1}", Helper.PathAudios, Path.GetFileName(dlg.FileName)), true);

                    new SQLReunioes().Adicionar(new DataBase.Reuniao()
                    {
                        DataReuniao = dataSelecionada,
                        Tipo = "AUDIO",
                        Valor = Path.GetFileName(dlg.FileName)
                    });

                    lvQuadros.ItemsSource = new SQLReunioes().Listar(dataSelecionada);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AdicionarImagem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dataSelecionada = (DateTime)cbiDataReunioes.SelectedValue;

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".jpeg";
                dlg.Filter = "Arquivos de Imagem (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    File.Copy(dlg.FileName, string.Format(@"{0}\{1}", Helper.PathImagens, Path.GetFileName(dlg.FileName)), true);

                    new SQLReunioes().Adicionar(new DataBase.Reuniao()
                    {
                        DataReuniao = dataSelecionada,
                        Tipo = "IMAGEM",
                        Valor = Path.GetFileName(dlg.FileName)
                    });

                    lvQuadros.ItemsSource = new SQLReunioes().Listar(dataSelecionada);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ucConteudo_ConteudoAtualizado(object sender, EventArgs e)
        {
            try
            {
                var dataSelecionada = (DateTime)cbiDataReunioes.SelectedValue;
                lvQuadros.ItemsSource = new SQLReunioes().Listar(dataSelecionada);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ucConteudo_AlterarConteudo(object sender, EventArgs e)
        {
            try
            {
                var dataSelecionada = (DateTime)cbiDataReunioes.SelectedValue;

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                switch ((sender as ucConteudo).TipoConteudo)
                {
                    case ConteudoReuniao.VIDEO:

                        dlg.DefaultExt = ".mp4";
                        dlg.Filter = "Arquivos de Video |*.wmv; *.avi; *.mkv; *.mov; *.mp4; *.mpeg;";

                        if (dlg.ShowDialog() == true)
                        {
                            if (File.Exists(string.Format(@"{0}\{1}", Helper.PathVideos, (sender as ucConteudo).Conteudo.Valor)))
                                File.Delete(string.Format(@"{0}\{1}", Helper.PathVideos, (sender as ucConteudo).Conteudo.Valor));

                            File.Copy(dlg.FileName, string.Format(@"{0}\{1}", Helper.PathVideos, Path.GetFileName(dlg.FileName)), true);

                            (sender as ucConteudo).Conteudo.Valor = Path.GetFileName(dlg.FileName);

                            new SQLReunioes().Alterar((sender as ucConteudo).Conteudo);

                            lvQuadros.ItemsSource = new SQLReunioes().Listar(dataSelecionada);
                        }

                        break;
                    case ConteudoReuniao.AUDIO:

                        dlg.DefaultExt = ".mp3";
                        dlg.Filter = "Arquivos de Audio (.mp3) |*.mp3;";

                        if (dlg.ShowDialog() == true)
                        {
                            if (File.Exists(string.Format(@"{0}\{1}", Helper.PathAudios, (sender as ucConteudo).Conteudo.Valor)))
                                File.Delete(string.Format(@"{0}\{1}", Helper.PathAudios, (sender as ucConteudo).Conteudo.Valor));

                            File.Copy(dlg.FileName, string.Format(@"{0}\{1}", Helper.PathAudios, Path.GetFileName(dlg.FileName)), true);

                            (sender as ucConteudo).Conteudo.Valor = Path.GetFileName(dlg.FileName);

                            new SQLReunioes().Alterar((sender as ucConteudo).Conteudo);

                            lvQuadros.ItemsSource = new SQLReunioes().Listar(dataSelecionada);
                        }

                        break;
                    case ConteudoReuniao.IMAGEM:

                        dlg.DefaultExt = ".jpeg";
                        dlg.Filter = "Arquivos de Imagem (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

                        if (dlg.ShowDialog() == true)
                        {
                            if (File.Exists(string.Format(@"{0}\{1}", Helper.PathImagens, (sender as ucConteudo).Conteudo.Valor)))
                                File.Delete(string.Format(@"{0}\{1}", Helper.PathImagens, (sender as ucConteudo).Conteudo.Valor));

                            File.Copy(dlg.FileName, string.Format(@"{0}\{1}", Helper.PathImagens, Path.GetFileName(dlg.FileName)), true);

                            (sender as ucConteudo).Conteudo.Valor = Path.GetFileName(dlg.FileName);

                            new SQLReunioes().Alterar((sender as ucConteudo).Conteudo);

                            lvQuadros.ItemsSource = new SQLReunioes().Listar(dataSelecionada);
                        }

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

        private void ucConteudo_ExcluirConteudo(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja Realmente Excluir?", "Atenção!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            try
            {
                switch ((sender as ucConteudo).Conteudo.Tipo)
                {
                    case "VIDEO":
                        if (File.Exists(string.Format(@"{0}\{1}", Helper.PathVideos, (sender as ucConteudo).Conteudo.Valor)))
                            File.Delete(string.Format(@"{0}\{1}", Helper.PathVideos, (sender as ucConteudo).Conteudo.Valor));
                        break;
                    case "AUDIO":
                        if (File.Exists(string.Format(@"{0}\{1}", Helper.PathAudios, (sender as ucConteudo).Conteudo.Valor)))
                            File.Delete(string.Format(@"{0}\{1}", Helper.PathAudios, (sender as ucConteudo).Conteudo.Valor));
                        break;
                    case "IMAGEM":
                        if (File.Exists(string.Format(@"{0}\{1}", Helper.PathImagens, (sender as ucConteudo).Conteudo.Valor)))
                            File.Delete(string.Format(@"{0}\{1}", Helper.PathImagens, (sender as ucConteudo).Conteudo.Valor));
                        break;
                    default:
                        break;
                }

                new SQLReunioes().Remover(int.Parse((sender as ucConteudo).Conteudo.Chave));
                lvQuadros.ItemsSource = new SQLReunioes().Listar((DateTime)(sender as ucConteudo).Conteudo.DataReuniao);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ucConteudo_AvancarSequencia(object sender, EventArgs e)
        {
            try
            {
                new SQLReunioes().AlterarSequencia((sender as ucConteudo).Conteudo, true);
                lvQuadros.ItemsSource = new SQLReunioes().Listar((DateTime)(sender as ucConteudo).Conteudo.DataReuniao);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ucConteudo_RetrocederSequencia(object sender, EventArgs e)
        {
            try
            {
                new SQLReunioes().AlterarSequencia((sender as ucConteudo).Conteudo, false);
                lvQuadros.ItemsSource = new SQLReunioes().Listar((DateTime)(sender as ucConteudo).Conteudo.DataReuniao);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void miSobre_Click(object sender, RoutedEventArgs e)
        {
            new Sobre().Show();
        }

        #endregion
    }
}
