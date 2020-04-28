using System;
using System.Text;

namespace tst
{
    class Program
    {
        static void Main(string[] args)
        {
            // enum dia { dom,seg,ter,qua,qui,sex,sab};
            // dia dia1 = dia.dom;


            //Strbuil();
            //tstArray();
            //DgTry();
            Tv2 t2 = new Tv2();
            t2.MudarCanal(4);
            Console.WriteLine($"Tv2 no Canal:{t2.Canal}");
            t2.ConectarInternet();

            Tv t = t2;

            t.Ligar();
            t.MudarCanal(5);
            Console.WriteLine($"A tv1 está no canal {t.Canal}");
            //vai chamar tv.ConectarInternet, mas será substituido por tv2.ConectarInternet por causa de virtual
            //Se tirar virtual e override sera usado tv.ConnectarInternet
            t.ConectarInternet();

            Console.ReadKey();
        }

        private static void DgTry()
        {
            string c1 = string.Empty;
            string c2 = string.Empty;
            ComSaida(c1, out c2);
            Console.WriteLine($"c1={c1}, c2={c2}");
            int i;
            int b = 0;

            try
            {
                i = 1 / b;

            }

            catch (DivideByZeroException c)
            {
                Console.WriteLine("Tentou dividir por zero");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro:{e.Message}");

                //throw e;
            }
            finally
            {
                Console.WriteLine("Apos o erro");

            }
        }

        private static void tstArray()
        {
            int[] ar = new int[10];
            ar[0] = 100;
            ar[1] = 200;
            ar[2] = 300;
            Console.WriteLine(ar[1]);
            for (int i = 0; i < ar.Length; i++)
            {
                Console.WriteLine($"ar[{i}]={ar[i]}");

            }
        }

        private static void Strbuil()
        {
            StringBuilder s = new StringBuilder();
            s.Append("ab");
            s.Append("cd");
            string c = s.ToString();

            Console.WriteLine(c);
        }
        private static void ComSaida(string p1, out string p2)
        {
            p2 = "passei isto";
        }
    }
}
