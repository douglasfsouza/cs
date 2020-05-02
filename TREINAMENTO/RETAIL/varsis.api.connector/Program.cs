using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Varsis.Api.Connector
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Core.Program.StartupType = typeof(Startup);
            Core.Program.Main(args);
        }
    }
}
