using System;
using System.IO;

namespace Lab6
{
    class Program
    {
        static void Main(string[] args)
        {
            //string s = CriarArquivo();
            //Escrever(s);


            string caminho = Path.GetTempPath();

            string dir = $@"{caminho}NF";

            string arq = $@"{dir}\{DateTime.Now.ToString("ddMMyyyy")}.txt";

            Ler(arq);

            


        }

        private static string CriarArquivo()
        {
            string caminho = Path.GetTempPath();

            string dir = $@"{caminho}NF";

            string arq = $@"{dir}\{DateTime.Now.ToString("ddMMyyyy")}.txt";
            if (!Directory.Exists(caminho + "/" + dir))
            {
                Directory.CreateDirectory(dir);
                if (!File.Exists(arq))
                {
                    File.Create(arq);
                }
            };
            return arq;
            
        }
        private static void Escrever(string arq)
        {
            using (StreamWriter sw = new StreamWriter(arq, true))
            {
                sw.WriteLine($"{DateTime.Now} - Hello world");

                sw.Flush();
                sw.Close();
            }            
            
        }
        private static void Ler(string arq)
        {
            using(StreamReader r = new StreamReader(arq))
            {
                //string tudo = r.ReadToEnd();

                while (!r.EndOfStream)
                {
                    Console.WriteLine(r.ReadLine());

                }

                Console.ReadKey();                               
            }


        }
    }
}
