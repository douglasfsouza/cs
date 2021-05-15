using System;
using System.IO;
using System.Text;

namespace dgArquivosStream
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Directory.Exists(@"c:\tst"))
            {
                Directory.CreateDirectory(@"c:\tst");
            }

            string filePath = @"c:\tst\fileStream.txt";
            FileStream file = new FileStream(filePath,FileMode.Create,FileAccess.Write);
            StreamWriter writer = new StreamWriter(file);

            string text = "Hello world with file stream!";

            writer.Write(text);
            writer.Flush();
            writer.Close();
            file.Close();       

            Console.WriteLine("Gerado o arquivo {0}", filePath);

            Console.WriteLine();
            Console.WriteLine("Leitura:");

            FileStream fileRead = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fileRead);

            StringBuilder data = new StringBuilder();

            while (reader.Peek() != -1)
            {
                data.Append((char)reader.Read());
            }
            Console.WriteLine(data.ToString());
            reader.Close();
            fileRead.Close();

            //reader:
            //reader.EndOfStream -> se chegou a final do arquivo
            //reader.Peek() -> Le o byte atual sem consumir
            //reader.ReadBlock -> le um bloco inteiro
            //reader.ReadLine -> Le uma linha inteira
            //reader.ReadToEnd -> Le da posicao atual ate o final

            ////writer:
            ///writer.autoflush -> ativar o modo de flush automatico
            ///writer.flush -> fazer o flush
            ///writer.newLine -> faz get ou set no caracter de quebra de linha
            ///writer.WriteLine -> grava a linha inteira e inclui a quebra
            ///writer.Write -> grava o byte passado
        }
    }
}
