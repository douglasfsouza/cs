using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DgStringBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder s = new StringBuilder();
            s.Append("Using StringBuilder to improve performance");
            Console.WriteLine(s);

            StringBuilder st = new StringBuilder("The total is: ");
            int tot = 25;
            st.AppendFormat("{0:C}", tot);
            Console.WriteLine(st);

            StringBuilder sta = new StringBuilder("1234");
            sta.Insert(2,"A");
            Console.WriteLine(sta);

            sta.Remove(3, 2);
            Console.WriteLine(sta);

            sta.Append("TQ");
            Console.WriteLine(sta);

            //Console.ReadKey();

            bool flag = true;
            string[] spelling = { "receive", "receeve", "recive" };
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Which of this is {0}:", flag);
            sb.AppendLine();
            for (int i = 0; i <= spelling.GetUpperBound(0); i++)
            {
                sb.Append($" - {spelling[i]}");
                sb.AppendLine();
            }
            Console.WriteLine(sb.ToString());

            StringBuilder cap = new StringBuilder("123", 5);
            cap.Capacity = 5;
            cap.Length = 5;
            cap.Insert(3, "xxxxxxxx");
            cap.AppendLine();
            cap.Append("12345");
            cap.AppendLine();
            cap.Append("123456");

            cap.AppendLine();
            cap.Append("1234567");

            cap.AppendLine();
            cap.Append("12345678");

            cap.AppendLine();
            cap.Append("123456789");

            cap.AppendLine();
            cap.Append("1234567890");

            Console.WriteLine(cap);
            Console.ReadKey();
        }
    }
}
