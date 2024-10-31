using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeatherApp.Utils
{
    /// <summary>
    /// This class contains helper methods that generate OData filters
    /// It will also be used for any other filter-related operations
    /// </summary>
    public class FilterHelper
    {
        private static DateTime _defaultDate = DateTime.MinValue;

        public static string GenerateFilter(string filter = "", string value = null, bool isFirst = false)
        {
            if (string.IsNullOrEmpty(filter) || string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return isFirst
                ? $"?$filter={filter} eq '{value}'"
                : $" and {filter} eq '{value}'";
        }

        public static string GenerateFilter(string filter = "", int value = 0, bool isFirst = false)
        {
            if (string.IsNullOrEmpty(filter) || value == 0)
            {
                return string.Empty;
            }

            return isFirst
                ? $"?$filter={filter}={value}"
                : $" and {filter}={value}";
        }

        public static string GenerateFilter(string filter = "", bool value = false, bool isFirst = false)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return string.Empty;
            }

            return isFirst
                ? $"?$filter={filter}={value}"
                : $" and {filter}={value}";
        }

        // Handle lists. Use above basic methods to generate filters for each item in the list
        public static string GenerateFilter(string filter = "", List<string> values = null, bool isFirst = false)
        {
            if (string.IsNullOrEmpty(filter) || values == null || values.Count == 0)
            {
                return string.Empty;
            }

            var filterString = string.Empty;
            for (var i = 0; i < values.Count; i++)
            {
                filterString += GenerateFilter(filter, values[i], i == 0);
            }

            return filterString;
        }

        public static string GenerateTopQuery(int top, int skip, bool isFirstItem)
        {
            if (top == 0)
            {
                return string.Empty;
            }

            return isFirstItem
                ? $"?$top={top}&$skip={skip}"
                : $"&$top={top}&$skip={skip}";
        }
    }
}