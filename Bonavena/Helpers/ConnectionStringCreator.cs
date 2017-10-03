using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Bonavena.Helpers
{
    public class ConnectionStringCreator
    {
        public static string SecureCreateSQL(string Connection)
        {
            return ValidateSQLServer(Connection);
        }

        public static string SecureCreateOracle(string Connection)
        {
            return ValidateOracle(Connection);
        }
       
        private static string ValidateSQLServer(string Connection)
        {
            Match resultado = Regex.Match(Connection, "^((Integrated Security=\\w*;|Password=\\w*;|Persist Security Info=(True|False);|User ID=\\w*;|Initial Catalog=\\w*;|Data Source=(\\w*|\\w*\\\\\\w*);|Application Name=(\\{0\\}|USR_MIDDLEWARE);|MultipleActiveResultSets=(True|False)(;*)))+$");
            if (resultado.Success)
                return Connection;
            else
                throw new Exception("La conexión es una cadena no segura");
        }

        private static string ValidateOracle(string Connection)
        {
            Match resultado = Regex.Match(Connection, "^SERVER=\\(DESCRIPTION=\\(ADDRESS=\\(PROTOCOL=TCP\\)\\(HOST=\\d+\\.\\d+\\.\\d+\\.\\d+\\)\\(PORT=\\d+\\)\\)\\(CONNECT_DATA=\\(SID=\\w+\\)\\)\\);uid=\\w+;pwd=\\w+;$");
            if (resultado.Success)
                return Connection;
            else
                throw new Exception("La conexión es una cadena no segura");
        }
    }
}
