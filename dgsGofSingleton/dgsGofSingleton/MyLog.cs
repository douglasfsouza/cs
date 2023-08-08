using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgsGofSingleton
{
    
    public class MyLog
    {
        static private MyLog instancia = new MyLog();
        private MyLog()
        {                
        }
        
        public void GravarLog(string message)
        {
            Console.WriteLine(message);
        }

        public static MyLog GetInstance()
        {
            return instancia;
        }
    }
}
