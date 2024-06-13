using LangLang.Controller;
using LangLang.Domain.IRepository;
using LangLang.Domain.Model.Enums;
using LangLang.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Domain.Model.Reports
{
    public class FourthReportGenerator : IReportGenerator
    {
        CourseController _courseController = Injector.CreateInstance<CourseController>();
        ExamTermController _examTermController = Injector.CreateInstance<ExamTermController>();
        IExamTermRepository _examTerms = Injector.CreateInstance<IExamTermRepository>();
        IPenaltyPointRepository _penaltyPoints = Injector.CreateInstance<IPenaltyPointRepository>();
        ExamTermGradeController _examTermGradeController = Injector.CreateInstance<ExamTermGradeController>();

        public void GenerateReport()
        {
            PdfGenerator pdfGenerator = new PdfGenerator("..\\..\\..\\Data\\report4.pdf");

            pdfGenerator.AddTitle("Statistics on created courses in the last year");
            pdfGenerator.AddNewLine();
            pdfGenerator.AddTable(GetNumberOfCourses(), "Languages", "Number of courses");

            pdfGenerator.AddNewPage();

            pdfGenerator.AddTitle("Statistics on created exams in the last year");
            pdfGenerator.AddNewLine();
            pdfGenerator.AddTable(GetNumberOfExamTerms(), "Languages", "Number of exams");

            pdfGenerator.AddNewPage();

            pdfGenerator.AddTitle("Statistics on penalty points");
            pdfGenerator.AddNewLine();
            pdfGenerator.AddTable(GetNumberOfPenaltyPoints(), "Languages", "Average number of penalty points");

            pdfGenerator.AddNewPage();

            pdfGenerator.AddTitle("Statistics on exam points");
            pdfGenerator.AddNewLine();
            pdfGenerator.AddTable(GetNumberOfPoints(), "Languages", "Average number of points on exams");

            pdfGenerator.SaveAndClose();
            SendEmail();

        }
        private void SendEmail()
        {
            EmailSender emailSender = new EmailSender("smtp.gmail.com", 587, "diirrektorr@gmail.com", "dvwa dbkw bzyl cauy");
            emailSender.SendEmail("diirrektorr@gmail.com", "diirrektorr@gmail.com", "Report 4", "Report 4 body",
                                    "..\\..\\..\\Data\\report4.pdf");
        }
        public Dictionary<Language, T> GetLanguages<T>() where T : struct
        {
            Dictionary<Language, T> languages = new Dictionary<Language, T>();
            var langs = Enum.GetValues(typeof(Language)).Cast<Language>().ToList();

            foreach (Language language in langs)
                if (language != Language.NULL)
                    languages.Add(language, default(T));

            return languages;
        }
        public Dictionary<Language, int> GetNumberOfCourses()
        {
            Dictionary<Language, int> numberOfCourses = GetLanguages<int>();
            var courses = _courseController.FindCoursesByDate(DateTime.Today.AddYears(-1));

            foreach (var course in courses)
                numberOfCourses[course.Language] += 1;

            return numberOfCourses;
        }

        public Dictionary<Language, int> GetNumberOfExamTerms()
        {
            Dictionary<Language, int> numberOfExamTerms = GetLanguages<int>();
            var examTerms = _examTermController.FindExamTermsByDate(DateTime.Today.AddYears(-1));

            foreach (var examTerm in examTerms)
                numberOfExamTerms[examTerm.Language] += 1;

            return numberOfExamTerms;
        }

        public Dictionary<Language, double> GetNumberOfPenaltyPoints()
        {
            Dictionary<Language, double> numberOfPenaltyPoints = GetLanguages<double>();
            var penaltyPoints = _penaltyPoints.GetAllPenaltyPoints();

            if (penaltyPoints.Count == 0)
                return numberOfPenaltyPoints;

            foreach (var number in numberOfPenaltyPoints)
            {
                List<LanguageLevel> levels = new List<LanguageLevel>();
                int sum = 0;

                foreach (var penaltyPoint in penaltyPoints)
                {
                    var course = _courseController.GetById(penaltyPoint.CourseId);

                    if (course.Language == number.Key)
                    {
                        if (!levels.Contains(course.Level))
                            levels.Add(course.Level);
                        sum += 1;
                    }
                }

                double averageNumber = 0;
                if (sum != 0)
                    averageNumber = sum / levels.Count();

                numberOfPenaltyPoints[number.Key] = averageNumber;
            }
            return numberOfPenaltyPoints;
        }

        public Dictionary<Language, double> GetNumberOfPoints()
        {
            Dictionary<Language, double> numberOfPoints = GetLanguages<double>();
            var examTerms = _examTerms.GetAllExamTerms();

            foreach (var number in numberOfPoints)
            {
                List<LanguageLevel> levels = new List<LanguageLevel>();
                int sum = 0;

                foreach (var examTerm in examTerms)
                {
                    var grades = _examTermGradeController.GetExamTermGradeByExam(examTerm.ExamID);

                    if (examTerm.Language == number.Key)
                    {
                        if (!levels.Contains(examTerm.Level))
                            levels.Add(examTerm.Level);

                        foreach (var grade in grades)
                            sum += grade.ListeningPoints + grade.ReadingPoints + grade.SpeakingPoints + grade.WritingPoints;
                    }
                }

                double averageNumber = 0;
                if (sum != 0)
                    averageNumber = sum / levels.Count();

                numberOfPoints[number.Key] = averageNumber;
            }
            return numberOfPoints;
        }
    }
}
