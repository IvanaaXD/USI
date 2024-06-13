using LangLang.Controller;
using LangLang.Domain.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Domain.Model.Reports
{
    public class SecondReportGenerator : IReportGenerator
    {

        readonly IStudentGradeRepository _studentGrades = Injector.CreateInstance<IStudentGradeRepository>();
        readonly ICourseGradeRepository _courseGrade = Injector.CreateInstance<ICourseGradeRepository>();
        readonly CourseController _courseController = Injector.CreateInstance<CourseController>();
        readonly DirectorController _directorController = Injector.CreateInstance<DirectorController>();
          
        public void GenerateReport()
        {
            PdfGenerator pdfGenerator = new PdfGenerator("..\\..\\..\\Data\\report2.pdf");
            pdfGenerator.AddTitle("Average teacher and course grades in the past year");
            pdfGenerator.AddNewLine();

            pdfGenerator.AddTupleTable(GetTeacherCourseReport(), "Course", "Teacher Grade", "Knowledge Grade", "Activity Grade");
            pdfGenerator.SaveAndClose(); SendEmail();

        }
        private void SendEmail()
        {
            EmailSender emailSender = new EmailSender("smtp.gmail.com", 587, "diirrektorr@gmail.com", "dvwa dbkw bzyl cauy");
            emailSender.SendEmail("diirrektorr@gmail.com", "diirrektorr@gmail.com", "Report 2", "Report 2 body",
                                    "..\\..\\..\\Data\\report2.pdf");
        }

        public Dictionary<Course, (double, double, double)> GetTeacherCourseReport()
        {
            Dictionary<Course, (double, double, double)> finalCourses = new();
            Dictionary<Course, double> averageTeacherGrade = GetAverageTeacherGradeByCourse();
            Dictionary<Course, double> averageKnowledgeGrade = CalculateAverageGrade("knowledge");
            Dictionary<Course, double> averageActivityGrade = CalculateAverageGrade("activity");
            List<Course> lastYearCourses = _courseController.GetCoursesLastYear();

            foreach (Course course in lastYearCourses)
            {
                if (_courseController.HasGradingPeriodStarted(course))
                {
                    finalCourses[course] = (averageTeacherGrade[course], averageKnowledgeGrade[course], averageActivityGrade[course]);
                }
            }

            return finalCourses;
        }
        public Dictionary<Course, double> GetAverageTeacherGradeByCourse()
        {
            Dictionary<Course, double> finalResult = new();
            foreach (Course course in _courseController.GetCoursesLastYear())
            {
                int result = 0;
                Teacher? teacher = _directorController.GetTeacherByCourse(course.Id);
                if (teacher == null)
                    continue;
                List<StudentGrade> teachersGrades = _studentGrades.GetStudentGradesByTeacherCourse(teacher.Id, course.Id);

                foreach (StudentGrade studentGrade in teachersGrades)
                    result += studentGrade.Value;

                if (teachersGrades.Count == 0)
                    finalResult[course] = 0;
                else
                    finalResult[course] = result / teachersGrades.Count;

            }
            return finalResult;
        }
        public Dictionary<Course, double> CalculateAverageGrade(string typeOfGrade)
        {
            Dictionary<Course, double> finalResult = new();
            foreach (Course course in _courseController.GetCoursesLastYear())
            {
                int result = 0;

                List<CourseGrade> studentGrades = _courseGrade.GetCourseGradesByCourse(course.Id);

                foreach (CourseGrade grade in studentGrades)
                {
                    if (typeOfGrade == "knowledge")
                        result += grade.StudentKnowledgeValue;
                    else
                        result += grade.StudentActivityValue;
                }
                if (result == 0)
                    finalResult[course] = 0;
                else
                    finalResult[course] = result / studentGrades.Count;
            }
            return finalResult;
        }
    }
}
