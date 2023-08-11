using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DgsGofVisitor
{
    public interface IVisitor
    {
        public void Accept(Carro car);
    }
}
