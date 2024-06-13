using System;
using LangLang.Controller;
using LangLang.Domain.Model;
using LangLang.View.Director;
using System;
using System.Reflection;

public class CRUDConsole
{
    public static void Display<T>(Person person)
    {
        while (true)
        {
            Console.WriteLine("Choose an entity type:");

            if (person.GetType() == typeof(Director))
                Console.WriteLine("Teacher (t)");

            Console.WriteLine("Course (c)");
            Console.WriteLine("ExamTerm (e)");
            Console.WriteLine("Exit (x)");

            string entityType = Console.ReadLine().ToLower();

            switch (entityType)
            {
                case "t":
                    if (person.GetType() == typeof(Director))
                        DisplayCrudOperations<Teacher>(person);
                    else
                        Console.WriteLine("Invalid choice.");
                    break;
                case "c":
                    DisplayCrudOperations<Course>(person);
                    break;
                case "e":
                    DisplayCrudOperations<ExamTerm>(person);
                    break;
                case "x":
                    return;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }

    public static void DisplayCrudOperations<T>(Person person) where T : new()
    {
        GenericCrud crud = new GenericCrud();
        dynamic controller = GetControllerByModelType(typeof(T));

        while (true)
        {
            Console.WriteLine("Choose an operation: Create (c), Read (r), Update (u), Delete (d), Exit (e)");
            string operation = Console.ReadLine().ToLower();

            switch (operation)
            {
                case "c":
                    CreateItem<T>(crud, person, controller);
                    break;
                case "r":
                    ReadItem<T>(crud, person, controller);
                    break;
                case "u":
                    UpdateItem<T>(crud, person, controller);
                    break;
                case "d":
                    DeleteItem<T>(crud, person, controller);
                    break;
                case "e":
                    return;
                default:
                    Console.WriteLine("Invalid operation.");
                    break;
            }
        }
    }

    private static void CreateItem<T>(GenericCrud crud, Person person, dynamic controller) where T : new()
    {
        T newItem = crud.Create<T>();

        Console.WriteLine("Item created: ");
        crud.Read(newItem);

        AddCheckType(newItem, person);
        controller.Add(newItem);
    }

    private static void ReadItem<T>(GenericCrud crud, Person person, dynamic controller) where T : new()
    {
        Console.Write("Enter ID of item to read: ");

        if (int.TryParse(Console.ReadLine(), out int readId))
        {
            T itemToRead = new T();
            itemToRead = controller.GetById(readId);
            crud.Read(itemToRead);
        }
        else
            Console.WriteLine("Invalid input.");
    }

    private static void UpdateItem<T>(GenericCrud crud, Person person, dynamic controller) where T : new()
    {
        Console.Write("Enter ID of item to update: ");

        if (int.TryParse(Console.ReadLine(), out int updateId))
        {
            T itemToUpdate = new T();
            T updatedItem = crud.Update(itemToUpdate);
            controller.Update(updatedItem);

            Console.WriteLine("Item updated:");
            crud.Read(updatedItem);
        }
        else
            Console.WriteLine("Invalid input.");
    }

    private static void DeleteItem<T>(GenericCrud crud, Person person, dynamic controller) where T : new()
    {
        Console.Write("Enter ID of item to delete: ");
        if (int.TryParse(Console.ReadLine(), out int deleteId))
        {
            T itemToDelete = new T();
            controller.GetById(deleteId);
            controller.Delete(itemToDelete);
            Console.WriteLine("Item deleted.");
        }
        else
            Console.WriteLine("Invalid input.");
    }
    /*
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
    */

    private static object GetControllerByModelType(Type type)
    {
        object controller = null;
        switch (type.Name)
        {
            case nameof(Teacher):
                controller = Injector.CreateInstance<DirectorController>();
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

    private static void AddCheckType<T>(T item, Person person)
    {
        if (person.GetType() == typeof(Teacher))
        {
            DirectorController controller = Injector.CreateInstance<DirectorController>();
            Teacher teacher = controller.GetById(person.Id);

            if (item.GetType() == typeof(ExamTerm))
            {
                ExamTerm examTerm = item as ExamTerm;
                teacher.CoursesId.Add(examTerm.ExamID);
            }
            else if (item.GetType() == typeof(Course))
            {
                Course course = item as Course;
                teacher.ExamsId.Add(course.Id);
            }

            controller.Update(teacher);
        }
    }
}
