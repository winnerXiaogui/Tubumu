using System;
using System.Collections.Specialized;
using System.Text;

namespace Tubumu.Modules.Framework.Extensions
{
    public static class NameValueCollectionExtensions
    {
        public static bool IsTrue(this NameValueCollection collection, string key)
        {
            if (collection == null) return false;

            var values = collection.GetValues(key);
            if (values.IsNullOrEmpty()) return false;


            return bool.TryParse(values[0], out var isTrue) && isTrue;
        }

        public static bool? IsTrueNullable(this NameValueCollection collection, string key)
        {
            if (collection == null) return null;

            var values = collection.GetValues(key);
            if (values.IsNullOrEmpty()) return null;

            return bool.TryParse(values[0], out bool isTrueValue) && isTrueValue;
        }

        public static string ToQueryString(this NameValueCollection queryString)
        {
            if (queryString.Count > 0)
            {
                StringBuilder qs = new StringBuilder();
                qs.Append("?");
                for (var i = 0; i < queryString.Count; i++)
                {
                    if (i > 0)
                        qs.Append("&");

                    qs.AppendFormat("{0}={1}", queryString.Keys[i], queryString[i]);
                }
                return qs.ToString();
            }
            return String.Empty;
        }
    }
}