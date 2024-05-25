using LangLang.Domain.IRepository;
using LangLang.Domain.Model;
using LangLang.Repository;
using System;
using System.Collections.Generic;
using LangLang.Domain.Model.Enums;
using System.Linq;

namespace LangLang.Controller
{
    public class ReportController
    {
        private readonly IDirectorRepository _directors;
        private readonly TeacherRepository? _teachers;
        private readonly ExamTermDAO? _examTerms;
        private readonly StudentGradeRepository? _studentGrades;
        private readonly PenaltyPointDAO? _penaltyPoints;
        private readonly CourseController? _courseController;
        private readonly CourseGradeRepository? _courseGrade;
        private readonly DirectorController? _directorController;
        private readonly ExamTermController? _examTermController;
        private readonly ExamTermGradeRepository? _examTermGrades;

        public ReportController()
        {
            _directors = Injector.CreateInstance<IDirectorRepository>();
            _teachers = new TeacherRepository();
            _examTerms = new ExamTermDAO();
            _studentGrades = new StudentGradeRepository();
            _penaltyPoints = new PenaltyPointDAO();
            _examTermGrades = new ExamTermGradeRepository();

            _examTermController = new ExamTermController(new ExamTermRepository(), new TeacherController());
            _courseController = new CourseController(new CourseRepository(), new TeacherController());
        }

        public void GenerateFirstReport()
        {
            StudentsController studentController = Injector.CreateInstance<StudentsController>();
            PdfGenerator pdfGenerator = new PdfGenerator("..\\..\\..\\Data\\report1.pdf");
            pdfGenerator.AddTitle("Number of penalty points in the last year");
            pdfGenerator.AddNewLine();
            pdfGenerator.AddTable(_courseController.GetPenaltyPointsLastYearPerCourse(), "Course", "Penalties");
            pdfGenerator.AddNewPage();
            pdfGenerator.AddTitle("Average points of students by penalties");
            pdfGenerator.AddNewLine();
            for (int i = 0; i <= 3; i++)
            {
                pdfGenerator.AddSubtitle("Number of penalty points: " + i);
                pdfGenerator.AddTable(studentController.GetStudentsAveragePointsPerPenalty()[i], "Student", "Average points");
            }
            pdfGenerator.SaveAndClose();
        }
        public void GenerateSecondReport()
        {
            PdfGenerator pdfGenerator = new PdfGenerator("..\\..\\..\\Data\\report2.pdf");
            pdfGenerator.AddTitle("Average teacher and course grades in the past year");
            pdfGenerator.AddNewLine();

            pdfGenerator.AddTupleTable(GetTeacherCourseReport(), "Course", "Teacher Grade", "Knowledge Grade", "Activity Grade");
            pdfGenerator.SaveAndClose();
        }
        public void GenerateThirdReport()
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
        }

        public void GenerateFourthReport()
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
                finalCourses[course] = (averageTeacherGrade[course], averageKnowledgeGrade[course], averageActivityGrade[course]);
            }
            return finalCourses;
        }
        public Dictionary<string, double> GetPartsOfExamReport()
        {
            Dictionary<string, double> examAverageResult = new();
            examAverageResult["reading"] = GetAverageReadingPointsLastYear();
            examAverageResult["listening"] = GetAverageListeningPointsLastYear();
            examAverageResult["speaking"] = GetAverageSpeakingPointsLastYear();
            return examAverageResult;
        }
        public Dictionary<Course, (int, int, double)> GetStudentsCourseReport()
        {
            Dictionary<Course, (int, int, double)> finalCourses = new();
            List<Course> lastYearCourses = _courseController.GetCoursesLastYear();
            foreach (Course course in lastYearCourses)
            {
                int attendedCount = GetAttendedCount(course.Id);
                int passedCount = GetPassedCount(course.Id);
                finalCourses[course] = (attendedCount, passedCount, CalculatePassPercentage(passedCount, attendedCount));
            }
            return finalCourses;
        }
        public Dictionary<Language, int> GetLanguagesInt()
        {
            Dictionary<Language, int> languages = new Dictionary<Language, int>();
            var langs = Enum.GetValues(typeof(Language)).Cast<Language>().ToList();

            foreach (Language language in langs)
            {
                if (language != Language.NULL)
                    languages.Add(language, 0);
            }

            return languages;
        }

        public Dictionary<Language, double> GetLanguagesDouble()
        {
            Dictionary<Language, double> languages = new Dictionary<Language, double>();
            var langs = Enum.GetValues(typeof(Language)).Cast<Language>().ToList();

            foreach (Language language in langs)
            {
                if (language != Language.NULL)
                    languages.Add(language, 0);
            }

            return languages;
        }

        public Dictionary<LanguageLevel, int> GetLanguageLevels()
        {
            Dictionary<LanguageLevel, int> levels = new Dictionary<LanguageLevel, int>();
            var languages = Enum.GetValues(typeof(LanguageLevel)).Cast<LanguageLevel>().ToList();

            foreach (LanguageLevel language in languages)
            {
                levels.Add(language, 0);
            }

            return levels;
        }

        public Dictionary<Language, int> GetNumberOfCourses()
        {
            Dictionary<Language, int> numberOfCourses = GetLanguagesInt();
            var courses = _courseController.FindCoursesByDate(DateTime.Today.AddYears(-1));

            foreach (var course in courses)
                numberOfCourses[course.Language] += 1;

            return numberOfCourses;
        }

        public Dictionary<Language, int> GetNumberOfExamTerms()
        {
            Dictionary<Language, int> numberOfExamTerms = GetLanguagesInt();
            var examTerms = _examTermController.FindExamTermsByDate(DateTime.Today.AddYears(-1));

            foreach (var examTerm in examTerms)
            {
                var course = _teachers.GetCourseById(examTerm.CourseID);
                numberOfExamTerms[course.Language] += 1;
            }

            return numberOfExamTerms;
        }

        public Dictionary<Language, double> GetNumberOfPenaltyPoints()
        {
            Dictionary<Language, double> numberOfPenaltyPoints = GetLanguagesDouble();

            var penaltyPoints = _penaltyPoints.GetAllPenaltyPoints();
            if (penaltyPoints.Count == 0)
                return numberOfPenaltyPoints;

            foreach (var number in numberOfPenaltyPoints)
            {
                List<LanguageLevel> levels = new List<LanguageLevel>();
                int sum = 0;

                foreach (var penaltyPoint in penaltyPoints)
                {
                    var course = _teachers.GetCourseById(penaltyPoint.CourseId);

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
            Dictionary<Language, double> numberOfPoints = GetLanguagesDouble();

            var examTerms = _examTerms.GetAllExamTerms();

            foreach (var number in numberOfPoints)
            {
                List<LanguageLevel> levels = new List<LanguageLevel>();
                int sum = 0;

                foreach (var examTerm in examTerms)
                {
                    var course = _teachers.GetCourseById(examTerm.CourseID);
                    var grades = _examTermGrades.GetExamTermGradeByExam(examTerm.ExamID);

                    if (course.Language == number.Key)
                    {
                        if (!levels.Contains(course.Level))
                            levels.Add(course.Level);

                        foreach (var grade in grades)
                        {
                            sum += grade.ListeningPoints + grade.ReadingPoints + grade.SpeakingPoints + grade.WritingPoints;
                        }
                    }
                }

                double averageNumber = 0;
                if (sum != 0)
                    averageNumber = sum / levels.Count();

                numberOfPoints[number.Key] = averageNumber;
            }

            return numberOfPoints;
        }

        public double GetAverageReadingPointsLastYear()
        {
            return CalculateAveragePoints("reading");
        }
        public double GetAverageSpeakingPointsLastYear()
        {
            return CalculateAveragePoints("speaking");
        }
        public double GetAverageWritingPointsLastYear()
        {
            return CalculateAveragePoints("writing");
        }
        public double GetAverageListeningPointsLastYear()
        {
            return CalculateAveragePoints("listening");
        }
        public double CalculateAveragePoints(string typeOfPoints)
        {
            int result = 0, count = 0;
            List<ExamTermGrade> examGrades = _examTermGrades.GetAllExamTermGrades();
            foreach (ExamTermGrade grade in examGrades)
            {
                ExamTerm exam = _examTerms.GetExamTermById(grade.ExamId);
                if (exam == null)
                {
                    continue;
                }
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
        public Dictionary<Course, double> GetAverageTeacherGradeByCourse()
        {
            Dictionary<Course, double> finalResult = new();
            foreach (Course course in _courseController.GetCoursesLastYear())
            {
                int result = 0;

                Teacher teacher = _directorController.GetTeacherByCourse(course.Id);

                List<StudentGrade> teachersGrades = _studentGrades.GetStudentGradesByTeacherCourse(teacher.Id, course.Id);

                foreach (StudentGrade studentGrade in teachersGrades)
                {
                    result += studentGrade.Value;
                }
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
        public int GetAttendedCount(int courseId)
        {
            Course course = _teachers.GetCourseById(courseId);
            return course.CurrentlyEnrolled;
        }
        public int GetPassedCount(int courseId)
        {
            int count = 0;
            Course course = _teachers.GetCourseById(courseId);
            List<ExamTermGrade> grades = _examTermGrades.GetAllExamTermGrades();
            foreach (ExamTermGrade grade in grades)
            {
                if (course.ExamTerms.Contains(grade.ExamId) && grade.Value >= 6)
                {
                    count++;
                }
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
