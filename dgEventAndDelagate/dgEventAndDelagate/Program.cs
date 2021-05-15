using System;

namespace dgEventAndDelagate
{
    class Program
    {
        static void Main(string[] args)
        {
            Coffee coffee1 = new Coffee();
            //coffee1.OutOfBeans += hand
            coffee1.MakeCoffee();

            Inventory i = new Inventory();
            


            
        }
    }
    public struct Coffee
    {
        public EventArgs e;
        public delegate void OutOfBeansHandler(Coffee coffee, EventArgs args);
        public event OutOfBeansHandler OutOfBeans;

        int currentStockLevel;
        int mininumStockLevel;

        public string Bean { get; set; }

        public void MakeCoffee()
        {
            currentStockLevel--;
            if (currentStockLevel < mininumStockLevel)
            {
                if (OutOfBeans != null)
                {
                    OutOfBeans(this, e);
                }
            }
        }
        

        
    }
    public class Inventory
    {
        public void HandleOutOfBeans(Coffee sender, EventArgs args)
        {
            string coffeeBean = sender.Bean;

        }
        public void SubscribeToEvent()
        {
            //coffee1.OutOfBeans += HandleOutOfBeans;
        }
        public void UnsubscribeToEvent()
        {
            //coffee1.OutOfBeans -= HandleOutOfBeans;
        }
    }
}
