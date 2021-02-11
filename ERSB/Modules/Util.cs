using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using ERSB.Models;

namespace ERSB.Modules
{
    public static class Util
    {
        public static void UpdateProgress(this DownloadItem firstItem, DownloadItem secondItem, Action actionOnComplete)
        {
            firstItem.ReceivedBytes = secondItem.ReceivedBytes;
            if(firstItem.TotalBytes == 0) firstItem.TotalBytes = secondItem.TotalBytes;
            firstItem.State = secondItem.State;
            if(firstItem.State == "completed")
            {
                actionOnComplete();
            }
          
        }
        public static DataTable ToDataTable<T>(this IEnumerable<T> iEnumerable)
        {
            if(iEnumerable is null) throw new ArgumentNullException(nameof(iEnumerable), "Enumerable is null");
            var dataTable = new DataTable();
            var propertyDescriptorCollection =
                TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor propertyDescriptor in propertyDescriptorCollection)
            {
                var type = propertyDescriptor.PropertyType;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    type = Nullable.GetUnderlyingType(type);


                dataTable.Columns.Add(propertyDescriptor.DisplayName, type!);
            }
            var values = new object[propertyDescriptorCollection.Count];
            foreach (var iListItem in iEnumerable)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = propertyDescriptorCollection[i].GetValue(iListItem);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
        public static string ToSafeString(this object obj)
        {
            return obj is null ? "" : obj.ToString();
        }
        public static string ToSafeString(this object obj, string defaultReturn)
        {
            return (obj is null ? defaultReturn : obj.ToString());
        }

        private static readonly string DataPath = GetDataFolderPath();

        public static string GetDataFolderPath()
        {
            var appPath = AppContext.BaseDirectory;
            var path = Path.Combine(appPath!, "data");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        /// <summary> 
        /// returns csv file name with given name in data folder of application.
        /// This does not create actual file. the file may exist or may not.
        /// </summary>
        /// <param name="fileName">file name without extension</param>
        /// <returns>full path of the csv file appended with ERSB/Data path. </returns>
        public static string CreateCsvFilePath(this string fileName)
        {
            //var dataPath= GetDataFolderPath();
            var fullPath = Path.Combine(DataPath, $"{fileName}.csv");
            return fullPath;
        }

        public static string WithoutExt(this string str)
        {
            return Path.GetFileNameWithoutExtension(str);
        }

        public static IEnumerable<string> GetFileNamesFromFolder(string folder)
        {
            var itemList = new List<string>();
            if (string.IsNullOrEmpty(folder)) return itemList;
            itemList.AddRange(Directory.GetFiles(folder)
                .Where(i => Path.GetExtension(i).ToUpperInvariant().Equals(".CSV",StringComparison.Ordinal))
                .Select(Path.GetFileNameWithoutExtension));
            return itemList;
        }

        public static string GetStaticString(string firstString, string secondString)
        {
            if(firstString is null) throw new ArgumentNullException(nameof(firstString),"Parameter must not be null");
            if(secondString is null) throw new ArgumentNullException(nameof(secondString),"Parameter must not be null");
            var i = 0;
            var outString = "";
            foreach (var c in firstString)
            {
                if (c.ToString()[0] == secondString[i])
                {
                    outString += c.ToString();
                    i += 1;
                }
                else
                {
                    return outString;
                }
            }

            return outString;
        }

        /// <summary>
        ///  Returns a string Array consisting of the all Numbers between two Specified Numbers.
        /// </summary>
        /// <param name="startValue">Required. Integer expression. The First Number.</param>
        /// <param name="endValue">Required. Integer expression. The Last Number. </param>
        /// <param name="incValue">Optional. Integer expression. The number of increment between each value.</param>
        /// <returns>Returns a string Array consisting of the all Numbers between two Specified Numbers.</returns>
        public static IEnumerable<string> Generate(int startValue, int endValue, int incValue = 1)
        {
            string outString = null;
            var separator = "";
            while (!(startValue > endValue))
            {
                outString += separator + startValue;
                startValue += incValue;
                separator = ",";
            }

            return outString?.Split(',');
        }

        /// <summary>
        /// Returns a string consisting of the specified number of zeros.
        /// </summary>
        /// <param name="number">Required. Integer expression. The number of zeros you want in the string.</param>
        /// <returns>Returns a string consisting of the specified number of zeros. </returns>
        public static string GetZeros(int number)
        {
            var outString = "";
            if (number <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(number),"Argument cannot be 0 or Less");
            }

            while (!(number <= 0))
            {
                outString += "0";
                number--;
            }

            return outString;
        }
    }
}