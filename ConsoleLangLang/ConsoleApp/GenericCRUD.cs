using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class GenericCrud
{
    public delegate TOutput Converter<in TInput, out TOutput>(TInput input);

    public T Create<T>() where T : new()
    {
        T item = new T();

        bool isFirstProperty = true;

        foreach (PropertyInfo prop in typeof(T).GetProperties())
        {
            if (isFirstProperty)
            {
                isFirstProperty = false;
                continue;
            }

            if (prop.CanWrite && !IsCollectionType(prop.PropertyType))
            {
                string formatHint = GetFormatHint(prop.PropertyType);
                Console.Write($"Enter {prop.Name} ({prop.PropertyType.Name}{formatHint}): ");

                string input = Console.ReadLine();
                object value = ConvertValue(input, prop.PropertyType);
                prop.SetValue(item, value);
            }
        }

        return item;
    }
    public void Read<TInput, TOutput>(TInput item, Converter<TInput, TOutput> converter)
    {
        var model = converter(item);
        PrintTable(new List<TOutput> { model });
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
            return " (e.g., 2023-01-01 12:00:00)";
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
                Console.Write($"Enter new value for {prop.Name} ({prop.PropertyType}) or press Enter to keep the current value: ");
                string input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    object value = ConvertValue(input, prop.PropertyType);
                    prop.SetValue(item, value);
                }
            }
        }
        return item;
    }

    /*    public void Delete(int index)
        {
            if (index < 0 || index >= dataStore.Count)
            {
                Console.WriteLine("Invalid index.");
                return;
            }
            dataStore.RemoveAt(index);
            Console.WriteLine("Item deleted.");
        }*/

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

        // Calculate maximum widths of each column
        for (int i = 0; i < properties.Length; i++)
        {
            columnWidths[i] = properties[i].Name.Length; // Start with the header length

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
        {
            Console.Write(properties[i].Name.PadRight(columnWidths[i] + 2));
        }
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
        if (type == typeof(int))
        {
            return int.Parse(input);
        }
        else if (type == typeof(float))
        {
            return float.Parse(input);
        }
        else if (type == typeof(double))
        {
            return double.Parse(input);
        }
        else if (type == typeof(bool))
        {
            return bool.Parse(input);
        }
        else if (type == typeof(DateTime))
        {
            return DateTime.Parse(input);
        }
        else if (type.IsEnum)
        {
            return Enum.Parse(type, input);
        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            return ConvertListType(input, type);
        }
        else
        {
            return input;
        }
    }

    private static object ConvertListType(string input, Type type)
    {
        if (string.IsNullOrEmpty(input)) 
        {
            return null;
        }
        Type itemType = type.GetGenericArguments()[0];
        string[] items = input.Split(',');
        var list = (IList)Activator.CreateInstance(type);
        foreach (string item in items)
        {
            list.Add(ConvertValue(item.Trim(), itemType));
        }
        return list;
    }
}
