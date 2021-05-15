using System;

namespace DgMultConstructors
{
    class Program
    {
        static void Main(string[] args)
        {
            var mc0 = new MultiConst();
            Console.WriteLine($"Without parameters: Size={mc0.Size} Manufacturer={mc0.Manufacturer}");
            
            var mc1 = new MultiConst("58'");            
            Console.WriteLine($"With One parameter: Size={mc1.Size} Manufacturer={mc1.Manufacturer}");

            var mc3 = new MultiConst("Samsung", "58'", true);
            
            Console.WriteLine($"With three parameters: Size={mc3.Size} Manufacturer={mc3.Manufacturer}");

        }
        class MultiConst
        {
            bool _smart;
             
            internal MultiConst()
            {
                Console.WriteLine();
                Console.WriteLine("Constructor 1 - Smart={0}", _smart);
            }
            internal MultiConst(string size)
            {
                this.Size = size;
                Console.WriteLine();
                Console.WriteLine("Constructor 2 - Smart={0}", _smart);
            }
            internal MultiConst(string manufacturer, string size, bool smart)
            {
                this.Size = size;
                this.Manufacturer = manufacturer;
                this._smart = smart;
                Console.WriteLine();
                Console.WriteLine("Constructor 3 - Smart={0}", _smart);
            }

            public string Manufacturer { get; set; }
            public string Size{ get; set; }
        }

    }
}
