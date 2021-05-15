using System;
using System.Collections.Generic;
using System.Text;

namespace dgInterfaces
{
    class TV : ITV,IVideo
    {
        public int ActualChannel { get; set; }
        public string Size { get; set; }
        public string Manufacturer { get; set; }
        void ITV.ChangeChannel(int channel)
        {
            ActualChannel = channel;
        }
    }
}
