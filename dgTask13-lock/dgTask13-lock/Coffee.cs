using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace dgTask13_lock
{
    class Coffee
    {
        private object coffeeLock = new object();
        private int stock;
        public Coffee(int initialStock)
        {
            stock = initialStock;
        }
        public bool MakeCoffees(int required)
        {
            lock (coffeeLock)
            {
                if (required <= 0)
                {
                    throw new ArgumentException("Invalid required!");
                }
                if (stock >= required)
                {
                    Console.WriteLine($"Stock before = {stock}");
                    stock -= required;
                    string xcoffee = required == 1 ? "coffee" : "coffees";
                    Console.WriteLine($"Made {required} {xcoffee}");
                    Console.WriteLine($"Stock after = {stock}");
                    Thread.Sleep(500);
                    return true;
                }
                else
                {
                    Console.WriteLine($"Insufficient stock = {stock} to make {required}");
                    return false;
                }
            }
        }
    }
}
