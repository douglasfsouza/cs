using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace dgTask12_2_Unobserved
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskScheduler.UnobservedTaskException += (object sender, UnobservedTaskExceptionEventArgs e) =>
            {
                foreach (Exception ex in ((AggregateException)e.Exception).InnerExceptions)
                {
                    Console.WriteLine(string.Format("An exception: {0}", ex.Message));
                }
                e.SetObserved();
            };
            Task.Run(() =>
            {
                using (WebClient client = new WebClient())
                {
                    Uri uri = new Uri("httpx://fourthcoffee/bogus");
                    client.DownloadString(uri);
                }
            });
            Thread.Sleep(5000);
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Console.WriteLine("Execution completed");
        }
    }
}
