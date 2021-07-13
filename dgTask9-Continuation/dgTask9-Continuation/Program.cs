using System;
using System.Threading.Tasks;

namespace dgTask9_Continuation
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<string> first = new Task<string>(() => { return "Hello"; });
            Task<string> second = first.ContinueWith((antecedent) =>
            {
                return string.Format("{0} World!!!", antecedent.Result);
            });
            first.Start();

            Console.WriteLine(second.Result);
        }
    }
}
