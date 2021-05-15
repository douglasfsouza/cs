using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace dgTesting
{
    public static class CustomerTest
    {
        public static void GetAgeTest()
        {
            //Arrange
            DateTime dob = DateTime.Today.AddDays(7).AddYears(-44);
            
            //fará 44 em 7 dias
            int expectedAge = 43;

            //Act
            Customer customer = new Customer();
            customer.DateOfBirth = dob;
            int calculatedAge = customer.GetAge();

            //Assert

            //Assert.IsTrue(calculatedAge == expectedAge, "Problem in GetAge, incorrect calculated age");
            //or
            Assert.AreEqual(expectedAge, calculatedAge, "Problem in GetAge, incorrect calculated age");
            
            Console.WriteLine("Sucesso!!");
            return;
        }
    }
}
