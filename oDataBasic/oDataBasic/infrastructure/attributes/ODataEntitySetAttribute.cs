﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace oDataBasic.infrastructure.attributes
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ODataEntitySet : Attribute
    {
        readonly string entitySetName;
        public ODataEntitySet(string EntitySetName)
        {
            this.entitySetName = EntitySetName;
        }
        public string EntitySetName
        {
            get { return entitySetName; }
        }
        public Type EntityType { get; set; }
    }
}