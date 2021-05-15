using System;
using System.Collections.Generic;
using System.Text;

namespace dgTesting
{
    public class Customer
    {
        public DateTime DateOfBirth { get; set; }

        public int GetAge()
        {
            TimeSpan difference = DateTime.Today.Subtract(DateOfBirth);
            int ageInYears = (int)(difference.Days / 365.25);
            return ageInYears;
        }
    }
}
