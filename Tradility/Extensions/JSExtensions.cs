using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradility.Extensions
{
    public static class JSExtensions
    {
        public static string ToJS(this double value) => value.ToString().Replace(",", ".");
        public static string ToJSString(this double value) => value.ToJS().ToJS();

        public static string ToJS(this decimal value) => value.ToString().Replace(",", ".");
        public static string ToJSString(this decimal value) => value.ToJS().ToJS();

        public static string ToJS(this long value) => value.ToString().Replace(",", ".");
        public static string ToJSString(this long value) => value.ToJS().ToJS();

        public static string ToJS(this string value) 
        {
            if (value.First() == '\'' && value.Last() == '\'')
                return value;

            return $"'{value}'";
        }
    }
}
