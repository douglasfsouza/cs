using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgGirls
{
    class Program
    {
        static void Main(string[] args)
        {
            Contexto con = new Contexto();
            bool sair = false;
            
            while(sair == false)
            {
                Console.Clear();
                Console.WriteLine("Type a Girl:");
                string n = Console.ReadLine();
                if (n.Length != 0)
                {
                    if (Search(con, n))
                    {
                        Console.WriteLine("Ja cadastrada");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine("Type a Age:");
                        int i = int.Parse(Console.ReadLine());
                        AddGirls(n, i);
                    }
                }
                else
                    sair = true;


            }

            /*
            foreach (var i in con.Girls)
            {
                Console.WriteLine(i.Name);

            }

            Console.WriteLine("Com A:");

            IQueryable<Girl> g = (from gi in con.Girls
                                  where gi.Name.StartsWith("A")
                                 select gi);

            foreach (var i in g)
            {
                Console.WriteLine(i.Name);

            }
            

            if (Search(con, "Andreia"))
                Console.WriteLine("Existe Andreia");
            else
                Console.WriteLine("Nao Existe Andreia");

    */
            Console.WriteLine("Quiting..");
            Console.ReadKey();
        }

        private static bool Search(Contexto con, string name)
        {
            IQueryable<Girl> g = (from gi in con.Girls
                                  where gi.Name == name
                                  select gi);
            if (g.Count() == 0)
                return false;
            else
                return true;

        }


        private static void AddGirls(string name, int age)
        {
            using (Contexto con = new Contexto())
            {
                con.Girls.Add(new dgGirls.Girl()
                {
                    Name = name,
                    Age = age

                });
                
                con.SaveChanges();

            }
        }
    }
}
