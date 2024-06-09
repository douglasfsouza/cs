using System;

namespace dgEnum
{
    class Program
    {
        static void Main(string[] args)
        {
            string version1 = "2";
            string version2 = "2.0";

            var v1 = version1.Split('.');
            var v2 = version2.Split(".");

            var ma1 = v1[0];
            var ma2 = v2[0];

            if (ma1.Equals(ma2))
            {
                Console.Write("hit");
            }

          


      

            Month m = Month.february;
            Console.WriteLine($"We are in {m}");
        }
        enum s
        {
            domingo = 0,
            segunda = 1,
            terça = 2,
            quarta = 3,
            quinta = 4,
            sexta = 5,
            sabado = 6

        };
        enum Month {january,
                    february,
                    march
       }
    }


}
