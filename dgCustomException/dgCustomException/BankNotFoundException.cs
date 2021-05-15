using System;
using System.Collections.Generic;
using System.Text;

namespace dgCustomException
{
    public class BankNotFoundException : Exception
    {
        public BankNotFoundException()
        {            

        }
        public BankNotFoundException(string message) : base(message)
        {

        }
        public BankNotFoundException(string message, Exception inner) : base(message, inner)
        {

        }

    }
}
