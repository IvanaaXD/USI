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

        foreach (PropertyInfo prop in typeof(T).GetProperties())
        {
            if (prop.CanWrite)
            {
                Console.Write($"Enter {prop.Name} ({prop.PropertyType}): ");
                string input = Console.ReadLine();
                object value = ConvertValue(input, prop.PropertyType);
                prop.SetValue(item, value);
            }
        }

        return item;
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

        var properties = typeof(T).GetProperties().Where(p => p.CanRead);

        // Print header
        foreach (var prop in properties)
        {
            Console.Write($"{prop.Name}\t");
        }
        Console.WriteLine();

        // Print rows
        foreach (var item in dataStore)
        {
            foreach (var prop in properties)
            {
                var value = prop.GetValue(item);
                if (value is IEnumerable enumerable && !(value is string))
                {
                    Console.Write($"{string.Join(",", enumerable.Cast<object>())}\t");
                }
                else
                {
                    Console.Write($"{value}\t");
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
