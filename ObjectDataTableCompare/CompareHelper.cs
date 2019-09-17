
namespace ObjectDataTableCompare
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public static class CompareHelper
    {
        public static string[] IndexString = new string[] { "Id" };

        public static IEnumerable<CompareResult<T>> Compare<T>(IEnumerable<T> cacheObj, DataTable dt, string[] index = null)
            where T : new()
        {
            var _index = index ?? IndexString;
            var type = typeof(T);
            var result = new List<CompareResult<T>>();

            var indexProps = type.GetProperties().Where(p => _index.Contains(p.Name)).ToList();
            var unIndexProps = type.GetProperties().Except(indexProps).ToList();

            result.Add(new CompareResult<T>()
            {
                Type = CompareType.Update,
                Data = dt.AsEnumerable().Where(dr =>
                {
                    return cacheObj.Any(obj =>
                    {
                        bool match = true;
                        indexProps.ForEach(idxProp =>
                        {
                            if (idxProp.GetValue(obj).ToString() != dr[idxProp.Name].ToString())
                            {
                                match = false;
                            }
                        });

                        return match && unIndexProps.Any(unIdxProp => unIdxProp.GetValue(obj).ToString() != dr[unIdxProp.Name].ToString());
                    });
                }).Select(dr => GenerateInstance<T>(dr))
            });

            result.Add(new CompareResult<T>()
            {
                Type = CompareType.Insert,
                Data = dt.AsEnumerable().Where(dr =>
                {
                    return !cacheObj.Any(obj =>
                    {
                        bool match = true;
                        indexProps.ForEach(idxProp =>
                        {
                            if (idxProp.GetValue(obj).ToString() != dr[idxProp.Name].ToString())
                            {
                                match = false;
                            }
                        });

                        return match;
                    });
                }).Select(dr => GenerateInstance<T>(dr))
            });

            result.Add(new CompareResult<T>()
            {
                Type = CompareType.Delete,
                Data = cacheObj.Where(obj =>
                {
                    return !dt.AsEnumerable().Any(dr =>
                    {
                        bool match = true;
                        indexProps.ForEach(idxProp =>
                        {
                            if (idxProp.GetValue(obj).ToString() != dr[idxProp.Name].ToString())
                            {
                                match = false;
                            }
                        });

                        return match;
                    });
                })
            });

            return result;
        }

        private static T GenerateInstance<T>(DataRow dr)
            where T : new()
        {
            var result = new T();
            var type = typeof(T);
            foreach (var prop in type.GetProperties())
            {
                prop.SetValue(result, dr[prop.Name]);
            }

            return result;
        }

        public enum CompareType
        {
            Insert,
            Update,
            Delete
        }

        public class CompareResult<T>
        {
            public CompareType Type { get; set; }

            public IEnumerable<T> Data { get; set; }
        }
    }
}
