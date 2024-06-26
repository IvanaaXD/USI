using LangLang.Controller;
using LangLang.Domain.IRepository;
using System;
using System.Collections.Generic;

namespace LangLang.Domain.Model.Reports
{
    public class ThirdReportGenerator : IReportGenerator
    {
        CourseController _courseController = Injector.CreateInstance<CourseController>();
        IExamTermDbRepository _examTerms = Injector.CreateInstance<IExamTermDbRepository>();
        ExamTermGradeController _examTermGradeController = Injector.CreateInstance<ExamTermGradeController>();
        ICourseGradeRepository _courseGrade = Injector.CreateInstance<ICourseGradeRepository>();

        public void GenerateReport()
        {
            PdfGenerator pdfGenerator = new PdfGenerator("..\\..\\..\\Data\\report3.pdf");
            pdfGenerator.AddTitle("Statistics on the points of passed exams in the last year");
            pdfGenerator.AddNewLine();

            pdfGenerator.AddTable(GetPartsOfExamReport(), "Each part of exam", "Average points");

            pdfGenerator.AddNewLine();
            pdfGenerator.AddTitle("Course statistics in the last year");
            pdfGenerator.AddNewLine();

            pdfGenerator.AddDifTypeTupleTable(GetStudentsCourseReport(), "Course", "Participants", "Passed", "Success Rate");

            pdfGenerator.SaveAndClose();
            SendEmail();
        }
        private void SendEmail()
        {
            EmailSender emailSender = new EmailSender("smtp.gmail.com", 587, "diirrektorr@gmail.com", "dvwa dbkw bzyl cauy");
            emailSender.SendEmail("diirrektorr@gmail.com", "diirrektorr@gmail.com", "Report 3", "Report 3 body",
                                    "..\\..\\..\\Data\\report3.pdf");
        }
        public Dictionary<string, double> GetPartsOfExamReport()
        {
            Dictionary<string, double> examAverageResult = new();
            examAverageResult["reading"] = CalculateAveragePoints("reading");
            examAverageResult["listening"] = CalculateAveragePoints("listening");
            examAverageResult["speaking"] = CalculateAveragePoints("speaking");
            examAverageResult["writing"] = CalculateAveragePoints("writing");
            return examAverageResult;
        }
        public Dictionary<Course, (int, int, double)> GetStudentsCourseReport()
        {
            Dictionary<Course, (int, int, double)> finalCourses = new();
            List<Course> lastYearCourses = _courseController.GetCoursesLastYear();

            foreach (Course course in lastYearCourses)
            {
                if (_courseController.HasGradingPeriodStarted(course))
                {
                    int attendedCount = GetAttendedCount(course.Id);
                    int passedCount = GetPassedCount(course.Id);
                    finalCourses[course] = (attendedCount, passedCount, CalculatePassPercentage(passedCount, attendedCount));
                }
            }

            return finalCourses;
        }
        public double CalculateAveragePoints(string typeOfPoints)
        {
            int result = 0, count = 0;
            List<ExamTermGrade> examGrades = _examTermGradeController.GetAllExamTermGrades();
            foreach (ExamTermGrade grade in examGrades)
            {
                ExamTerm exam = _examTerms.GetById(grade.ExamId);
                if (exam == null)
                    continue;
                else if (exam.ExamTime >= DateTime.Now.AddYears(-1))
                {
                    if (typeOfPoints == "listening")
                        result += grade.ListeningPoints;
                    else if (typeOfPoints == "speaking")
                        result += grade.SpeakingPoints;
                    else if (typeOfPoints == "writing")
                        result += grade.WritingPoints;
                    else if (typeOfPoints == "reading")
                        result += grade.ReadingPoints;

                    count++;
                }
            }
            return result == 0 ? 0 : result / count;
        }
        public int GetAttendedCount(int courseId)
        {
            Course course = _courseController.GetById(courseId);
            return course.CurrentlyEnrolled;
        }

        public int GetPassedCount(int courseId)
        {
            int count = 0;
            List<CourseGrade> grades = _courseGrade.GetCourseGradesByCourse(courseId);
            foreach (CourseGrade grade in grades)
            {
                if (grade.StudentKnowledgeValue >= 6 && grade.StudentActivityValue >= 6)
                    count++;
            }
            return count;
        }
        public double CalculatePassPercentage(int passedCount, int attendedCount)
        {
            if (attendedCount == 0)
            {
                return 0;
            }
            return (double)passedCount / attendedCount * 100;
        }
    }
}
