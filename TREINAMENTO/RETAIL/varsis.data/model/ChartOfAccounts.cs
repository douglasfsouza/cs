﻿using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model
{
  public  class ChartOfAccounts : EntityBase
    {
        public override string EntityName => "ChartOfAccounts";
        public string Code { get; set; }
        public string Name { get; set; }
    }
}


