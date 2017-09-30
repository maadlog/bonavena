using System;
using System.Collections.Generic;

using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Transactions;
using Bonavena.Database;
using Bonavena.Helpers;
using Bonavena.Enumerators;
using Bonavena.Attributes;

namespace Bonavena.Dao
{
    public class GenericDAO<T> where T : class, new()
    {
        public static IDataBase DataBase { get; set; }
        public static EntityMapper Mapper { get; set; }

        static GenericDAO()
        {
            var TableAtt = typeof(T).GetTypeInfo().GetCustomAttribute<TableAttribute>();

            if (TableAtt == null) throw new Exception("La clase " + typeof(T).Name + " debe tener el atributo Table para resolver la comunicación con DB");

            Mapper = new EntityMapper();


            DataBase = ResolveDatabase(TableAtt.Motor);

           
        }

        private static IDataBase ResolveDatabase(SQLType motor)
        {
            string NameConnectionString = GetNameConnectionString(motor);

            switch (motor)
            {
                case SQLType.SqlServer:
                    return new SQLServerDatabase(NameConnectionString); ;
                case SQLType.Oracle:
                    return new OracleDatabase(NameConnectionString); ;
                case SQLType.MySql:
                    break;
                case SQLType.PostgreSQL:
                    break;
                default:
                    break;
            }
            
            throw new NotImplementedException();
        }

        /// <summary>
        /// Persiste una entidad en base de datos. Puede crear o actualizar la misma.
        /// </summary>
        /// <param name="entity">Cualquier entidad definida en el mapping.</param>
        /// <returns>Identificador de la entidad persistida.</returns>
        public static int Save(T entity)
        {
            //st.Start();

            var spName = GetSpName(IsUpdate(entity) ? "spNameUpdate" : "spNameInsert");

            int idEntity = 0;

            if (HasChilds())
            {

                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    idEntity = DataBase.ExecuteSPNonQuery(spName, (p) => FillParameters(p, entity));

                    //Asigno a la clase el id generado
                    if (!IsUpdate(entity))
                        SetPropertyValue(Mapper.GetPropertyDBKey(entity.GetType())[0], idEntity, entity);

                    UpdateRelation(entity);

                    scope.Complete();
                }
            }
            else
            {
                idEntity = DataBase.ExecuteSPNonQuery(spName, (p) => FillParameters(p, entity));
            }

            return idEntity;
        }

        public static void Delete(int id)
        {
            if (!typeof(T).GetProperties().Any(p => p.Name == "Id" && p.CanWrite))
            {
                throw new Exception("La entidad no tiene definida una propiedad con el nombre 'Id' o es de solo lectura.");
            }

            var entity = new T();
            typeof(T).GetProperty("Id").SetValue(entity, id);

            Delete(entity);
        }

        public static void Delete(T entity)
        {
            DataBase.ExecuteSPNonQuery(GetSpName("spNameDelete"), p => FillParameters(p, entity));
        }

        public static List<T> Find(T filterEntity)
        {
            List<T> lista = null;

            DataBase.ExecuteSPWithResultSet(GetSpName("spNameSelect"), r =>
            {
                lista = (List<T>)BuildList(typeof(T), r);
            }, p => FillParameters(p, filterEntity));

            return lista;
        }

        public static List<T> GetAll()
        {
            return Find((T)null);
        }

        public static string CallFunction(string fnName, T entity)
        {
            string result = string.Empty;

            result = DataBase.ExecuteSPScalar(fnName, (p) => FillParameters(p, entity));

            return result;
        }

        private static string GetNameConnectionString(SQLType type)
        {
            string connection = string.Empty;
            switch (type)
            {
                case SQLType.SqlServer:
                    connection = "ConnectionString";
                    break;
                case SQLType.Oracle:
                    connection = "ConnectionStringOracle";
                    break;
                case SQLType.MySql:
                    break;
                case SQLType.PostgreSQL:
                    break;
                default:
                    break;
            }
            return connection;
        }

        private static void UpdateRelation(T entity)
        {
            foreach (var prop in GetPropertiesList(entity))
            {
                var clasePropiedad = prop.PropertyType.GenericTypeArguments[0].Name;
                var nameRelation = entity.GetType().Name + clasePropiedad;

                var idParent = GetPropertyValue(Mapper.GetPropertyDBKey(entity.GetType())[0], entity);
                //Considerando entidades de una sola clave

                var TableAtt = typeof(T).GetTypeInfo().GetCustomAttribute<TableAttribute>();

                DataBase.ExecuteSPNonQuery(GetSpName("spNameDelete", TableAtt.PrefixSp, nameRelation), p => FillParametersRelation(p, idParent));

                var lista = (IList)prop.GetValue(entity, null);

                if ((lista != null))
                {
                    foreach (object item in lista)
                    {
                        var idChild = GetPropertyValue(Mapper.GetPropertyDBKey(item.GetType())[0], item);
                        //Considerando entidades de una sola clave
                        DataBase.ExecuteSPNonQuery(GetSpName("spNameInsert", TableAtt.PrefixSp, nameRelation), p => FillParametersRelation(p, idParent, idChild));
                    }
                }
            }
        }

        private static bool HasChilds()
        {
            var res = typeof(T).GetProperties()
                   .Any(p => typeof(IList).IsAssignableFrom(p.PropertyType) && p.PropertyType.IsGenericType);
            return res;
        }

        private static bool IsUpdate(T entity)
        {
            Type tipo = entity.GetType();
            string[] camposClave = Mapper.GetPropertyDBKey(tipo);
            string[] camposClaveFk = Mapper.GetPropertyDBKeyAndKeyChild(tipo);

            object valor = null;
            object valorFk = null;
            bool Return = true;
            if (camposClaveFk.Length > 0)
            {
                for (int s = 0; s < camposClaveFk.Length; s++)
                {
                    valor = GetPropertyValue(camposClave[s], entity);
                    if ((valor == null) || string.IsNullOrEmpty(valor.ToString()) || valor.ToString() == "0")
                    {
                        //for (int i = 0; i <= camposClaveFk.Length - 1; i++)
                        //{
                        valorFk = GetPropertyValue(camposClaveFk[s], entity);
                        if ((valorFk != null) && (string.IsNullOrEmpty(valorFk.ToString()) || valorFk.ToString() != "0"))
                        {
                            //for (int r = 0; r <= camposClave.Length - 1; r++)
                            //{
                            valor = GetPropertyValue(camposClave[s], entity);
                            if ((valor == null) || string.IsNullOrEmpty(valor.ToString()) || valor.ToString() == "0")
                            {
                                SetPropertyValue(camposClave[s], valorFk, entity);
                                Return = false;
                            }
                            //}

                        }
                        //}
                    }
                }
            }
            for (int l = 0; l <= camposClave.Length - 1; l++)
            {
                valor = GetPropertyValue(camposClave[l], entity);
                if ((valor == null) || string.IsNullOrEmpty(valor.ToString()) || valor.ToString() == "0")
                {
                    Return = false;
                }
            }

            return Return;
        }

        private static List<PropertyInfo> GetPropertiesList(object entity)
        {
            var res = entity.GetType().GetProperties()
                   .Where(p => typeof(IList).IsAssignableFrom(p.PropertyType) && p.PropertyType.IsGenericType).ToList();
            return res;
        }

        private static void FillParameters(DbParameterCollection parametros, object entity)
        {
            if (entity == null) { return; }

            foreach (DbParameter parametro in parametros)
            {
                if (parametro.Direction == ParameterDirection.ReturnValue) continue;

                string prop = Mapper.GetPropertyName(parametro, entity.GetType());
                if (string.IsNullOrWhiteSpace(prop)) continue;

                object valor = GetPropertyValue(prop, entity);
                if ((valor == null) || string.IsNullOrWhiteSpace(valor.ToString()))
                {
                    parametro.Value = DBNull.Value;
                }
                else
                {
                    parametro.Value = valor;
                }
            }
        }

        private static void FillParametersRelation(DbParameterCollection parametros, object idParent, object idChild = null, object relationEntity = null)
        {
            parametros[1].Value = idParent;
            parametros[2].Value = idChild ?? DBNull.Value;

            //IMPLEMENTAR SI ES NECESARIO QUE UNA ENTIDAD DE RELACION ADEMAS CONTENGA ATRIBUTOS PROPIOS, 
            //EN ESE CASO HAY QUE AGREGAR LA RELACION EN EL MAPPING COMO ENTIDAD.
            //if (relationEntity != null)
            //{
            //    for (var i = 0; i <= parametros.Count - 1; i++)
            //    {
            //
            //    }
            //}
        }

        //Funcion recursiva para obtener el valor de una propiedad de un objeto. Si la propiedad es otro objeto, se vuelve 
        //a llamar a si misma para obtener la propiedad del objeto hijo
        private static object GetPropertyValue(string propiedad, object entity)
        {
            if (entity == null) return null;
            Type tipo = entity.GetType();
            string[] objeto = propiedad.Split('.');

            if (objeto.Length == 2)
            {
                var res = GetPropertyValue(objeto[1], tipo.GetProperty(objeto[0].ToString()).GetValue(entity));
                return res;
            }
            else if (objeto.Length == 1)
            {
                var res = tipo.GetProperty(propiedad).GetValue(entity);
                //Debug.WriteLine("End: GetPropertyValue " + propiedad + " - " + st.ElapsedMilliseconds);
                return res;
            }
            else
            {
                throw new Exception("Se quiso obtener el valor de una propiedad formada por mas de un objeto.");
            }
        }

        private static void SetPropertyValue(string propiedad, object valor, object entity)
        {
            //Debug.WriteLine("Start: SetPropertyValue " + propiedad + " - " + st.ElapsedMilliseconds);
            Type tipo = entity.GetType();
            string[] objeto = propiedad.Split('.');

            switch (objeto.Length)
            {
                case 0:
                    throw new Exception("Se quiso establer el valor de una propiedad sin nombre");
                case 1:
                    if (!DBNull.Value.Equals(valor))
                    {
                        tipo.GetProperty(propiedad).SetValue(entity, valor);
                    }
                    //Debug.WriteLine("End: SetPropertyValue " + propiedad + " - " + st.ElapsedMilliseconds);
                    return;
                case 2:
                    Type tipoProp = tipo.GetProperty(objeto[0]).PropertyType;

                    object instancia = tipo.GetProperty(objeto[0]).GetValue(entity);

                    if (instancia == null)
                    {
                        instancia = Activator.CreateInstance(tipoProp);
                    }

                    tipo.GetProperty(objeto[0]).SetValue(entity, instancia);
                    if (!DBNull.Value.Equals(valor))
                    {
                        tipoProp.GetProperty(objeto[1]).SetValue(instancia, valor);
                    }
                    //Debug.WriteLine("End: SetPropertyValue " + propiedad + " - " + st.ElapsedMilliseconds);
                    return;
                default:
                    Type tipoProp2 = tipo.GetProperty(objeto[0]).PropertyType;

                    object instancia2 = tipo.GetProperty(objeto[0]).GetValue(entity);

                    if (instancia2 == null)
                    {
                        instancia2 = Activator.CreateInstance(tipoProp2);
                    }

                    tipo.GetProperty(objeto[0]).SetValue(entity, instancia2);

                    if (!DBNull.Value.Equals(valor))
                    {
                        string prop = objeto.Skip(1).Aggregate((x, y) => x + "." + y);
                        SetPropertyValue(prop, valor, instancia2);
                    }
                    //Debug.WriteLine("End: SetPropertyValue " + propiedad + " - " + st.ElapsedMilliseconds);
                    return;
            }

            //if (objeto.Length == 2)
            //{
            //    Type tipoProp = tipo.GetProperty(objeto[0]).PropertyType;

            //    object instancia = tipo.GetProperty(objeto[0]).GetValue(entity);

            //    if (instancia == null)
            //    {
            //        instancia = Activator.CreateInstance(tipoProp);
            //    }

            //    tipo.GetProperty(objeto[0]).SetValue(entity, instancia);

            //    if (!DBNull.Value.Equals(valor))
            //    {
            //        tipoProp.GetProperty(objeto[1]).SetValue(instancia, valor);
            //    }

            //}
            //else if (objeto.Length == 1)
            //{
            //    if (!DBNull.Value.Equals(valor))
            //    {
            //        tipo.GetProperty(propiedad).SetValue(entity, valor);
            //    }
            //}
            //else
            //{
            //    throw new Exception("Se quiso establer el valor de una propiedad formada por mas de un objeto.");
            //}

        }

        private static void FillChilds(object entity)
        {
            //Determino por reflection si tiene propiedades de tipo List y genero una coleccion en base al tipo de la coleccion, 
            //busco el SP correspondiente a la relacion y genero las entidades de la coleccion tipada para luego asignarla a la propiedad.
            //Por ultimo, por cada entidad creada vuelva a llamar recursivamente a FillChilds hasta terminar con todos los nodos hijos de la 
            //entidad creada.
            //Debug.WriteLine("Start: FillChilds - " + st.ElapsedMilliseconds);
            foreach (var prop in GetPropertiesList(entity))
            {
                var childType = prop.PropertyType.GenericTypeArguments[0].GetTypeInfo();
                var nameRelation = entity.GetType().Name + childType.Name;

                var idParent = GetPropertyValue(Mapper.GetPropertyDBKey(entity.GetType())[0], entity);
                //Considerando entidades de una sola clave
                var TableAtt = typeof(T).GetTypeInfo().GetCustomAttribute<TableAttribute>();

                IList lista = null;

                DataBase.ExecuteSPWithResultSet(GetSpName("spNameSelect", TableAtt.PrefixSp, nameRelation), r =>
                {
                    lista = BuildList(childType, r);

                }, p => FillParametersRelation(p, idParent));

                if ((lista != null) && lista.Count > 0)
                {
                    SetPropertyValue(prop.Name, lista, entity);
                }
            }
            //Debug.WriteLine("End: FillChilds - " + st.ElapsedMilliseconds);
        }

        private static IList BuildList(TypeInfo tipo, DbDataReader reader)
        {
            //Debug.WriteLine("Start: BuildList - " + st.ElapsedMilliseconds);
            IList lista = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { tipo.BaseType }));

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var instancia = Activator.CreateInstance(tipo.BaseType);
                    Fill(instancia, reader);
                    FillChilds(instancia);
                    lista.Add(instancia);
                }
            }
            //Debug.WriteLine("End: BuildList - " + st.ElapsedMilliseconds);
            return lista;
        }

        private static void Fill(object entity, IDataReader reader)
        {
            //Debug.WriteLine("Start: Fill - " + st.ElapsedMilliseconds);
            for (var i = 0; i <= reader.FieldCount - 1; i++)
            {
                string propiedad = Mapper.GetPropertyName(reader.GetName(i), entity.GetType());
                if (!string.IsNullOrEmpty(propiedad))
                {
                    SetPropertyValue(propiedad, reader[i], entity);
                }
            }
            //Debug.WriteLine("End: Fill - " + st.ElapsedMilliseconds);
        }

        private static string GetSpName(string nameOperation)
        {
            //Debug.WriteLine("Start: GetSpName(1) - " + st.ElapsedMilliseconds);
            var TableAtt = typeof(T).GetTypeInfo().GetCustomAttribute<TableAttribute>();

            var prefix = TableAtt.Satellite != SatelliteType.None ? TableAtt.Satellite.GetPrefix() : TableAtt.PrefixSp;

            var res = GetSpName(nameOperation, prefix, typeof(T).Name);
            //Debug.WriteLine("End: GetSpName(1) - " + st.ElapsedMilliseconds);
            return res;
        }

        private static string GetSpName(string nameOperation, string prefix, string entityName)
        {
            //Debug.WriteLine("Start: GetSpName(3) - " + st.ElapsedMilliseconds);
            var res = string.Format(ConfigurationManager.AppSettings[nameOperation], prefix, entityName);
            //Debug.WriteLine("End: GetSpName(3) - " + st.ElapsedMilliseconds);
            return res;
        }

    }
}
