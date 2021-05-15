using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arquivos
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamWriter x;
            StreamReader y;
            //string a = "c:\\tst\\arqcs.txt";
            //ou
            // @ tira a propriedade de scape da barra
            string a = @"c:\tst\arqcs.txt"; 

            string opc = "ler";
            string t;
            opc = "append";
            opc = "input";

            Console.WriteLine("Criar a pasta");
            if (!Directory.Exists(@"c:\tst"))
            {
                Directory.CreateDirectory(@"c:\tst");
            }
            Console.WriteLine(@"Pasta criada: c:\tst");
            //Directory.Delete(@"c:\tst2",true);

            switch (opc)
            {
                case "input":
                    y = File.OpenText(a);
                    while (!y.EndOfStream)
                    {
                        t = y.ReadLine();
                        Console.WriteLine(t);
                    }
                    y.Close();
                    
                    break;

                case "output":
                    x = File.CreateText(a);
                    x.WriteLine("Escrevendo no arquivo");
                    x.Close();
                    Console.WriteLine("Linha escrita");
                    break;
                case "append":
                    x = File.AppendText(a);
                    x.WriteLine("Escrevendo outra linha no arquivo");
                    x.Close();
                    Console.WriteLine("Outra linha escrita");

                    break;
                default:
                    y = File.OpenText(a);
                    break;
            }
            

            Console.WriteLine();
            Console.WriteLine("Ler tudo de uma so vez com ReadAllText:");
            string tudo = File.ReadAllText(a);
            Console.WriteLine(tudo);

            Console.WriteLine();
            Console.WriteLine("Ler linha por linha com RealAllLines:");
            string[] linhas = File.ReadAllLines(a);
            foreach (var item in linhas)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();
            Console.WriteLine("Ler byte por byte com RealAllBytes:");
            byte[] dbytes = File.ReadAllBytes(a);
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(dbytes));
            foreach (var item in dbytes)
            {
                Console.WriteLine(item);               
            }

            /*
            int i = 0;
            foreach (var item in dbytes)
            {                 
                byte[] b = new byte[item];

                b[i++] = item;
                
                Console.WriteLine(System.Text.Encoding.UTF8.GetString(b));
            }
            */


            Console.WriteLine();
            Console.WriteLine("Gravando com AppendAllLines");
            string[] appendLinhas = new string[] { "\r\nAppend Lines A", "Append Lines B"};

            File.AppendAllLines(a, appendLinhas);            

            Console.WriteLine();
            Console.WriteLine("Gravando com AppendAllText");
            string appendLinhasText = "\r\nAppend Text A \r\nAppend Text B";

            File.AppendAllText(a, appendLinhasText);

            Console.WriteLine("Excluir do bkp:");
            string bkp = @"c:\tst\bkp\arqcs.txt";
            if (File.Exists(bkp))
            {
                File.Delete(bkp);
            }            
            Console.ReadLine();

            if (!Directory.Exists(@"c:\tst\bkp"))
            {
                Directory.CreateDirectory(@"c:\tst\bkp");
            }

            Console.WriteLine("Copiar p / bkp:");
            bool boverwrite = true;
            File.Copy(a, bkp, boverwrite);

            Console.WriteLine("Data de criação do arquivo:");
            DateTime filecreate = File.GetCreationTime(a);
            Console.WriteLine("The file was created on {0}", filecreate);

            Console.WriteLine("Utilizando FileInfo");
            FileInfo fi = new FileInfo(a);
            Console.WriteLine("Copiando para info.txt");
            fi.CopyTo(@"c:\tst\info.txt");
            FileInfo fn = new FileInfo(@"c:\tst\info.txt");
            Console.WriteLine("Verificando se existe");
            if (fn.Exists)
            {
                Console.WriteLine("Existe, arquivo copiado com sucesso na pasta {0}", fn.DirectoryName);
            }
            else
            {
                Console.WriteLine("Não existe!");
            }
            Console.WriteLine("Tamanho: {0}", fn.Length);
            Console.WriteLine("Extensão: {0}", fn.Extension);
            Console.WriteLine("Excluindo");
            Console.ReadLine();

            Console.WriteLine();
            Console.WriteLine("Verificar os arquivos da pasta {0}:",fn.Directory.Name);
            string[] files = Directory.GetFiles(fn.DirectoryName);
            foreach (var item in files)
            {
                Console.WriteLine(item);

            }
            if (fn.Exists)
            {
                fn.Delete();
            }

            Console.WriteLine("DirectoryInfo.GetDirectories retorna o tipo DiretoryInfo em vez de string o que permite manipulação direta");
            DirectoryInfo directoryInfo = new DirectoryInfo(fi.DirectoryName);
            DirectoryInfo[] dirs  = directoryInfo.GetDirectories();
            foreach (var item in dirs)
            {
                Console.WriteLine(item);
                Console.WriteLine("Arquivos de {0}:", item.Name);
                FileInfo[] filesBkp = item.GetFiles();
                foreach (var fb in filesBkp)
                {
                    Console.WriteLine(fb.Name);
                }
            }

            string temp = Path.GetTempFileName();
            Console.WriteLine("Arquivo temporario: {0}", temp);
            Console.WriteLine("Tamanho: {0}", new FileInfo(temp).Length);
            File.AppendAllText(temp, "Hello");
            Console.WriteLine("Tamanho: {0}", new FileInfo(temp).Length);
            File.AppendAllText(temp, $"\r\nHoje é {DateTime.Today:dd-MM-yy}");
            string[] ltemp = File.ReadAllLines(temp);
            Console.WriteLine("Conteudo do arquivo temporario:");
            foreach (var lt in ltemp)
            {
                Console.WriteLine(lt);
            }


            Console.ReadLine();

        }
    }
}
