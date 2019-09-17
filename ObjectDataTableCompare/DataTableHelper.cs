
namespace ObjectDataTableCompare
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;

    public static class DataTableHelper
    {
        /// <summary>
        /// Model轉DataTable ColumnName為Description；若沒有擇取屬性名稱
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="collection">type obj</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> collection, Type type = null)
        {
            var props = type?.GetProperties() ?? typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(
                props.Select(p =>
                    new DataColumn(
                        ((DescriptionAttribute)p.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault())?.Description ?? p.Name,
                        p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? p.PropertyType.GetGenericArguments()[0] : p.PropertyType)
                    ).ToArray());

            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (var pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(pi.PropertyType == typeof(string) && obj == null ? "" : obj ?? DBNull.Value);
                    }

                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }

            return dt;
        }
    }
}
