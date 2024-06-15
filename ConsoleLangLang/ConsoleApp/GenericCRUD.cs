using ConsoleLangLang.ConsoleApp.DTO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LangLang.Domain.Model;

public class GenericCrud
{
    public T Create<T>() where T : new()
    {
        T item = new T();
        var validator = new PropertyValidator<T>(item);

        foreach (PropertyInfo prop in typeof(T).GetProperties())
        {
            if (prop.CanWrite && !IsCollectionType(prop.PropertyType))
            {
                object value;

                if (prop.PropertyType.IsEnum)
                    value = ReadEnumFromUser<T>(prop);
                else
                {
                    string formatHint = GetFormatHint(prop.PropertyType);
                    Console.Write($"Enter {prop.Name} ({prop.PropertyType.Name}{formatHint}): ");

                    string input = Console.ReadLine();
                    value = ConvertValue(input, prop.PropertyType);
                }

                if (value == null)
                    return default(T);

                prop.SetValue(item, value);

                string validationError = validator.ValidateProperty(prop.Name);
                if (validationError != null)
                {
                    Console.WriteLine(validationError);
                    return default(T);
                }
            }
        }

        if (!validator.IsValid())
            return default(T);

        return item;
    }

    private object ReadEnumFromUser<T>(PropertyInfo prop) where T : new()
    {
        Console.WriteLine($"Choose {prop.Name}:");
        var enumValues = Enum.GetValues(prop.PropertyType).Cast<Enum>().OrderBy(e => e.ToString()).ToList();

        if (typeof(T) == typeof(Teacher) && (prop.Name == "Languages" || prop.Name == "LevelOfLanguages"))
            return SelectMultipleEnum(enumValues);
        else if (typeof(T) == typeof(Course) || typeof(T) == typeof(ExamTerm))
            return SelectMultipleEnum(enumValues);
        else
            return SelectSingleEnum(enumValues);

    }

    private List<Enum> SelectMultipleEnum(List<Enum> enumValues)
    {
        Console.WriteLine("Select multiple options separated by commas (e.g., 1,2,3):");
        for (int i = 0; i < enumValues.Count; i++)
            Console.WriteLine($"{i + 1}. {enumValues[i]}");

        Console.Write($"Enter choices (1-{enumValues.Count}): ");
        string input = Console.ReadLine();

        string[] choices = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var selectedEnums = new List<Enum>();

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

    private Enum SelectSingleEnum(List<Enum> enumValues)
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
        if (type == typeof(List<DayOfWeek>))
            return false;
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

    public T Update<T>(T item)
    {
        foreach (PropertyInfo prop in typeof(T).GetProperties())
        {
            if (prop.CanWrite)
            {
                object currentValue = prop.GetValue(item);

                Console.Write($"Enter new value for {prop.Name} ({prop.PropertyType}) or press Enter to keep the current value ({currentValue}): ");
                string input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    object value = ConvertValue(input, prop.PropertyType);
                    prop.SetValue(item, value);
                }
                else
                   prop.SetValue(item, currentValue);
            }
        }
        return item;
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
                    else
                        valueString = value?.ToString() ?? string.Empty;

                    Console.Write(valueString.PadRight(columnWidths[i] + 2)); // Padding for table formatting
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
            catch(Exception e)
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
}
