using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collectorcord {
    public static class TextUtil {
        public static string Capitalize(string s) {
            // Check for empty string.  
            if (string.IsNullOrEmpty(s)) {
                return string.Empty;
            }
            // Return char and concat substring.  
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static string ArrayToString(string[] arr, bool withStringQuotes = false) {
            if (arr.Count() < 1) {
                return "()";
            }
            string ret = "(";
            foreach (string str in arr) {
                if (withStringQuotes) {
                    ret += "'" + str + "', ";
                    continue;
                }
                ret += str + ", ";
            }
            //truncate final comma and space
            ret = ret.Substring(0, ret.Length - 2);
            ret += ")";
            return ret;
        }

    }
}
