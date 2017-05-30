using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Reuniao.DataBase;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Windows;
using System.Xml;
using System.Xml.Resolvers;
using System.Xml.Linq;
using HtmlAgilityPack;
using System.Text;

namespace Reuniao
{
    public partial class TesteGoogleDrive : Window
    {
        public TesteGoogleDrive()
        {
            InitializeComponent();

            #region Exemplo para Utilizar o Google Drive
            /*
            UserCredential credenciais;

            string pathCredencialGoogleDrive = string.Format(@"{0}CredencialGoogleDrive\", System.AppDomain.CurrentDomain.BaseDirectory);

            if (!Directory.Exists(pathCredencialGoogleDrive))
                Directory.CreateDirectory(pathCredencialGoogleDrive);

            credenciais = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets()
                {
                    ClientId = "842501674555-a2f7k2hj5sgqsk4ttf6iouc8n1ptj7bj.apps.googleusercontent.com",
                    ClientSecret = "qlvTH1epuSOI8Tp5PQbt5Rjy"
                },
                new string[] { DriveService.Scope.Drive },
                "user",
                CancellationToken.None,
                new FileDataStore(pathCredencialGoogleDrive, true)).Result;

            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credenciais
            });

            //Criar Pasta no Google Drive
            var fileMetadata1 = new Google.Apis.Drive.v3.Data.File();
            fileMetadata1.Name = "Reuniao";
            fileMetadata1.MimeType = "application/vnd.google-apps.folder";
            var request1 = service.Files.Create(fileMetadata1);
            request1.Fields = "id";
            var file1 = request1.Execute();

            //Inserir arquivo local na pasta criada anteriormente
            var folderId = file1.Id;
            var fileMetadata2 = new Google.Apis.Drive.v3.Data.File();
            fileMetadata2.Name = "photo.jpg";
            fileMetadata2.Parents = new List<string> { folderId };
            FilesResource.CreateMediaUpload request2;
            using (var stream = new System.IO.FileStream(string.Format(@"{0}Imagem\402016122_univ_cnt_1.jpg", System.AppDomain.CurrentDomain.BaseDirectory), System.IO.FileMode.Open))
            {
                request2 = service.Files.Create(
                    fileMetadata2, stream, "image/jpeg");
                request2.Fields = "id";
                request2.Upload();
            }
            var file2 = request2.ResponseBody;
             * 
             * */
            #endregion

            #region Exemplo para Compactar ou Nao os Arquivos
            /*
            //Ler Arquivos de um ZIP
            using (ZipArchive archive = ZipFile.OpenRead(string.Format(@"{0}Debug.zip", System.AppDomain.CurrentDomain.BaseDirectory)))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    //verifica se a entrada é uma pasta ou nao
                    bool isDir = (File.GetAttributes(entry.FullName) & FileAttributes.Directory) == FileAttributes.Directory;
                }
            }


            //Gravar Arquivos em um ZIP
            using (var memoryStream = new MemoryStream())
            {
                using (var novoArquivoZip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    //Criar nova entrada (arquivo ou pasta)
                    var novoArquivo = novoArquivoZip.CreateEntryFromFile(string.Format(@"{0}Cantico\001.mp3", System.AppDomain.CurrentDomain.BaseDirectory), "Audio/Teste.mp3");
                }

                using (var fileStream = new FileStream(string.Format(@"{0}Teste.zip", System.AppDomain.CurrentDomain.BaseDirectory), FileMode.Create))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                }
            }
             * */
            #endregion

            #region Popular Canticos
            /*
            #region Popular Canticos (Antigos)

            for (int i = 1; i <= 135; i++)
            {
                string tema = string.Empty;
                string texto = string.Empty;
                string letra = string.Empty;

                HtmlDocument htmlDoc = new HtmlDocument();

                htmlDoc.OptionFixNestedTags = true;

                htmlDoc.Load(string.Format(@"C:\Users\rbazani\OneDrive\Salão do Reino\ePubs (Extraidos)\sn_T (Canticos)\OEBPS\{0}_SN_{1}.xhtml", (i + 2).ToString("000"), i.ToString("000")), Encoding.UTF8);

                if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
                {
                    MessageBox.Show("Erros!");
                }
                else
                {

                    if (htmlDoc.DocumentNode != null)
                    {
                        HtmlNode bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");

                        if (bodyNode != null)
                        {
                            HtmlNodeCollection nodes = bodyNode.SelectNodes("//div//p//b");

                            foreach (HtmlNode node in nodes)
                            {
                                if (!string.IsNullOrEmpty(node.InnerText))
                                {
                                    tema = node.InnerText;
                                    break;
                                }
                            }

                            HtmlNodeCollection nodes2 = bodyNode.SelectNodes("//div//p//a");

                            foreach (HtmlNode node in nodes2)
                            {
                                if (!string.IsNullOrEmpty(node.InnerText))
                                {
                                    texto = node.InnerText;
                                    break;
                                }
                            }

                            List<HtmlNode> nodesLetra = bodyNode.Descendants("div").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("_L")).ToList();

                            letra = "";

                            foreach (HtmlNode node in nodesLetra)
                            {
                                if (!string.IsNullOrEmpty(node.InnerText))
                                {
                                    if (!node.InnerText.Replace("\r\n", "").Contains("(REFRÃO)"))
                                        letra += node.InnerText.Replace("\r\n", "").Trim() + "|";
                                }
                            }
                        }
                    }
                }

                new SQLCanticos().Adicionar(new DataBase.Cantico()
                {
                    Numero = i,
                    Tema = tema,
                    Texto = texto,
                    Letra = letra
                });

                texto = string.Empty;
            }
            
            #endregion

            #region Popular Canticos (Novos)

            for (int i = 136; i <= 150; i++)
            {
                string tema = string.Empty;
                string texto = string.Empty;
                string letra = string.Empty;

                HtmlDocument htmlDoc = new HtmlDocument();

                htmlDoc.OptionFixNestedTags = true;

                htmlDoc.Load(string.Format(@"C:\Users\rbazani\OneDrive\Salão do Reino\ePubs (Extraidos)\snnw_T  (Canticos Novos)\OEBPS\1102014{0}.xhtml", (i + 414 + ((i >= 139) ? 1 : 0)).ToString("000")), Encoding.UTF8);

                if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
                {
                    MessageBox.Show("Erros!");
                }
                else
                {

                    if (htmlDoc.DocumentNode != null)
                    {
                        HtmlNode bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");

                        if (bodyNode != null)
                        {
                            HtmlNodeCollection nodes = bodyNode.SelectNodes("//header//h1//strong");

                            foreach (HtmlNode node in nodes)
                            {
                                if (!string.IsNullOrEmpty(node.InnerText))
                                {
                                    tema = node.InnerText;
                                    break;
                                }
                            }

                            HtmlNodeCollection nodes2 = bodyNode.SelectNodes("//p//a");

                            foreach (HtmlNode node in nodes2)
                            {
                                if (!string.IsNullOrEmpty(node.InnerText))
                                {
                                    texto = node.InnerText.Replace("⁠", "");
                                    break;
                                }
                            }

                            HtmlNodeCollection nodes3 = bodyNode.SelectNodes("//div//div//div//ol//li//p");

                            letra = "";

                            foreach (HtmlNode node in nodes3)
                            {
                                if (!string.IsNullOrEmpty(node.InnerText))
                                {
                                    if (!node.InnerText.Replace("\r\n", "").Contains("(REFRÃO)"))
                                        letra += node.InnerText.Replace("\r\n", "").Trim() + "|";
                                }
                            }
                        }
                    }
                }

                new SQLCanticos().Adicionar(new DataBase.Cantico()
                {
                    Numero = i,
                    Tema = tema,
                    Texto = texto,
                    Letra = letra
                });

                texto = string.Empty;
            }

            #endregion
            */
            #endregion

            MessageBox.Show("OK");
        }

    }

}
