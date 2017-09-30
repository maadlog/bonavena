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
            return String.Format(ValidateSQLServer(Connection), ValidUser() + "|" + ValidIP());
        }

        public static string SecureCreateOracle(string Connection)
        {
            return ValidateOracle(Connection);
        }

        private static string ValidUser()
        {
            var User = HttpContext.Current.User.Identity.Name;

            if (String.IsNullOrEmpty(User)) return String.Empty;

            Match resultado = Regex.Match(User, "^(-|\\w)*(\\\\)*\\w+$");
            if (resultado.Success)
                return User;
            else
                throw new Exception("El usuario obtenido del contexto es una cadena no segura");
        }

        private static string ValidIP()
        {
            var IP = HttpContext.Current.Request.Headers["IP"];

            if (String.IsNullOrEmpty(IP)) return String.Empty;

            if (IP == "::1"/*IP LOCAL*/) return IP;

            Match resultado = Regex.Match(IP, "^\\d+\\.\\d+\\.\\d+\\.\\d+$");
            if (resultado.Success)
                return IP;
            else
                throw new Exception("La conexión es una cadena no segura");
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
