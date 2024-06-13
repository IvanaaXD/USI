using System;
using LangLang.Controller;
using LangLang.Domain.Model;
using System.Reflection;
using LangLang.DTO;
using PdfSharp.Snippets.Font;

public class CRUDConsole
{
    static dynamic controller;

    public static void Display(Person person)
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
        controller = GetControllerByModelType(typeof(T));

        while (true)
        {
            Console.WriteLine("Choose an operation: Create (c), Read (r), Update (u), Delete (d), Exit (x)");
            string operation = Console.ReadLine().ToLower();

            switch (operation)
            {
                case "c":
                    CreateObject<T>(crud, person);
                    break;
                case "r":
                    ReadItem<T>(crud, person);
                    break;
                case "u":
                    UpdateItem<T>(crud, person);
                    break;
                case "d":
                    DeleteItem<T>(crud, person);
                    break;
                case "e":
                    return;
                default:
                    Console.WriteLine("Invalid operation.");
                    break;
            }
        }
    }

    private static void CreateItem<T>(GenericCrud crud, Person person) where T : new()
    {
        T newItem = crud.Create<T>();

        Console.WriteLine("Item created: ");
        crud.Read(newItem);

        AddCheckType(newItem, person);
        controller.Add(newItem);
    }

    private static void ReadItem<T>(GenericCrud crud, Person person) where T : new()
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

    private static void UpdateItem<T>(GenericCrud crud, Person person) where T : new()
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

    private static void DeleteItem<T>(GenericCrud crud, Person person) where T : new()
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
    
    private static void CreateObject<T>(GenericCrud crud, Person person) where T : new()
    {
        T newItem = crud.Create<T>();
        Console.WriteLine("Item created:");
        crud.Read(newItem); 

        MethodInfo addMethod = controller.GetType().GetMethod("Add");
        if (addMethod != null)
        {
            addMethod.Invoke(controller, new object[] { newItem });
            Console.WriteLine($"{typeof(T).Name} added successfully.");
            AddCheckType(newItem, person);
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
            AddToTeacher(item, person);
        else
            AddToDirector(item);
    }
    private static void AddToDirector<T>(T item)
    {
        Director director = controller.GetDirector();

        if (item.GetType() == typeof(ExamTerm))
        {
            ExamTerm examTerm = item as ExamTerm;
            director.CoursesId.Add(examTerm.ExamID);
        }
        else if (item.GetType() == typeof(Course))
        {
            Course course = item as Course;
            director.ExamsId.Add(course.Id);
        }

        controller.Update(director);
    }

    private static void AddToTeacher<T>(T item, Person person)
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
