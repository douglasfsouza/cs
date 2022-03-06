using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace dgs.Store.Domain.Helpers
{
    public static class StringHelpers
    {
        public static string Encrypt(this string senha)
        {
            var salt = "9c2df0e9-36ea-43e5-9f60-a9a0bde408f6";
            //transformar em array de bytes
            var passwd = Encoding.UTF8.GetBytes( senha + salt);

            using (var sha = SHA512.Create())
            {
                passwd = sha.ComputeHash(passwd);
            }
            return Convert.ToBase64String(passwd);
                
        }
        
    }
}
