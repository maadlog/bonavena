using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Bonavena.Database
{
    public interface IDataBase
    {
        bool ExecuteScriptNonQuery(string script, string @base = "");
        int ExecuteSPNonQuery(string storeProcedureName, Action<DbParameterCollection> fillParameters = null);
        string ExecuteSPScalar(string storeProcedureName, Action<DbParameterCollection> fillParameters = null);
        void ExecuteSPWithResultSet(string storeProcedureName, Action<DbDataReader> action, Action<DbParameterCollection> fillParameters = null);
    }
}