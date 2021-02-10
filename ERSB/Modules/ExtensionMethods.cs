using System.Collections.Generic;
using System.Linq;

namespace ERSB.Modules
{
    static class ExtensionMethods
    {
        public static string ToFileNamesString(this IEnumerable<string> enumerable)
        {
            return enumerable.Aggregate(string.Empty, (current, item) => current + $"{item}\n");
        }
    }
}
