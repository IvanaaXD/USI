using System;
using System.Reflection;
using ConsoleLangLang.DTO;
using LangLang.Controller;
using LangLang.Domain.Model;

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
                        continue;
                    // DisplayCrudOperations<TeacherDTO>(person);
                    else
                        Console.WriteLine("Invalid choice.");
                    break;
                case "c":
                    // DisplayCrudOperations<CourseDTO>(person);
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
        TDto instance = new TDto();

        MethodInfo toModelClassMethod = typeof(TDto).GetMethod("ToModelClass");
        var modelObject = toModelClassMethod.Invoke(instance, null);
        Type modelType = modelObject.GetType();
        controller = GetControllerByModelType(modelType);

        while (true)
        {
            Console.WriteLine("Choose an operation: Create (c), Read (r), Update (u), Delete (d), Exit (x)");
            string operation = Console.ReadLine().ToLower();

            switch (operation)
            {
                case "c":
                    CreateObject<TDto>(crud, person);
                    break;
                case "r":
                    ReadItem<TDto>(crud, person);
                    break;
                case "u":
                    UpdateItem<TDto>(crud, person);
                    break;
                case "d":
                    DeleteItem<TDto>(crud, person);
                    break;
                case "x":
                    return;
                default:
                    Console.WriteLine("Invalid operation.");
                    break;
            }
        }
    }

    private static void CreateObject<TDto>(GenericCrud crud, Person person) where TDto : new()
    {
        TDto newItem = crud.Create<TDto>();
        Console.WriteLine("Item created:");
        crud.Read(newItem); 
        MethodInfo addMethod = controller.GetType().GetMethod("Add");

        if (addMethod != null)
        {
            MethodInfo toModelMethod = typeof(TDto).GetMethod("ToModelClass");
            if (toModelMethod != null)
            {
                var modelItem = toModelMethod.Invoke(newItem, null);
                addMethod.Invoke(controller, new object[] { modelItem });
                Console.WriteLine($"{typeof(TDto).Name} added successfully.");
                AddCheckType(newItem, person);
            }
            else
                Console.WriteLine($"ToModelClass method not found on DTO type: {typeof(TDto).Name}");
        }
        else
            Console.WriteLine($"Add method not found on controller for entity type: {typeof(TDto).Name}");
    }

    private static void ReadItem<TDto>(GenericCrud crud, Person person) where TDto : new()
    {
        Console.Write("Enter ID of item to read: ");
        MethodInfo getByIdMethod = controller.GetType().GetMethod("GetById");

        if (int.TryParse(Console.ReadLine(), out int readId))
        {
            var modelToRead = getByIdMethod.Invoke(controller, new object[] { readId });
            TDto newItem = new TDto();
            MethodInfo toDtoMethod = typeof(TDto).GetMethod("ToDTO");

            newItem = (TDto)toDtoMethod.Invoke(newItem, new object[] { modelToRead });
            crud.Read(newItem);
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid integer ID.");
        }
    }


    private static GenericCrud.Converter<TDto, object> GetDtoToModelConverter<TDto>() where TDto : new()
    {
        return dto =>
        {
            MethodInfo toModelMethod = typeof(TDto).GetMethod("ToModelClass", Type.EmptyTypes);
            if (toModelMethod == null)
                throw new InvalidOperationException($"ToModelClass method not found on type {typeof(TDto).Name}");
            return toModelMethod.Invoke(dto, null);
        };
    }

    private static GenericCrud.Converter<object, TDto> GetModelToDtoConverter<TModel, TDto>() where TDto : new()
    {
        return model =>
        {
            if (!(model is TModel))
                throw new ArgumentException($"Expected model of type {typeof(TModel).Name}, but got {model.GetType().Name}");

            TModel modelObj = (TModel)model;
            TDto dtoInstance = new TDto();
            MethodInfo toDtoMethod = typeof(TDto).GetMethod("ToDTO", Type.EmptyTypes);
            //MethodInfo toModelMethod = typeof(TDto).GetMethod("ToModelClass", Type.EmptyTypes);

            if (toDtoMethod == null)
                throw new InvalidOperationException($"ToDTO method not found on type {typeof(TDto).Name} that accepts {typeof(TModel).Name} as parameter.");
            return (TDto)toDtoMethod.Invoke(dtoInstance, new object[] { modelObj });
        };
    }



    private static void UpdateItem<TDto>(GenericCrud crud, Person person) where TDto : new()
    {
        Console.Write("Enter ID of item to update: ");

        if (int.TryParse(Console.ReadLine(), out int updateId))
        {
            TDto itemToUpdate = controller.GetById<TDto>(updateId); 
            TDto updatedItem = crud.Update(itemToUpdate);
            controller.Update(updatedItem);

            Console.WriteLine("Item updated:");
            var converter = GetDtoToModelConverter<TDto>();
            crud.Read(updatedItem, converter); MethodInfo addMethod = controller.GetType().GetMethod("Add");
        }
        else
            Console.WriteLine("Invalid input.");
    }

    private static void DeleteItem<TDto>(GenericCrud crud, Person person) where TDto : new()
    {
        Console.Write("Enter ID of item to delete: ");
        if (int.TryParse(Console.ReadLine(), out int deleteId))
        {
            TDto itemToDelete = controller.GetById<TDto>(deleteId); // Assuming GetById<TDto> is defined in the controller
            controller.Delete(itemToDelete);
            Console.WriteLine("Item deleted.");
        }
        else
            Console.WriteLine("Invalid input.");
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
        DirectorController directorController = controller as DirectorController;
        Director director = directorController.GetDirector();

        if (item is ExamTermDTO)
        {
            ExamTermDTO examTermDTO = (ExamTermDTO)(object)item;
            ExamTerm examTerm = examTermDTO.ToModelClass(); // Assuming ToModelClass() is defined on ExamTermDTO
            director.ExamsId.Add(examTerm.ExamID);
        }
        else if (item is CourseDTO)
        {
            CourseDTO courseDTO = (CourseDTO)(object)item;
            //Course course = courseDTO.ToModelClass<Course>(); // Assuming ToModelClass<T>() is defined on CourseDTO
            //director.CoursesId.Add(course.Id);
        }

        directorController.Update(director);
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
            /*Course course = item.ToModelClass<Course>(); 
            teacher.ExamsId.Add(course.Id);*/
        }

        controller.Update(teacher);
    }
}
