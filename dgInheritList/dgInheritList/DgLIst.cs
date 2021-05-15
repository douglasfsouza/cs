using System;
using System.Collections.Generic;
using System.Text;

namespace dgInheritList
{
    class DgList<T> : List<T> 
    {
        public void RemoveDuplicates()
        {
            this.Sort();
            for (int i = base.Count -1; i > 0; i--)
            {
                if (this[i].Equals(this[i - 1]))
                {
                    this.RemoveAt(i);
                }

            }
        }

    }
}
