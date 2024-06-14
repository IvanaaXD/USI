using ConsoleLangLang.ConsoleApp;
using ConsoleLangLang.ConsoleApp.DTO;
using LangLang.Controller;
using LangLang.Domain.Model;
using System;
using System.Windows.Documents;
using LangLang.Domain.Model;
using System.Collections.Generic;
using System.Windows;
using LangLang.Migrations;

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
                Console.WriteLine("Choose an operation:\n" +
                                  "\t1) CRUD operations\n" +
                                  "\t2) Smart selection of course teacher\n" +
                                  "\t3) Smart selection of the teacher on the exam\n" +
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
                    case "3":
                        return;
                    case "4":
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
                Console.WriteLine("Choose an operation:\n" +
                                  "\t1) Enter the course id" +
                                  "\tx) Exit");

                string operation = Console.ReadLine().ToLower();

                switch (operation)
                {
                    case "1":
                        int courseId;
                        if (Int32.TryParse(Console.ReadLine(), out courseId))
                        {
                            AssignTeacher(courseController.GetById(courseId));
                        }
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
                            filteredCourses.Add(courseDTO);
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
            /*else
            {
                SelectedCourseDirector.HasTeacher = false;
                MessageBox.Show("There is no available teacher for that course");
            }*/
        }

    }
}
