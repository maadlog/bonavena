using Bonavena.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bonavena.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string TableName { get; set; }
        public SQLType Motor { get; set; }
    }
}
