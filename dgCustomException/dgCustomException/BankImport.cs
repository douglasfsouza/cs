using System;
using System.Collections.Generic;
using System.Text;

namespace dgCustomException
{
    class BankImport
    {
        public void Import(int bankCode)
        {
            if (bankCode != 341)
            {
                throw new BankNotFoundException("Bank not found");
            }
            Console.WriteLine("Bank imported sucessfully!!");

        }
    }
}
