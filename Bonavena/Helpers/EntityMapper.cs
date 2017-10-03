using Bonavena.Attributes;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Bonavena.Helpers
{
    public class EntityMapper
    {
        private const string MensajeEntidadNoEncontrada = "No se encontro el atributo Table en la entidad '{0}'";
        private const string MensajePropiedadDuplicada = "La propiedad '{0}' de la entidad '{1}' esta presente en mas de un atributo.";
        private const string MensajeAtributoDuplicado = "El atributo '{0}' de la entidad '{1}' esta presente en mas de un atributo.";

        public string GetPropertyName(DbParameter parametro, Type TypeEntidad)
        {
            string campo = parametro.ParameterName.ToString().Replace("@", "");
            return GetPropertyName(campo, TypeEntidad);
        }

        public string GetPropertyName(string campo, Type TypeEntidad)
        {
            var split = campo.Split('.');

            if (split.Length == 1)
            {
                var Props = TypeEntidad.GetProperties().SelectMany(propertyInfo => propertyInfo.GetCustomAttributes<NameEntityAttribute>())
                .Where(Att =>
                {
                    return (Att != null && Att.ColumnName == campo);
                });

                if (!Props.Any()) return string.Empty;

                if (Props.Count() > 1)
                    throw new Exception(string.Format(MensajeAtributoDuplicado, campo, TypeEntidad.Name));

                return Props.ElementAt(0).PropertyName.Trim();
            }
            else
            {
                var typePropiedad = TypeEntidad.GetProperty(split[0]).PropertyType;
                string prop = split.Skip(1).Aggregate((x, y) => x + "." + y);
                return split[0] + "." + GetPropertyName(prop, typePropiedad);
            }

        }

        public string GetAttributeDBName(string propiedad, Type TypeEntidad)
        {
            var Props = TypeEntidad.GetProperties().SelectMany(x => x.GetCustomAttributes<NameEntityAttribute>())
                .Where(Att =>
                {
                    return (Att != null && Att.PropertyName == propiedad);
                });

            if (!Props.Any()) return string.Empty;

            if (Props.Count() > 1)
                throw new Exception(string.Format(MensajePropiedadDuplicada, propiedad, TypeEntidad.Name));

            return Props.ElementAt(0).ColumnName.Trim();
        }

        public string[] GetPropertyDBKey(Type TypeEntidad)
        {
            var Props = TypeEntidad.GetProperties().SelectMany(x => x.GetCustomAttributes<NameEntityAttribute>())
                .Where(Att =>
                {
                    return (Att != null && Convert.ToBoolean(Att.PrimaryKey));
                });


            var resultado = new string[Props.Count()];

            for (int i = 0; i < Props.Count(); i++)
            {
                resultado[i] = Props.ElementAt(i).PropertyName.Trim();
            }

            return resultado;
        }

        //public string GetEntityTable(Type TypeEntidad)
        //{
        //    var A = (TableAttribute)Attribute.GetCustomAttribute(TypeEntidad, typeof(TableAttribute));

        //    if (A == null)
        //        throw new Exception(string.Format(MensajeEntidadNoEncontrada, TypeEntidad.Name));

        //    return A.TableName.Trim();
        //}

        public string[] GetPropertyDBKeyAndKeyChild(Type TypeEntidad)
        {
            var Props = TypeEntidad.GetProperties().SelectMany(x => x.GetCustomAttributes<NameEntityAttribute>())
               .Where(Att =>
               {
                   return (Att != null && Convert.ToBoolean(Att.IsForeingKey));
               });

            var resultado = new string[Props.Count()];

            for (int i = 0; i < Props.Count(); i++)
            {
                resultado[i] = Props.ElementAt(i).PropertyName.Trim();
            }

            return resultado;
        }
    }
}
