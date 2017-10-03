using Bonavena.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bonavena.Configuration
{
    public class BonavenaConfig
    {
        private static BonavenaConfig _Context { get; set; }
        public static BonavenaConfig Context
        {
            get
            {
                if (_Context == null) _Context = new BonavenaConfig();
                return _Context;
            }
        }


        internal Dictionary<SQLType, Dictionary<string, string>> conStrings;
        internal string SPTemplate = "sp_{0}_{1}";

        private BonavenaConfig()
        {
            conStrings = new Dictionary<SQLType, Dictionary<string, string>>();
            foreach (SQLType item in Enum.GetValues(typeof(SQLType)))
            {
                conStrings.Add(item, new Dictionary<string, string>());
            }
        }

        public BonavenaConfig SetConnectionString(string conString,SQLType type,string name = "Default")
        {
            conStrings[type][name] = conString;
            return this;
        }

        public BonavenaConfig SetSPTemplate(string template = "sp_{0}_{1}")
        {
            SPTemplate = template;
            return this;
        }

    }
}
