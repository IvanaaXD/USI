using System;
using System.Reflection;
using System.Windows;
using ConsoleLangLang.ConsoleApp.DTO;
using ConsoleLangLang.DTO;
using LangLang.Controller;
using LangLang.Domain.Model;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

public class CRUDConsole
{
    static dynamic controller;

    public static void Display(Person person)
    {
        while (true)
        {
            Console.Clear();
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
                        DisplayCrudOperations<TeacherDTO>(person);
                    else
                        Console.WriteLine("Invalid choice.");
                    break;
                case "c":
                    DisplayCrudOperations<CourseDTO>(person);
                    break;
                case "e":
                    DisplayCrudOperations<ExamTermDTO>(person);
                    break;
                case "x":
                    return;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }

    public static void DisplayCrudOperations<TDto>(Person person) where TDto : new()
    {
        GenericCrud crud = new GenericCrud();
        var modelObject = ToModel(new TDto());
        controller = GetControllerByModelType(modelObject.GetType());
        bool isDirector = person is Director;
        bool isTeacher = modelObject.GetType() == typeof(Teacher);  

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Choose an operation: \nExit (x) \nCreate (c) \nRead (r) ");
            if (!isDirector || (isDirector && isTeacher))
                Console.WriteLine("Update (u) \nDelete (d)");

            string operation = Console.ReadLine().ToLower();

            switch (operation)
            {
                case "c":
                    CreateObject<TDto>(crud, person);
                    break;
                case "r":
                    ReadObject<TDto>(crud, person);
                    break;
                case "u":
                    if (!isDirector|| (isDirector && isTeacher))
                        UpdateObject<TDto>(crud, person);
                    else
                        Console.WriteLine("Update operation is not allowed for Directors.");
                    break;
                case "d":
                    if (!isDirector || (isDirector && isTeacher))
                        DeleteObject<TDto>(crud, person);
                    else
                        Console.WriteLine("Delete operation is not allowed for Directors.");
                    break;
                case "x":
                    return;
                default:
                    Console.WriteLine("Invalid operation.");
                    break;
            }
            Console.ReadLine();
        }
    }

    private static TDto ToDTO<TDto>(object modelToRead) where TDto : new()
    {
        TDto newItem = new TDto();
        MethodInfo toDtoMethod = typeof(TDto).GetMethod("ToDTO");

        if (toDtoMethod != null)
        {
            newItem = (TDto)toDtoMethod.Invoke(newItem, new object[] { modelToRead });
            return newItem;
        }
        else
            throw new InvalidOperationException($"Method 'ToModelClass' not found in type '{typeof(TDto).Name}'.");
    }

    private static object ToModel<TDto>(TDto item) where TDto : new()
    {
        MethodInfo toModelMethod = typeof(TDto).GetMethod("ToModelClass");
        if (toModelMethod != null)
        {
            var modelItem = toModelMethod.Invoke(item, null);
            return modelItem;
        }
        else
            throw new InvalidOperationException($"Method 'ToModelClass' not found in type '{typeof(TDto).Name}'.");
    }

    private static object GetById<TDto>()
    {
        Console.Write("Enter ID of item to read: ");
        string input = Console.ReadLine();

        if (int.TryParse(input, out int readId))
        {
            MethodInfo getByIdMethod = controller.GetType().GetMethod("GetById");
            var modelToRead = getByIdMethod.Invoke(controller, new object[] { readId });
            return modelToRead;
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid integer ID.");
            return null; 
        }
    }

    public static void CreateObject<TDto>(GenericCrud crud, Person person) where TDto : new()
    {
        Console.Clear();

        TDto newItem = crud.Create<TDto>();
        if (newItem == null)
            return;

        Console.WriteLine("Item created:");
        crud.Read(newItem);

        MethodInfo addMethod = controller.GetType().GetMethod("Add");

        if (addMethod != null)
        {
            var modelItem = ToModel(newItem);
            object returnedValue = addMethod.Invoke(controller, new object[] { modelItem });
            
            TDto returnedValueDTO = ToDTO<TDto>(returnedValue);

            if (typeof(TDto) != typeof(TeacherDTO))
                AddCheckType(returnedValueDTO, person);
        }
        else
            Console.WriteLine($"Add method not found on controller for entity type: {typeof(TDto).Name}");
    }

    private static void ReadObject<TDto>(GenericCrud crud, Person person) where TDto : new()
    {
        Console.Clear();

        var modelToRead = GetById<TDto>();
        TDto newItem = ToDTO<TDto>(modelToRead);
        crud.Read(newItem);

        Console.ReadLine();
    }

    private static void UpdateObject<TDto>(GenericCrud crud, Person person) where TDto : new()
    {
        Console.Clear();

        var modelToRead = GetById<TDto>();
        TDto updatedItem = ToDTO<TDto>(modelToRead);
        TDto updated = crud.Update(updatedItem);
        Console.WriteLine("Item updated:");
        crud.Read(updated);

        MethodInfo updateMethod = controller.GetType().GetMethod("Update");

        if (updateMethod != null)
        {
            var modelItem = ToModel(updated);
            updateMethod.Invoke(controller, new object[] { modelItem });
        }
        else
            Console.WriteLine($"Add method not found on controller for entity type: {typeof(TDto).Name}");
    }

    private static void DeleteObject<TDto>(GenericCrud crud, Person person) where TDto : new()
    {
        Console.Clear();

        var modelToRead = GetById<TDto>();
        TDto deletedItem = ToDTO<TDto>(modelToRead);

        MethodInfo deleteMethod = controller.GetType().GetMethod("Delete");

        if (deleteMethod != null)
        {
            deleteMethod.Invoke(controller, new object[] { modelToRead });
            Console.WriteLine("Item deleted.");
            DeleteCheckType(deletedItem, person);
        }
        else
            Console.WriteLine($"Add method not found on controller for entity type: {typeof(TDto).Name}");
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

    private static void AddCheckType<TDto>(TDto item, Person person)
    {
        if (person.GetType() == typeof(Teacher))
            AddToTeacher(item, person);
        else
            AddToDirector(item);
    }

    private static void AddToDirector<TDto>(TDto item)
    {
        DirectorController directorController = Injector.CreateInstance<DirectorController>();
        Director director = directorController.GetDirector();

        if (item is ExamTermDTO)
        {
            ExamTermDTO examTermDTO = (ExamTermDTO)(object)item;
            ExamTerm examTerm = examTermDTO.ToModelClass(); 
            director.ExamsId.Add(examTerm.ExamID);
            SmartSelectionOfExamTeacher(directorController,examTerm);
            
        }
        else if (item is CourseDTO)
        {
            CourseDTO courseDTO = (CourseDTO)(object)item;
            Course course = courseDTO.ToModelClass();
            director.CoursesId.Add(course.Id);
        }

        directorController.UpdateDirector(director);
    }

    private static void AddToTeacher<TDto>(TDto item, Person person)
    {
        DirectorController controller = Injector.CreateInstance<DirectorController>();
        Teacher teacher = controller.GetById(person.Id);

        if (item.GetType() == typeof(ExamTermDTO))
        {
            ExamTermDTO examTermDTO = (ExamTermDTO)(object)item;
            ExamTerm examTerm = examTermDTO.ToModelClass();
            teacher.CoursesId.Add(examTerm.ExamID);
        }
        else if (item.GetType() == typeof(CourseDTO))
        {
            CourseDTO courseDTO = (CourseDTO)(object)item;
            Course course = courseDTO.ToModelClass();
            teacher.CoursesId.Add(course.Id);
        }

        controller.Update(teacher);
    }

    private static void DeleteCheckType<TDto>(TDto item, Person person)
    {
        if (person.GetType() == typeof(Teacher))
            DeleteFromTeacher(item, person);
    }

    private static void DeleteFromTeacher<TDto>(TDto item, Person person)
    {
        DirectorController directorController = Injector.CreateInstance<DirectorController>();
        TeacherController teacherController = Injector.CreateInstance<TeacherController>();
        Teacher teacher = directorController.GetById(person.Id);

        if (item.GetType() == typeof(ExamTermDTO))
        {
            ExamTermDTO examTermDTO = (ExamTermDTO)(object)item;
            ExamTerm examTerm = examTermDTO.ToModelClass();
            teacherController.RemoveExamTerm(examTerm.ExamID);
        }
        else if (item.GetType() == typeof(CourseDTO))
        {
            CourseDTO courseDTO = (CourseDTO)(object)item;
            Course course = courseDTO.ToModelClass();
            controller.Delete(course.Id);
            directorController.RemoveCourseFromList(teacher.Id, course.Id);
            directorController.RemoveCourseFromDirector(course.Id);
        }
    }

    private static void SmartSelectionOfExamTeacher(DirectorController controller, ExamTerm examTerm)
    {
        int teacherCourseId = controller.FindMostAppropriateTeacher(examTerm);
        if (teacherCourseId > 0)
        {
            Teacher teacher = controller.GetById(teacherCourseId);
            teacher.ExamsId.Add(examTerm.ExamID);
            controller.Update(teacher);
            Console.WriteLine($"{teacher.FirstName} {teacher.LastName} was chosen");
        }
        else
            MessageBox.Show("There is no available teacher for that course");
    }
}
