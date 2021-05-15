using System;
using System.Collections.Generic;
using System.Linq;

namespace DgLinq
{
    class Program
    {
        static void Main(string[] args)
        {
            string s = "1.90";            
            double dd = Convert.ToDouble(s)/100;
            decimal d = Convert.ToDecimal(s)/100;                      

            Console.WriteLine($"s={s}, decimal={d}, double={dd}");
            string b = "bol_+";
            b = Uri.EscapeUriString(b);

            Console.WriteLine("b={0}", b);

            string s1 = "abx";
            string s2 = "x";
            if (s1.Length > 2)
              s2 = s1.Substring(0,2);
            Console.WriteLine(s2);

            
            Products pro = new Products();
            List<Products> pros = new List<Products>();
            pro.Id = 1;
            pro.Description = "Book";
            pro.Category = 10;
            pro.Price = 100;
            pros.Add(pro);

            pro = new Products();
            pro.Id = 2;
            pro.Description = "Table";
            pro.Category = 20;
            pro.Price = 1000;
            pros.Add(pro);

            pro = new Products();
            pro.Id = 3;
            pro.Description = "Ball";
            pro.Category = 30;
            pro.Price = 90;
            pros.Add(pro);

            pro = new Products();
            pro.Id = 4;
            pro.Description = "Racket";
            pro.Category = 30;
            pro.Price = 90;
            pros.Add(pro);

            pro = new Products();
            pro.Id = 5;
            pro.Description = "Without Category";
            pro.Category = 40;
            pro.Price = 50;
            pros.Add(pro);

            Category cat = new Category();
            List<Category> cats = new List<Category>();
            cat.Id = 10;
            cat.Description = "Paper";
            cats.Add(cat);

            cat = new Category();
            cat.Id = 20;
            cat.Description = "Cooking";
            cats.Add(cat);
           
            cat = new Category();
            cat.Id = 30;
            cat.Description = "Sport";
            cats.Add(cat);

            Console.WriteLine("Produtos abaixo de 100 ou igual:");
            var proHigh =
                  from p in pros
                  where p.Price <= 100
                  select p;
            foreach (var p in proHigh)
            {
                Console.WriteLine($"{p.Description} - {p.Price}");
            }

            Console.WriteLine();
            Console.WriteLine("Categorias join Produtos:");

            var catQuery =
                      from c in cats
                      join p in pros on c.Id equals p.Category
                      select new { categoria = c.Description, produto = p.Description };
            foreach (var c in catQuery)
            {
                Console.WriteLine($"Produto: {c.produto} - Categoria: {c.categoria}");
            }
            Console.WriteLine();
            Console.WriteLine("Produtos com categoria ou sem:");
            var proQuery =
                    from p in pros
                    join c in cats on p.Category equals c.Id into categ
                    from catn in categ.DefaultIfEmpty()                    
                    select new { produto = p.Description, categoria = p.Category};
            foreach (var p in proQuery)
            {
                Console.WriteLine($"Produto: {p.produto} - Categoria: {p.categoria}");

            }
            Console.WriteLine();
            Console.WriteLine("Produtos sem categoria:");
            var proQuerys =
                    from p in pros
                    join c in cats on p.Category equals c.Id into categ
                    from catn in categ.DefaultIfEmpty()
                    where catn == null                    
                    select new { produto = p.Description, categoria = catn == null ? "(No added)" : p.Category.ToString() };
            foreach (var p in proQuerys)
            {
                Console.WriteLine($"Produto: {p.produto} - Categoria: {p.categoria}");

            }

            Console.WriteLine();
            Console.WriteLine("Usando IEnumerable e Orderby:");

            IEnumerable<string> priceQuery =
                        from p in pros
                        where p.Price > 10
                        orderby p.Price ascending
                        select $"The price is {p.Price}";
            foreach (var p in priceQuery)
            {
                Console.WriteLine(p);
            }

            Console.WriteLine();
            Console.WriteLine("Agrupamento por categoria:");
            var agrp =
                  from p in pros
                  group p by p.Category into agrpro
                  select new { 
                              cat = agrpro.Key, 
                              tot = agrpro.Sum(t => t.Price),
                              qtd = agrpro.Count()
                  };

            foreach (var p in agrp)
            {
                Console.WriteLine($"Categoria: {p.cat} - Total: {p.tot} Qtd: {p.qtd}");
                
            }
            Console.WriteLine();

            Console.WriteLine("Linq - Procurando um produto com id conhecido. Id: 4");
            var proId =
                  (from p in pros
                  where p.Id == 4
                  select p).First();
            Console.WriteLine($"Produto encontrado: {proId.Description} Id: {proId.Id}");

            Console.WriteLine("Lambda - Procurando um produto com id conhecido. Id: 4");
            var proIdo = pros.FirstOrDefault(m => m.Id == 4);
            Console.WriteLine($"Produto encontrado: {proIdo.Description} Id: {proIdo.Id}");
            
            Console.WriteLine();
            Console.WriteLine("Group by com n campos:");

            List<Children> children = new List<Children>() { 
                new Children { Age = 15, Friend = "Matheus", FavoriteColor = "Black", ID = 1, School = "Monteiro" },
                new Children {Age = 16, Friend = "Nicolas", FavoriteColor = "Red", ID = 2, School = "Lobato"},
                new Children {Age = 12, Friend = "Matheus",FavoriteColor = "Black",ID = 3, School = "Monteiro"}
            };           

            var consolidatedChildren = from c in children
                                       group c by new
                                       {
                                           c.School,
                                           c.Friend,
                                           c.FavoriteColor,
                                       } into gcs
                                       select new
                                       {
                                           School = gcs.Key.School,
                                           Friend = gcs.Key.Friend,
                                           FavoriteColor = gcs.Key.FavoriteColor,
                                           Age = gcs.Min(m => m.Age),
                                           _Children = gcs.ToList(),
                                           Qtd = gcs.Count()
                                        };
            foreach (var a in consolidatedChildren)
            {
                Console.WriteLine($"School: {a.School} - Menor idade:{a.Age} - Qtd:{a.Qtd}");
            }

            Console.Read();
                                                                        
        }
    }
    class Children
    {
        public int ID { get; set; }
        public string Friend { get; set; }
        public string School { get; set; }
        public string FavoriteColor { get; set; }
        public int Age { get; set; }
    }
}
