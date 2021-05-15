using System;
using System.Collections.Generic;
using System.Text;

namespace dgValidateSetProperty
{
    class Validar
    {
        private DateTime _d;
        public DateTime data
        {
            get
            {
                return _d;
            }
            set
            {
                DateTime d;
                if (DateTime.TryParse(value.ToString(),out d))
                {
                    if (value > DateTime.Now)
                    {
                        throw new ArgumentOutOfRangeException("Data", "Data futura");
                    }

                }
                else
                {
                    throw new ArgumentException("Data", "Data invalida");
                }
                _d = d;
            }
        }
    }
}
