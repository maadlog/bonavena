using Bonavena.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Bonavena.Database
{
    public class OracleDatabase : IDataBase
    {
        private readonly string _connection;
        public OracleDatabase(string connection)
        {
            _connection = connection;
        }
        /*
        private OracleConnection GetConnection()
        {
            //var parameters = System.Web.HttpContext.Current != null ? System.Web.HttpContext.Current.Request.Headers["IP"] : string.Empty;
            //var name = System.Web.HttpContext.Current != null ? System.Web.HttpContext.Current.User.Identity.Name.ToString() : string.Empty;

            var connectionString = ConnectionStringCreator.SecureCreateOracle(ConfigurationManager.ConnectionStrings[_connection].ConnectionString);

            return new OracleConnection(connectionString);
        }
        */
        public void ExecuteSPWithResultSet(string storeProcedureName, Action<DbDataReader> action, Action<DbParameterCollection> fillParameters = null)
        {
            throw new NotImplementedException();
            /*
            var conn = GetConnection();

            if (conn.State != ConnectionState.Open) conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storeProcedureName;

                OracleCommandBuilder.DeriveParameters(cmd);

                if (fillParameters != null)
                    fillParameters(cmd.Parameters);

                using (var reader = cmd.ExecuteReader())
                {
                    action(reader);
                }
            }
            conn.Close();
            conn.Dispose();
            */
        }

        public int ExecuteSPNonQuery(string storeProcedureName, Action<DbParameterCollection> fillParameters = null)
        {
            throw new NotImplementedException();
            /*
            var conn = GetConnection();

            if (conn.State != ConnectionState.Open) conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storeProcedureName;

                OracleCommandBuilder.DeriveParameters(cmd);

                if (fillParameters != null)
                    fillParameters(cmd.Parameters);

                var ouputParameter = cmd.Parameters["RETURN_VALUE"];

                cmd.ExecuteNonQuery();

                conn.Dispose();


                if ((decimal)ouputParameter.Value == -1)
                {
                    throw new Exception(string.Format("Error al ejecutarse el SP: {0}", storeProcedureName));
                }
                else
                {

                    return int.Parse(ouputParameter.Value.ToString());
                }
            }
            */
        }

        public bool ExecuteScriptNonQuery(string script, string @base = "")
        {
            throw new NotImplementedException();
            /*
            var conn = GetConnection();

            if (conn.State != ConnectionState.Open) conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                if (!string.IsNullOrEmpty(@base))
                {
                    cmd.Connection.ChangeDatabase(@base);
                }

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = script;

                cmd.ExecuteNonQuery();

                conn.Dispose();

                return true;
            }
            */
        }

        public string ExecuteSPScalar(string storeProcedureName, Action<DbParameterCollection> fillParameters = null)
        {
            throw new NotImplementedException();
            /*
            var conn = GetConnection();

            if (conn.State != ConnectionState.Open) conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storeProcedureName;

                OracleCommandBuilder.DeriveParameters(cmd);

                if (fillParameters != null)
                    fillParameters(cmd.Parameters);

                var res = cmd.ExecuteScalar().ToString();

                conn.Dispose();

                return res;
            }
            */
        }

        public DbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
            /*
            return GetConnection().BeginTransaction();
            */
        }
    }
}
