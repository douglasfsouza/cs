using System;
using System.Collections.Generic;
using System.Text;

namespace dgInterfaces
{
    interface ITV
    {
        public int ActualChannel { get; set; }
        public string Size { get; set; }
        public string  Manufacturer { get; set; }
        void ChangeChannel(int channel);
    }
}
