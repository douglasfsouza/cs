using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Globalization;

namespace dgTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            string line = "1234567";

            string lineMenor = line.PadLeft(3, '0'); //resulta 1234567

            string lineR = string.Empty;
            if (line.Length > 9)
            {
                lineR =  line.Substring(line.Length - 9);
            }
            else
            {
                lineR =  line.PadLeft(9, '0');
            }
            string xxx = "12345678901";
            string xxxFormated = Convert.ToInt64(xxx).ToString("D12");
            Console.WriteLine(xxxFormated);

            string seq = "fsfsaf{Banco}-{Agencia}";

            while (seq.Contains("{"))
            {
                int openPosition = seq.IndexOf("{");
                int closePosition = seq.IndexOf("}");
                string field = seq.Substring(openPosition, closePosition - openPosition + 1);
                seq = seq.Replace(field, string.Empty);
            }

            Console.WriteLine(seq);


            string fbanco = "adfadf{Banco";

            if (fbanco.Contains("{Banco}"))
            {
                Console.WriteLine("contem");
            }
            else
            {
                Console.WriteLine("nao contem");
            }
            string strConta = "12345678";
            long conta =  Convert.ToInt64(strConta)/10;
            long digConta = 0;
            Math.DivRem(conta,10,out digConta);


            string bn = 111031401014.ToString("D8").PadRight(15);
            Console.WriteLine(bn);

            string defaultComment = "Douglas";

            var sf =  string.Format("{0}-{1:dd/MM/yyyy}", defaultComment, DateTime.Now);

            Console.WriteLine(sf);

            if (Convert.ToInt64("") == 6 || Convert.ToInt64("02") == 6)
            {
               string protest =  1.ToString("D2");
            }
             
            string conv = FormatCovenant6("");

            string n756 = "123412345678901234567";
            long dac756 = CalcDAC756(n756.ToCharArray());
            List<string> rejectionCodes = new List<string>()
            {
                "03","11", "13","14","21","23","42","43","44","45", "51","52","53","54","58"
            };

            foreach (string rejectionCode in rejectionCodes)
            {
                Console.WriteLine($"{rejectionCode}001");
            }
            string n748 = "123456789012";
            long dac748 = CalcDAC748(n748.ToCharArray());
            string n001 = "12345678922";
            long dac001 = CalcDAC001(n001.ToCharArray());

            string dddata = "00000000";
            if (!string.IsNullOrWhiteSpace(dddata.Replace('0',' ')))
            {
                Console.WriteLine("data valida");
            }
            else
            {
                Console.WriteLine("data vazia");
            }
            string codbar = "1234567890123456789xx23456789012345678901234";
            string freeField = codbar.Substring(codbar.Length - 25);

            string banco = "BRAD".PadRight(30);
            string bancoL = "BRAD".PadLeft(30);
            Console.WriteLine(banco);
            Console.WriteLine(bancoL);


            string cov = "123456789012345678901234567890";
            

            double dblCovenant = 0;
            try
            {
                dblCovenant = Convert.ToDouble(cov);
            }
            catch
            {
                dblCovenant = 0;             
            }
            finally
            {
                if (dblCovenant > 0)
                {
                    string rconv = $"{cov:D20}";
                    Console.WriteLine(dblCovenant);

                }
                else
                {
                    Console.WriteLine("0".PadLeft(20,'0'));
                }

            }
             
            


            decimal valorForm = 1000;
            string valorForms = $"{valorForm:N2}";
            //string valorForms = valorForm.ToString("F2", CultureInfo.CurrentCulture);
            var cultura = new CultureInfo("pt-BR"); 
            string resultado = valorForm.ToString("F2", new CultureInfo("pt-BR")); // resultado = “1,234.57”
            Console.WriteLine(valorForms);

            var bancon = 1;
            var bancos = bancon.ToString("D3");

            string tg = "123456789";
            string t8 = string.Empty;
            if (tg.Length > 8)
            {
                t8 = tg.Substring(0, 8);
            }
            else
            {
                t8 = $"{Convert.ToInt64(tg):D8}";
            }
              
            Console.WriteLine(t8);

            DateTime nullDate = new DateTime();
            string settlementDate = Convert.ToInt64( "030723").ToString("00/00/00");
            nullDate = DateTime.Parse(settlementDate, new System.Globalization.CultureInfo("pt-BR"));
            if (nullDate == new DateTime())
            {
                Console.WriteLine("vazia");
            }
            else
            {
                Console.WriteLine($"dta valida {nullDate.ToString()}");
            }

            string nn033 = $"{1234567:D44}";

            string r033 = BankNumberDigitCalc(nn033.ToCharArray());

            string nn237 = "1234567890124";
            string r237 = FreeLineDigit(nn237.ToCharArray());

            long numToMod = 2000;
            long numMod = numToMod % 100;


            string cb = "10492772500000077492103443000105040000052450";
            string cbSemDac = "1049772500000077492103443000105040000052450";
            long newDac = CalcModule11(cbSemDac.ToCharArray());

            double dValue = 1000.5;
            long lValue =  (long)(dValue * 100 );
            Console.WriteLine(lValue);

            DateTime dataBase = new DateTime(1997, 10, 7);

            DateTime dueDate = new DateTime(2023, 10, 7);

            long diff = (dueDate - dataBase).Days;

            Console.WriteLine(diff);

            string code = "00571234511012345678";
            char[] ccode = code.ToCharArray();

            string final = code.Substring(code.Length - 3);
            Console.WriteLine(final);

            if (ccode.Length > 0 )
            {
                foreach (var arg in ccode)
                {
                    Console.WriteLine(arg);
                }
            }
            else
            {
                Console.WriteLine("vazio");
            }

            //long digit = FreeLineDigit(code.ToCharArray(),1);

            //Console.WriteLine("Digito: " +  digit);



            long ano = DateTime.Today.Year;
            long mes = DateTime.Today.Month;
            long dia = 1;
            DateTime firstDay = Convert.ToDateTime($"{ano}-{mes}-01");
            //DateTime lastDay = firstDay.AddDays(DateTime.DaysInMonth((int)ano, (int)mes));
            DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);

            Console.WriteLine(firstDay);
            Console.WriteLine(lastDay);
            //CustomerTest.GetAgeTest();

            //int xrevenue = 123456;

            //int xnewRevenue = 0;

            //Math.DivRem(xrevenue,10000,out xnewRevenue);
            //Console.WriteLine(xnewRevenue);

            long revenue = 123456;
            Math.DivRem(revenue,10000,out revenue);
            Console.WriteLine(revenue);




            string sourceDocument = "131546";
            string aSourceDocument = $"{Convert.ToInt64(sourceDocument):D16}";
            Console.WriteLine(aSourceDocument);

            string s = string.Empty;
            long n = 4;
            // s = $"{n:D4}";
           

            DateTime d = DateTime.Now;
            //s = $"{d:MMyyyy}";

            long v = 1234;
            s = $"{v:D2}";

            //s = string.Empty.PadLeft(5);

            s = "ab".PadLeft(5);




            Console.WriteLine(s);


        }

        static string FreeLineDigit(char[] code)
        {
            if (code.Length < 13)
            {
                return "0";
            }
            string mult = "2765432765432";
            long sum = 0;

            for (int i = 12; i >= 0; i--)
            {
                sum += Convert.ToInt32(code[i].ToString()) * Convert.ToInt32(mult.Substring(i, 1));
            }

            string result;

            long resultDigit;
            Math.DivRem(sum, 11, out resultDigit);
            if (resultDigit == 0)
            {
                result = "0";
            }
            else
            {
                result = (11 - resultDigit).ToString();
                if (result == "10")
                {
                    result = "P";
                }
            }

            return result;
        }

        static long CalcModule11(char[] code)
        {
            long multiplier = 4;
            long sum = 0;
            foreach (var item in code)
            {
                long digit = Convert.ToInt32(item.ToString());
                sum += digit * multiplier;

                if (--multiplier == 1)
                {
                    multiplier = 9;
                }
            }

            long resultDigit;
            Math.DivRem(sum, 11, out resultDigit);
            resultDigit = 11 - resultDigit;
            if (resultDigit > 9)
            {
                resultDigit = 1;
            }
            return resultDigit;
        }

        static string BankNumberDigitCalc(char[] code)
        {
            if (code.Length < 44)
            {
                return "0";
            }
            long mult = 2;
            long sum = 0;

            for (int i = 43; i >= 0; i--)
            {
                sum += Convert.ToInt32(code[i].ToString()) * mult;
                if (++mult > 8)
                {
                    mult = 2;
                }
            }

            string result;

            long resultDigit;
            Math.DivRem(sum, 11, out resultDigit);
            if (resultDigit <= 1)
            {
                result = "0";
            }
            else
            {
                result = (11 - resultDigit).ToString();
            }

            return result;
        }

        static long CalcDAC001(char[]code)
        {
            long sum = 0;
            char[] multiplier = "78923456789".ToCharArray();

            for (int i = 10; i >= 0; i--)
            {
                sum += (Convert.ToInt32(code[i].ToString()) * Convert.ToInt32(multiplier[i].ToString()));
            }

            long resultDigit;
            Math.DivRem(sum, 11, out resultDigit);
                         
            return resultDigit;
        }

        static long CalcDAC748(char[] code)
        {
            long sum = 0;
            long multiplier = 2;

            for (int i = code.Length - 1; i >= 0; i--)
            {
                sum += (Convert.ToInt32(code[i].ToString()) * multiplier);
                if (multiplier++ > 8)
                {
                    multiplier = 2;
                }
            }

            long resultDigit;
            Math.DivRem(sum, 11, out resultDigit);
            if (resultDigit <= 1)
            {
                resultDigit = 0;
            }
            else
            {
                resultDigit = 11 - resultDigit;
            }

            return resultDigit;
        }

        static long CalcDAC756(char[] code)
        {
            long sum = 0;
            char[] multiplier = "319731973197319731973".ToCharArray();

            for (int i = 20; i >= 0; i--)
            {
                sum += (Convert.ToInt32(code[i].ToString()) * Convert.ToInt32(multiplier[i].ToString()));
            }

            long resultDigit;
            Math.DivRem(sum, 11, out resultDigit);

            if (resultDigit <= 1)
            {
                resultDigit = 0;
            }
            else
            {
                resultDigit = 11 - resultDigit;
                if (resultDigit > 9)
                {
                    resultDigit = 0;
                }
            }

            return resultDigit;
        }


        static private string FormatCovenant6(string data)
        {
            string result = "0".PadLeft(6, '0');
            
            double dblCovenant = 0;
            try
            {
                dblCovenant = Convert.ToDouble(data);
            }
            catch
            {
                dblCovenant = 0;
            }
            finally
            {
                if (dblCovenant > 0)
                {
                    if (data.Length > 6)
                    {
                        result = data.Substring(0, 6);
                    }
                    else
                    {
                        result = data.PadLeft(6, '0');
                    }
                }
            }
            return result;
        }


    }
}
