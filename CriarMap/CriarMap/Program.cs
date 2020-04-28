using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CriarMap
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamWriter x;
            StreamReader y;
            StreamWriter z;
            StreamWriter k;

            string t;
            string campo = null;
            string campoT = null;
            string campou;
            string lin;
            string linMdl = null;
            string tipo;

            string a = "c:\\git\\PROJETOS\\DGS\\cs\\CriarMap\\vspagtitulos3Srv.txt";
            string b = "c:\\git\\PROJETOS\\DGS\\cs\\CriarMap\\map-vsrettitulos3.txt";
            string c = "c:\\git\\PROJETOS\\DGS\\cs\\CriarMap\\map-tipo-vsrectitulos3.txt";
            string d = "c:\\git\\PROJETOS\\DGS\\cs\\CriarMap\\mdl-vsrectitulos3.txt";

            y = File.OpenText(a);
            x = File.CreateText(b);
            z = File.CreateText(c);
            k = File.CreateText(d);

            while (!y.EndOfStream)
            {
                t = y.ReadLine();
                if (t.Trim().StartsWith("name =") )
                {
                    campo = t.Trim().Substring(7);
                    campo = campo.Replace("\"", null);
                    campo = campo.Replace(",", null);
                    campou = "U_" + campo;
                    lin = "map.Add(\"" + campo + "\",\"" +  campou.ToUpper() + "\");";                    
                    Console.WriteLine(lin);
                    x.WriteLine(lin);

                }
                if (t.Trim().StartsWith("dataType ="))
                {
                    campoT = t.Trim().Substring(11);
                    campoT = campoT.Replace("\"", null);
                    campoT = campoT.Replace(",", null);

                    tipo = "X";
                    if (campoT == "db_Numeric")
                    {
                        tipo = "N";
                        linMdl = "public long " + campo + " { get; set; " + "}";
                    }
                        
                    if (campoT == "db_Alpha")
                    {                        
                        linMdl ="public string " + campo + " { get; set; " + "}";
                        tipo = "T"; 
                    }
                       
                    if (campoT == "db_Float")
                    {
                        linMdl = "public double " + campo + " { get; set; " + "}";
                        tipo = "N";
                    }
                       
                    if (campoT == "db_Date")
                    {
                        linMdl = "public DateTime " + campo + " { get; set; " + "}";
                        tipo = "T";
                    }                        
                    
                    campou = tipo;
                    lin = "map.Add(\"" + campo + "\",\"" + campou.ToUpper() + "\");";
                                       
                    Console.WriteLine(lin);

                    z.WriteLine(lin);
                    k.WriteLine(linMdl);

                }
            }
            y.Close();
            x.Close();
            z.Close();
            k.Close();
            Console.WriteLine("Pressione uma tecla para finalizar");
            Console.ReadLine();
                
        }
    }
}
