using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspODataFramework.Models
{
    public interface ISimpleList
    {
        string Key { get; set; }
        string Description { get; set; }
    }
}
