using System;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace dgExpandoObject
{
    class Program
    {
        static void Main(string[] args)
        {
            dynamic dinamicObj = new ExpandoObject();
            dynamic system1 = new ExpandoObject();
            dynamic system2 = new ExpandoObject();

            system1.canAccess = false;
            system1.someFunction = someFunc(system1.canAccess);

            system2.canAccess = true;
            system2.someFunction = someFunc(system2.canAccess);

            Console.WriteLine("system1: {0}", system1.canAccess);
            Console.WriteLine("system1: {0}", system1.someFunction);
            Console.WriteLine("system2: {0}", system2.canAccess);
            Console.WriteLine("system2: {0}", system2.someFunction);



            static string someFunc(bool canAccess)
            {
                if (canAccess == true)
                {
                    return "You can access";
                }
                else
                {
                    return "Access denied!!";
                }

            }

            

        }
    }
}
