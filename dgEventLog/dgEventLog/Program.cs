using System;
using System.Diagnostics;
using System.Security;

namespace dgEventLog
{
    class Program
    {
        static void Main(string[] args)
        {
            string eventLog = "DgEventLog";
            string eventSource = "DgEventSource";
            try
            {
                if (!EventLog.SourceExists(eventSource))
                {
                    EventLog.CreateEventSource(eventSource, eventLog);
                }
                EventLog.WriteEntry(eventSource, "Doug writing the log in Windows Event Log");
                Console.WriteLine("Log wrote!");
            }
            catch (SecurityException e)
            {
                Console.WriteLine("Use Administrator in Visual Studio to write logs");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);               
            }
            
            
        }
    }
}
