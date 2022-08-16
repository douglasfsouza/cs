using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp6
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpCookie c = new HttpCookie("Console");
            c.Value = "Douglas";
            c.Expires = DateTime.Now.AddMinutes(120);
            HttpRequest dd = new HttpRequest("x","http://localhost",string.Empty);
            dd.Cookies.Add(c);

            HttpCookie ler = dd.Cookies.Get("Console");
            Console.WriteLine(ler.Value);
            Console.ReadLine();

        }
    }
}
