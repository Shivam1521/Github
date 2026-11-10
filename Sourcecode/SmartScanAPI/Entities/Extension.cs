using Dapper;
using System.Data;
using System.Reflection;

namespace Entities
{
    public static class Extension
    {
        /// <summary>
        /// Converts an enumerable set to a Dapper TVP
        /// </summary>
        public static SqlMapper.ICustomQueryParameter AsTableValuedParameter<T>(
            this IEnumerable<T> enumerable,
            string typeName,
            IEnumerable<string>? orderedColumnNames = null)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            var dataTable = new DataTable();

            // Case 1: Simple value type or string
            if (typeof(T).IsValueType || typeof(T) == typeof(string))
            {
                dataTable.Columns.Add(
                    orderedColumnNames?.FirstOrDefault() ?? "NONAME",
                    Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)
                );

                foreach (var obj in enumerable)
                {
                    dataTable.Rows.Add((object?)obj ?? DBNull.Value);
                }
            }
            else
            {
                // Case 2: Complex type
                PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var readableProperties = properties.Where(p => p.CanRead).ToArray();

                if (readableProperties.Length > 1 && orderedColumnNames == null)
                    throw new ArgumentException(
                        "Ordered list of column names must be provided when TVP contains more than one column"
                    );

                var columnNames = (orderedColumnNames ?? readableProperties.Select(p => p.Name)).ToArray();
                var propertyMap = readableProperties.ToDictionary(p => p.Name);

                // Build columns
                foreach (var name in columnNames)
                {
                    var prop = propertyMap[name];
                    var columnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    dataTable.Columns.Add(name, columnType);
                }

                // Fill rows
                foreach (var obj in enumerable)
                {
                    var values = columnNames.Select(name =>
                    {
                        var value = propertyMap[name].GetValue(obj);
                        return value ?? DBNull.Value;
                    }).ToArray();

                    dataTable.Rows.Add(values);
                }
            }

            return dataTable.AsTableValuedParameter(typeName);
        }
    }
}
