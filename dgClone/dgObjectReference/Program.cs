using System;
using System.Xml.Schema;

namespace dgObjectReference
{
    class Program
    {
        static void Main(string[] args)
        {
            var tv0 = new TV("58'");
            var tv1 = tv0;

            Console.WriteLine($"tv0 Size={tv0.Size}");
            Console.WriteLine($"tv1 Size={tv1.Size}");

            Console.WriteLine();
            Console.WriteLine("Altering tv0");
            tv0.Size = "60'";

            Console.WriteLine($"tv0 Size={tv0.Size}");
            Console.WriteLine($"tv1 Size={tv1.Size}");

            Console.WriteLine();
            Console.WriteLine("Altering tv1");
            tv1.Size = "68'";

            Console.WriteLine($"tv0 Size={tv0.Size}");
            Console.WriteLine($"tv1 Size={tv1.Size}");

            var tv2 = tv1.Clone();
            Console.WriteLine();
            Console.WriteLine("Cloning");
            Console.WriteLine($"tv2 Size={tv2.Size}");

            Console.WriteLine();
            Console.WriteLine($"Altetering tv2");
            tv2.Size = "70'";
            Console.WriteLine($"tv0 Size={tv0.Size}");
            Console.WriteLine($"tv1 Size={tv1.Size}");
            Console.WriteLine($"tv2 Size={tv2.Size}");





        }
        class TV
        {
            bool _smart;

            internal TV()
            {
                Console.WriteLine();
                Console.WriteLine("Constructor 1 - Smart={0}", _smart);
            }
            internal TV(string size)
            {
                this.Size = size;
                Console.WriteLine();
                Console.WriteLine("Constructor 2 - Smart={0}", _smart);
            }
            internal TV(string manufacturer, string size, bool smart)
            {
                this.Size = size;
                this.Manufacturer = manufacturer;
                this._smart = smart;
                Console.WriteLine();
                Console.WriteLine("Constructor 3 - Smart={0}", _smart);
            }

            public TV Clone()
            {
                return (TV)this.MemberwiseClone();
            }




            public string Manufacturer { get; set; }
            public string Size { get; set; }
        }
        

    }
}
