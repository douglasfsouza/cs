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
            StreamWriter jj;
            StreamWriter rr;

            string t;
            string campo = null;
            string campoT = null;
            string campou;
            string lin;
            string linMdl = null;
            string linJson = null;
            string linr = null;
            string tipo;

            string a = "e:\\git\\cs\\CriarMap\\tabela.txt";
            string b = "e:\\git\\cs\\CriarMap\\map-u-tabela.txt";
            string c = "e:\\git\\cs\\CriarMap\\tipos-tabela.txt";
            string d = "e:\\git\\cs\\CriarMap\\mdl-tabela.txt";
            string j = "e:\\git\\cs\\CriarMap\\json-tabela.txt";
            string r = "e:\\git\\cs\\CriarMap\\to-record-tabela.txt";

            y = File.OpenText(a);
            x = File.CreateText(b);
            z = File.CreateText(c);
            k = File.CreateText(d);
            jj = File.CreateText(j);
            rr = File.CreateText(r);

            linMdl = "public override string EntityName => xx;";
            k.WriteLine(linMdl);
            linMdl = "public string Name " + "{" + " get; set; " + "}";
            k.WriteLine(linMdl);
            linMdl = "public string Code " + "{" + " get; set; " + "}";
            k.WriteLine(linMdl);

            lin = "map.Add(\"" + "name" + "\",\"" + "T" + "\");";
            z.WriteLine(lin);
            lin = "map.Add(\"" + "code" + "\",\"" + "T" + "\");";
            z.WriteLine(lin);

            lin = "map.Add(\"" + "name" + "\",\"" + "Name" + "\");";
            x.WriteLine(lin);
            lin = "map.Add(\"" + "code" + "\",\"" + "Code" + "\");";
            x.WriteLine(lin);

            linJson = "record.Name = entity.RecId.ToString();";
            jj.WriteLine(linJson);
            linJson = "record.Code = entity.RecId.ToString();";
            jj.WriteLine(linJson);

            linr = $"entity.Name = record.Name;";
            rr.WriteLine(linr);
            linr = $"entity.Code = record.Code;";
            rr.WriteLine(linr);

            while (!y.EndOfStream)
            {
                t = y.ReadLine();
                if (t.Trim().StartsWith("name =") )
                {
                    campo = t.Trim().Substring(7);
                    campo = campo.Replace("\"", null);
                    campo = campo.Replace(",", null);
                    campou = "U_" + campo;
                    lin = "map.Add(\"" + campo.ToLower() + "\",\"" +  campou.ToUpper() + "\");";                    
                    Console.WriteLine(lin);
                    x.WriteLine(lin);

                    linJson = $"record.{campou.ToUpper()} = entity.{campo.Substring(0, 1).ToUpper()}{campo.Substring(1).ToLower()};";
                    jj.WriteLine(linJson);

                    linr = $"entity.{campo.Substring(0, 1).ToUpper()}{campo.Substring(1).ToLower()} = record.{campou.ToUpper()};";
                    rr.WriteLine(linr);

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
                        linMdl = "public long " + campo.Substring(0, 1) + campo.Substring(1).ToLowerInvariant() + " { get; set; " + "}";
                    }
                        
                    if (campoT == "db_Alpha")
                    {                        
                        linMdl ="public string " + campo.Substring(0, 1) + campo.Substring(1).ToLowerInvariant() + " { get; set; " + "}";
                        tipo = "T"; 
                    }
                       
                    if (campoT == "db_Float")
                    {
                        linMdl = "public double " + campo.Substring(0,1) + campo.Substring(1).ToLowerInvariant() + " { get; set; " + "}";
                        tipo = "N";
                    }
                       
                    if (campoT == "db_Date")
                    {
                        linMdl = "public DateTime " + campo.Substring(0, 1) + campo.Substring(1).ToLowerInvariant() + " { get; set; " + "}";
                        tipo = "T";
                    }                        
                    
                    campou = tipo;
                    lin = "map.Add(\"" + campo.ToLower() + "\",\"" + campou.ToUpper() + "\");";
                                       
                    Console.WriteLine(lin);

                    z.WriteLine(lin);
                    k.WriteLine(linMdl);

                }
            }
            y.Close();
            x.Close();
            z.Close();
            k.Close();
            jj.Close();
            rr.Close();
            Console.WriteLine("Pressione uma tecla para finalizar");
            Console.ReadLine();
                
        }
    }
}
