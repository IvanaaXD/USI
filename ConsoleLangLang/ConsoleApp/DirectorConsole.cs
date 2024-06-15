using LangLang.Controller;
using LangLang.Domain.Model;
using System;
using System.Collections.Generic;
using System.Windows;

namespace LangLang.ConsoleApp
{
    public class DirectorConsole
    {
        private static Director director;
        private static DirectorController directorController = Injector.CreateInstance<DirectorController>();
        private static CourseController courseController = Injector.CreateInstance<CourseController>();
        public static void Display()
        {
            director = directorController.GetDirector();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Choose an operation:\n" +
                                  "\t1) CRUD operations\n" +
                                  "\t2) Smart selection of course teacher\n" +
                                  "\tx) Exit");

                string operation = Console.ReadLine().ToLower();

                switch (operation)
                {
                    case "1":
                        CRUDConsole.Display(director);
                        break;
                    case "2":
                        SmartSelectionOfCourseTeacher();
                        break;
                    case "x":
                        return;
                    default:
                        Console.WriteLine("Invalid operation.");
                        break;
                }
            }
        }
        private static void SmartSelectionOfCourseTeacher()
        {
            GenericCrud crud = new GenericCrud();
            crud.PrintTable(GetCoursesWithoutTeacher());

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Choose an operation:\n" +
                                  "\t1) Enter the course id\n" +
                                  "\tx) Exit\n");

                string operation = Console.ReadLine().ToLower();

                switch (operation)
                {
                    case "1":
                        int courseId;
                        if (Int32.TryParse(Console.ReadLine(), out courseId))
                            AssignTeacher(courseController.GetById(courseId));
                        break;
                    case "x":
                        return;
                    default:
                        Console.WriteLine("Invalid operation.");
                        break;
                }
            }
        }
        private static List<Course> GetCoursesWithoutTeacher()
        {
            var coursesId = director.CoursesId;
            var courses = courseController.GetAllCourses();
            var filteredCourses = new List<Course>();

            if (coursesId != null)
            {
                foreach (Course course in courses)
                {
                    if (coursesId.Contains(course.Id))
                    {
                        Course courseDTO = new Course();
                        // CourseDTO courseDTO = new CourseDTO(course);    OVO TREBA DA RADI
                        Domain.Model.Teacher? courseTeacher = directorController.GetTeacherByCourse(course.Id);
                        if (courseTeacher == null)
                            filteredCourses.Add(course);
                    }
                }
            }

            return filteredCourses;
        }
        private static void AssignTeacher(Course course)
        {
            int teacherCourseId = directorController.FindMostAppropriateTeacher(course);
            if (teacherCourseId != -1)
            {
                Domain.Model.Teacher teacher = directorController.GetById(teacherCourseId);
                teacher.CoursesId.Add(course.Id);
                directorController.Update(teacher);
                Console.WriteLine($"{teacher.FirstName} {teacher.LastName} was chosen");
            }
            else
                MessageBox.Show("There is no available teacher for that course");
        }
    }
}
