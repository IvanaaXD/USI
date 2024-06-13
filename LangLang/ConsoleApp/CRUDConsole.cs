using LangLang.Controller;
using LangLang.Domain.Model;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using static iText.IO.Util.IntHashtable;
using System.Reflection;

public class CRUDConsole
{
    public static void Display()
    {
        while (true)
        {
            Console.WriteLine("Choose an entity type: Teacher (t), Course (c), ExamTerm (e), Exit (x)");
            string entityType = Console.ReadLine().ToLower();

            switch (entityType)
            {
                case "t":
                    DisplayCrudOperations<Teacher>();
                    break;
                case "c":
                    DisplayCrudOperations<Course>();
                    break;
                case "e":
                    DisplayCrudOperations<ExamTerm>();
                    break;
                case "x":
                    return;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }

    public static void DisplayCrudOperations<T>() where T : new()
    {
        GenericCrud crud = new GenericCrud();
        object controller = GetControllerByModelType(typeof(T));

        while (true)
        {
            Console.WriteLine("Choose an operation: Create (c), Read (r), Update (u), Delete (d), Exit (e)");
            string operation = Console.ReadLine().ToLower();

            switch (operation)
            {
                case "c":
                   /* T newItem = crud.Create<T>();
                    // Optionally, you can save the newItem to your data store or perform other operations
                    Console.WriteLine("Item created:");
                    crud.Read(newItem); // Display the created item*/
                    
                    CreateObject<T>(crud,controller);
                    break;
                case "r":
                    Console.Write("Enter ID of item to read: ");
                    if (int.TryParse(Console.ReadLine(), out int readId))
                    {
                        // Here you would fetch the item from your data store based on readId
                        // For simplicity, I'll create a new instance of T and display it
                        T itemToRead = new T(); // Replace with logic to fetch from data store
                        crud.Read(itemToRead);
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;
                case "u":
                    Console.Write("Enter ID of item to update: ");
                    if (int.TryParse(Console.ReadLine(), out int updateId))
                    {
                        // Here you would fetch the item from your data store based on updateId
                        // For simplicity, I'll create a new instance of T and update it
                        T itemToUpdate = new T(); // Replace with logic to fetch from data store
                        T updatedItem = crud.Update(itemToUpdate);
                        // Optionally, you can save the updatedItem to your data store
                        Console.WriteLine("Item updated:");
                        crud.Read(updatedItem); // Display the updated item
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;
                case "d":
                    Console.Write("Enter ID of item to delete: ");
                    if (int.TryParse(Console.ReadLine(), out int deleteId))
                    {
                        // Here you would fetch the item from your data store based on deleteId
                        // For simplicity, I'll create a new instance of T and delete it
                        T itemToDelete = new T(); // Replace with logic to fetch from data store
                        // Perform delete operation
                        Console.WriteLine("Item deleted.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;
                case "e":
                    return;
                default:
                    Console.WriteLine("Invalid operation.");
                    break;
            }
        }
    }

    private static void CreateObject<T>(GenericCrud crud, object controller) where T : new()
    {
        T newItem = crud.Create<T>();
        Console.WriteLine("Item created:");
        crud.Read(newItem); 

        MethodInfo addMethod = controller.GetType().GetMethod("Add");
        if (addMethod != null)
        {
            addMethod.Invoke(controller, new object[] { newItem });
            Console.WriteLine($"{typeof(T).Name} added successfully.");
        }
        else
        {
            Console.WriteLine($"Add method not found on controller for entity type: {typeof(T).Name}");
        }
    }

    private static object GetControllerByModelType(Type type)
    {
        object controller = null;
        switch (type.Name)
        {
            case nameof(Teacher):
                controller = Injector.CreateInstance<TeacherController>();
                break;
            case nameof(Course):
                controller = Injector.CreateInstance<CourseController>();
                break;
            case nameof(ExamTerm):
                controller = Injector.CreateInstance<ExamTermController>();
                break;
            default:
                throw new ArgumentException("Unsupported entity type.");
        }

        if (controller == null)
            Console.WriteLine($"Controller not found for entity type: {type.Name}");

        return controller;
    }
}
