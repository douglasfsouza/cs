using System;

namespace dgCustomException
{
    class Program
    {
        static void Main(string[] args)
        {
            BankImport bank = new BankImport();
            try
            {
                bank.Import(237);
            }            
            catch(BankNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("General Error. Error: {0}",e.Message);                
            }
            
        }
    }
}
