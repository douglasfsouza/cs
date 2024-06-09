// See https://aka.ms/new-console-template for more information
namespace DgDelegate3
{
    public class DgDelegate3
    {
        public delegate TextWriter CoVariance();
        public static StreamWriter StreamMethod()
        {
            return null;
        }

        public static StringWriter StringMethod()
        {
            return null;
        }

        public static void Main()
        {
            CoVariance c = StreamMethod;
            c = StringMethod;

            Console.WriteLine("Exemplo de CoVariance Delegate");
        }

    }
    

}

