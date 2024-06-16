using ConsoleLangLang.ConsoleApp.DTO;
using LangLang.Migrations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class GenericCrud
{
    public T Create<T>() where T : new()
    {
        T item = new T();
        var validator = new PropertyValidator<T>(item);

        foreach (PropertyInfo property in typeof(T).GetProperties())
        {
            if (property.CanWrite)
            {
                object value;

                if (IsCollectionType(property.PropertyType))
                    value = GetPropertyValue(property, property.PropertyType.GetGenericArguments()[0]);
                else
                    value = GetPropertyValue(property, property.PropertyType);

                if (value == null)
                    return default(T);

                property.SetValue(item, value);

                string validationError = validator.ValidateProperty(property.Name);
                if (validationError != null)
                {
                    Console.WriteLine(validationError);
                    Console.ReadLine();
                    return default(T);
                }
            }
        }

        if (!validator.IsValid())
        {
            Console.ReadLine();
            return default(T);
        }

        return item;
    }

    private object GetPropertyValue(PropertyInfo property, Type elementType)
    {
        if (elementType.IsEnum)
            return ReadEnumFromUser(property, elementType);
        else
        {
            string formatHint = GetFormatHint(property.PropertyType);
            Console.Write($"Enter {property.Name} ({property.PropertyType.Name}{formatHint}): ");

            string input = Console.ReadLine();
            return ConvertValue(input, property.PropertyType);
        }
    }

    private object GetPropertyValueUpdate(PropertyInfo property, Type elementType, object currentValue)
    {
        if (elementType.IsEnum)
            return ReadEnumFromUser(property, elementType);
        else
        {
            string formatHint = GetFormatHint(property.PropertyType);
            Console.Write($"Enter new value for {property.Name} ({property.PropertyType}) or press Enter to keep the current value ({currentValue}): ");

            string input = Console.ReadLine();

            if (!string.IsNullOrEmpty(input))
                return ConvertValue(input, property.PropertyType);

            return null;
        }
    }

    private object ReadEnumFromUser(PropertyInfo prop, Type elementType)
    {
        Console.WriteLine($"Choose {prop.Name}:");
        var enumValues = GetEnumsList(elementType);

        if (prop.Name == "Languages" || prop.Name == "LevelOfLanguages" || prop.Name == "WorkDays")
        {
            var selectedEnums = SelectMultipleEnum(enumValues, elementType);
            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = (IList)Activator.CreateInstance(listType);

            foreach (var enumValue in selectedEnums)
                list.Add(Convert.ChangeType(enumValue, elementType));
            return list;
        }
        else
            return SelectSingleEnum(enumValues);
    }

    private List<object> GetEnumsList(Type elementType)
    {
        var nullValue = Enum.GetValues(elementType)
                    .Cast<object>()
                    .FirstOrDefault(e => e.ToString() == "NULL");

        var enumValues = Enum.GetValues(elementType)
                             .Cast<object>()
                             .Where(e => !e.Equals(nullValue))
                             .OrderBy(e => e.ToString())
                             .ToList();
        return enumValues;
    }

    private List<object> SelectMultipleEnum(List<object> enumValues, Type elementType)
    {
        Console.WriteLine("Select multiple options separated by commas (e.g., 1,2,3):");
        for (int i = 0; i < enumValues.Count; i++)
            Console.WriteLine($"{i + 1}. {enumValues[i]}");

        Console.Write($"Enter choices (1-{enumValues.Count}): ");
        string input = Console.ReadLine();

        string[] choices = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var selectedEnums = new List<object>();

        foreach (var choice in choices)
        {
            if (int.TryParse(choice, out int index) && index >= 1 && index <= enumValues.Count)
                selectedEnums.Add(enumValues[index - 1]);
            else
            {
                Console.WriteLine("Invalid choice.");
                return null;
            }
        }

        return selectedEnums;
    }

    private object SelectSingleEnum(List<object> enumValues)
    {
        for (int i = 0; i < enumValues.Count; i++)
            Console.WriteLine($"{i + 1}. {enumValues[i]}");

        Console.Write($"Enter choice (1-{enumValues.Count}): ");
        string input = Console.ReadLine();

        if (!int.TryParse(input, out int choice) || choice < 1 || choice > enumValues.Count)
        {
            Console.WriteLine("Invalid choice.");
            return null;
        }

        return enumValues[choice - 1];
    }

    private bool IsCollectionType(Type type)
    {
        return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
    }

    private string GetFormatHint(Type type)
    {
        if (type == typeof(int))
            return " (e.g., 123)";
        else if (type == typeof(double))
            return " (e.g., 123.45)";
        else if (type == typeof(DateTime))
            return " (e.g., 2023-01-01)";
        else if (type == typeof(bool))
            return " (e.g., true or false)";
        else
            return string.Empty;
    }
    public void Read<T>(T item)
    {
        PrintTable(new List<T> { item });
    }

    public T Update<T>(T item) where T : new()
    {
        var validator = new PropertyValidator<T>(item);

        foreach (PropertyInfo prop in typeof(T).GetProperties())
        {
            if (prop.CanWrite)
            {
                object currentValue = prop.GetValue(item);

                if (IsCollectionType(prop.PropertyType))
                    UpdateCollectionType(prop, item, currentValue);
                else
                    UpdateNonCollectionType(prop, item, currentValue);

                string validationError = validator.ValidateProperty(prop.Name);
                if (validationError != null)
                {
                    Console.WriteLine(validationError);
                    Console.ReadLine();
                    return default(T);
                }
            }
        }

        if (!validator.IsValid())
        {
            Console.ReadLine();
            return default(T);
        }

        return item;
    }

    private void UpdateCollectionType<T>(PropertyInfo property, T item, object currenValue) where T : new()
    {
        object value = GetPropertyValue(property, property.PropertyType.GetGenericArguments()[0]);
        int count = GetListCount(value);

        if (count!=0)
             property.SetValue(item, value);
        else
            property.SetValue(item, currenValue);
    }

    private void UpdateNonCollectionType<T>(PropertyInfo property, T item, object currentValue) where T : new()
    {
        object value = GetPropertyValueUpdate(property, property.PropertyType, currentValue);

        if (value != null)
            property.SetValue(item, value);
    }

    public void PrintTable<T>(List<T> dataStore)
    {
        if (!dataStore.Any())
        {
            Console.WriteLine("No data available.");
            return;
        }

        var properties = typeof(T).GetProperties().Where(p => p.CanRead).ToArray();
        int[] columnWidths = CalculateColumnWidths(dataStore, properties);

        PrintHeader(properties, columnWidths);
        PrintRows(dataStore, properties, columnWidths);
    }

    private int[] CalculateColumnWidths<T>(List<T> dataStore, PropertyInfo[] properties)
    {
        int[] columnWidths = new int[properties.Length];

        for (int i = 0; i < properties.Length; i++)
        {
            columnWidths[i] = properties[i].Name.Length;

            foreach (var item in dataStore)
            {
                var value = properties[i].GetValue(item);
                string valueString;

                if (value is IEnumerable enumerable && !(value is string))
                    valueString = string.Join(",", enumerable.Cast<object>());
                else
                    valueString = value?.ToString() ?? string.Empty;

                if (valueString.Length > columnWidths[i])
                    columnWidths[i] = valueString.Length;
            }
        }

        return columnWidths;
    }

    private void PrintHeader(PropertyInfo[] properties, int[] columnWidths)
    {
        for (int i = 0; i < properties.Length; i++)
            Console.Write(properties[i].Name.PadRight(columnWidths[i] + 2));

        Console.WriteLine();
    }

    private void PrintRows<T>(List<T> dataStore, PropertyInfo[] properties, int[] columnWidths)
    {
        foreach (var item in dataStore)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                try
                {
                    var value = properties[i].GetValue(item);
                    string valueString;

                    if (value is IEnumerable enumerable && !(value is string))
                        valueString = string.Join(",", enumerable.Cast<object>());
                    else if (value is DateTime)
                        valueString = value?.ToString().Split(" ")[0] ?? string.Empty;
                    else
                        valueString = value?.ToString() ?? string.Empty;

                    Console.Write(valueString.PadRight(columnWidths[i] + 2));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accessing property {properties[i].Name}: {ex.Message}");
                }
            }
            Console.WriteLine();
        }
    }

    public static object ConvertValue(string input, Type type)
    {
        try
        {
            if (type == typeof(int))
                return int.Parse(input);
            else if (type == typeof(float))
                return float.Parse(input);
            else if (type == typeof(double))
                return double.Parse(input);
            else if (type == typeof(bool))
                return bool.Parse(input);
            else if (type == typeof(DateTime))
                return DateTime.Parse(input);
            else if (type.IsEnum)
                return Enum.Parse(type, input);
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                return ConvertListType(input, type);
            else
                return input;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return null;
        }
    }

    private static object ConvertListType(string input, Type type)
    {
        if (string.IsNullOrEmpty(input))
            return null;

        Type itemType = type.GetGenericArguments()[0];
        string[] items = input.Split(',');
        var list = (IList)Activator.CreateInstance(type);

        foreach (string item in items)
            list.Add(ConvertValue(item.Trim(), itemType));

        return list;
    }

    public static int GetListCount(object list)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));

        Type type = list.GetType();

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            PropertyInfo countProperty = type.GetProperty("Count");
            if (countProperty != null)
                return (int)countProperty.GetValue(list);
        }

        if (list is IList)
            return ((IList)list).Count;

        return 0;

    }
}
