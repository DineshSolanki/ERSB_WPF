using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERSB.Modules
{
    static class ExtensionMethods
    {
        public static string ToFileNamesString(this IEnumerable<string> enumerable)
        {
            string fileNames = string.Empty;
            foreach (var item in enumerable)
            {
                fileNames += $"{item}\n";
            }
            return fileNames;
        }
    }
}
