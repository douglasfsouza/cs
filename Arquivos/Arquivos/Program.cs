using System;
using System.Collections.Generic;
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
            string a = "c:\\tst\\arqcs.txt";

            string opc = "ler";
            string t;
            opc = "append";
            opc = "input";

            switch (opc)
            {
                case "input":
                    y = File.OpenText(a);
                    while (!y.EndOfStream)
                    {
                        t = y.ReadLine();
                        Console.WriteLine(t);
                    }
                    
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
            Console.ReadLine();          
                

       
            
        }
    }
}
