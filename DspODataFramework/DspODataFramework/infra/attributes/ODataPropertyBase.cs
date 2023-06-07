using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspODataFramework.infra.attributes
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ODataPropertyBase : Attribute
    {
        private bool _IsCustom = false;
        private bool _IsCustomSetted = false;

        public string Label { get; set; }

        public string PhysicalName { get; set; }

        public bool Mandatory { get; set; }

        public string linkedTable { get; set; }

        public string linkedUDO { get; set; }

        public string linkedSystemObject { get; set; }

        public string defaultValue { get; set; }

        public string Tooltip { get; set; }

        public bool IsCustom
        {
            get
            {
                return _IsCustom;
            }
            set
            {
                _IsCustom = value;
                _IsCustomSetted = true;
            }
        }

        public bool IsCustomSetted { get => _IsCustomSetted; }

        public bool OnlyDisplay { get; set; }

        public bool IgnoreOnBackend { get; set; }

       
    }
}
