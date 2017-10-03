using Bonavena.Configuration;
using Bonavena.Enumerators;
using Bonavena.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace Bonavena.Database
{
    public class SQLServerDatabase : IDataBase
    {
        private string _connection;
        public SQLServerDatabase(string connection)
        {
            _connection = connection;
        }
        private SqlConnection GetConnection()
        {
            var connectionString = ConnectionStringCreator.SecureCreateSQL(BonavenaConfig.Context.conStrings[SQLType.SqlServer]["Default"]);

            return new SqlConnection(connectionString);
        }

        public void ExecuteSPWithResultSet(string storeProcedureName, Action<DbDataReader> action, Action<DbParameterCollection> fillParameters = null)
        {
            var conn = GetConnection();

            if (conn.State != ConnectionState.Open) conn.Open();
            using (var cmd = conn.CreateCommand())
            {

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storeProcedureName;

                SqlCommandBuilder.DeriveParameters(cmd);

                if (fillParameters != null)
                    fillParameters(cmd.Parameters);

                using (var reader = cmd.ExecuteReader())
                {
                    action(reader);
                }
            }
            conn.Close();
            conn.Dispose();
            // stopWatch.Stop();
            // Debug.WriteLine("Data Base ExecuteSPWithResultSet - " + stopWatch.ElapsedMilliseconds);
            // stopWatch.Reset();

        }

        private void ExecuteArithAbort(DbConnection con)
        {
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SET ARITHABORT ON";
                cmd.ExecuteNonQuery();
            }

        }

        public int ExecuteSPNonQuery(string storeProcedureName, Action<DbParameterCollection> fillParameters = null)
        {
            var conn = GetConnection();

            if (conn.State != ConnectionState.Open) conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storeProcedureName;

                SqlCommandBuilder.DeriveParameters(cmd);

                if (fillParameters != null)
                    fillParameters(cmd.Parameters);

                var ouputParameter = cmd.Parameters["@RETURN_VALUE"];

                cmd.ExecuteNonQuery();

                conn.Dispose();
                if ((int)ouputParameter.Value == -1)
                {
                    throw new Exception(string.Format("Error al ejecutarse el SP: {0}", storeProcedureName));
                }
                else
                {
                    return (int)ouputParameter.Value;
                }
            }

        }

        public bool ExecuteScriptNonQuery(string script, string @base = "")
        {
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

        }

        public string ExecuteSPScalar(string storeProcedureName, Action<DbParameterCollection> fillParameters = null)
        {
            var conn = GetConnection();

            if (conn.State != ConnectionState.Open) conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = storeProcedureName;

                SqlCommandBuilder.DeriveParameters(cmd);
                cmd.Parameters.Add(fillParameters);
                if (fillParameters != null)
                    fillParameters(cmd.Parameters);


                //cmd.Parameters.Add("@fijo", fillParameters.Method.GetParameters("fijo").ToString());
                //cmd.Parameters.Add("@contrato", "000000000028");
                //cmd.Parameters.Add("@persona", "4957");

                cmd.CommandText = "select " + storeProcedureName + "(@fijo,@contrato,@persona)";

                var res = cmd.ExecuteScalar().ToString();

                conn.Dispose();
                return res.TrimEnd();
            }

        }

        public DbTransaction BeginTransaction()
        {
            return GetConnection().BeginTransaction();
        }
    }
}
