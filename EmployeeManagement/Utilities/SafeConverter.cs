using System;

namespace EmployeeManagement.Utilities
{
    public static class SafeConverter
    {
        public static decimal ToDecimal(object value, decimal defaultValue = 0m)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            if (value is decimal decimalValue)
                return decimalValue;

            if (value is int intValue)
                return Convert.ToDecimal(intValue);

            if (value is double doubleValue)
                return Convert.ToDecimal(doubleValue);

            if (value is float floatValue)
                return Convert.ToDecimal(floatValue);

            if (decimal.TryParse(value.ToString(), out decimal result))
                return result;

            return defaultValue;
        }

        public static int ToInt32(object value, int defaultValue = 0)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            if (value is int intValue)
                return intValue;

            if (value is decimal decimalValue)
                return Convert.ToInt32(decimalValue);

            if (int.TryParse(value.ToString(), out int result))
                return result;

            return defaultValue;
        }

        public static string ToString(object value, string defaultValue = "")
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            return value.ToString();
        }
    }
}