using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace dgIEnumerable
{
    class Contador : IEnumerable
    {
        public IEnumerable Contar()
        {
            int i = 0;
            while (i++ < 10)
            {                
                yield return i;
            }          
        }       
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
         

    }
}
