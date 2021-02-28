using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Accounts.Infrastructure.Extensions
{
    public static class ListExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> o)
        {
            return o == null || !o.Any();
        }

        public static string ToSqlInStatement(this string[] o)
        {
            var sb = new StringBuilder("'");

            for (int i = 0; i < o.Length; i++)
            {
                sb.Append(o[i]);

                if (i != o.Length - 1)
                {
                    sb.Append("','");
                }
                else
                {
                    sb.Append("'");
                }
            }

            return sb.ToString();
        }
    }
}