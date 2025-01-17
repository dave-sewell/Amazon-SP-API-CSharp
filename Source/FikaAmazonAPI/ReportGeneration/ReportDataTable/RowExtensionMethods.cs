﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FikaAmazonAPI.ReportGeneration.ReportDataTable
{
    public static class RowExtensionMethods
    {
        public static string GetString(this TableRow row, string id)
        {
            return AValueWithThisIdExists(row, id)
                       ? row[id]
                       : null;
        }
        public static int GetInt32(this TableRow row, string id)
        {
            return AValueWithThisIdExists(row, id) && TheValueIsNotEmpty(row, id)
                       ? Convert.ToInt32(row[id])
                       : int.MinValue;
        }
        public static long GetInt64(this TableRow row, string id)
        {
            return AValueWithThisIdExists(row, id) && TheValueIsNotEmpty(row, id)
                       ? Convert.ToInt64(row[id])
                       : long.MinValue;
        }
        public static decimal GetDecimal(this TableRow row, string id)
        {
            return AValueWithThisIdExists(row, id) && TheValueIsNotEmpty(row, id)
                       ? Convert.ToDecimal(row[id], CultureInfo.InvariantCulture)
                       : decimal.MinValue;
        }
        public static DateTime GetDateTime(this TableRow row, string id)
        {
            return AValueWithThisIdExists(row, id) && TheValueIsNotEmpty(row, id)
                       ? Convert.ToDateTime(row[id])
                       : DateTime.MinValue;
        }
        public static bool GetBoolean(this TableRow row, string id)
        {
            if (TheBooleanValueIsEmpty(row, id))
                return false;

            AssertThatTheRequestIsValid(row, id);

            return string.Equals(row[id], "true", StringComparison.OrdinalIgnoreCase);
        }
        private static void AssertThatTheRequestIsValid(TableRow row, string id)
        {
            AssertThatAValueWithThisIdExistsInThisRow(row, id);
            AssertThatThisIsAnAcceptableBoolValue(row, id);
        }
        private static void AssertThatThisIsAnAcceptableBoolValue(TableRow row, string id)
        {
            var acceptedValues = new[] { "true", "false" };
            if (acceptedValues.Contains(row[id], StringComparer.OrdinalIgnoreCase) == false)
                throw new InvalidCastException($"You must use 'true' or 'false' when setting bools for {id}");
        }

        private static void AssertThatAValueWithThisIdExistsInThisRow(TableRow row, string id)
        {
            if (AValueWithThisIdExists(row, id) == false)
                throw new InvalidOperationException($"{id} could not be found in the row.");
        }
        private static bool TheBooleanValueIsEmpty(TableRow row, string id)
        {
            return AValueWithThisIdExists(row, id) && string.IsNullOrEmpty(row[id]);
        }

        public static float GetSingle(this TableRow row, string id)
        {
            return AValueWithThisIdExists(row, id) && TheValueIsNotEmpty(row, id)
                ? Convert.ToSingle(row[id])
                : float.MinValue;
        }
        public static char GetChar(this TableRow row, string id)
        {
            return Convert.ToChar(row[id]);
        }
        public static T GetDiscreteEnum<T>(this TableRow row, string id) where T : struct, IConvertible
        {
            var value = row[id].Replace(" ", string.Empty);
            T @enum;
            if (Enum.TryParse(value, true, out @enum))
                return @enum;

            throw new InvalidOperationException($"No enum with value {value} found in enum {typeof(T).Name}");
        }
        public static T GetDiscreteEnum<T>(this TableRow row, string id, T defaultValue) where T : struct, IConvertible
        {
            var value = row[id].Replace(" ", string.Empty);
            T @enum;
            return Enum.TryParse(value, true, out @enum) ? @enum : defaultValue;
        }
        public static TEnum GetEnumValue<TEnum>(this TableRow row, string id)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), row[id]);
        }
        public static Enum GetEnum<T>(this TableRow row, string id) where T : class
        {
            return GetTheEnumValue<T>(row[id], id);
        }
        private static Enum GetTheEnumValue<T>(string rowValue, string propertyName) where T : class
        {
            var value = rowValue.Replace(" ", string.Empty);

            var enumType = GetTheEnumType<T>(propertyName, value);

            return Enum.Parse(enumType, value, true) as Enum;
        }

        private static Type GetTheEnumType<T>(string propertyName, string value)
        {
            var propertyType = (from property in typeof(T).GetProperties()
                                where property.Name == propertyName
                                      && property.PropertyType.IsEnum
                                      && EnumValueIsDefinedCaseInsensitve(property.PropertyType, value)
                                select property.PropertyType).FirstOrDefault();

            if (propertyType == null)
                throw new InvalidOperationException($"No enum with value {value} found in type {typeof(T).Name}");

            return propertyType;
        }
        private static bool EnumValueIsDefinedCaseInsensitve(Type @enum, string value)
        {
            Enum parsedEnum = null;
            try
            {
                parsedEnum = Enum.Parse(@enum, value, true) as Enum;
            }
            catch
            {
                // just catch it
            }

            return parsedEnum != null;
        }
        public static Guid GetGuid(this TableRow row, string id)
        {
            return AValueWithThisIdExists(row, id) && TheValueIsNotEmpty(row, id)
                       ? new Guid(row[id])
                       : new Guid();
        }
        public static double GetDouble(this TableRow row, string id)
        {
            return AValueWithThisIdExists(row, id) && TheValueIsNotEmpty(row, id)
                       ? Convert.ToDouble(row[id], CultureInfo.InvariantCulture)
                       : double.MinValue;
        }
        private static bool AValueWithThisIdExists(IEnumerable<KeyValuePair<string, string>> row, string id)
        {
            return row.Any(x => x.Key == id);
        }
        private static bool TheValueIsNotEmpty(TableRow row, string id)
        {
            return string.IsNullOrEmpty(row[id]) == false;
        }
    }
}
