using LangLang.Observer;
using System;
using System.Collections.Generic;
using LangLang.Repository;
using LangLang.Domain.Model;
using LangLang.Domain.Model.Enums;
using System.Linq;
using LangLang.Domain.IRepository;

namespace LangLang.Controller
{
    public class DirectorController
    {
        private readonly IDirectorRepository _directors;
        private readonly TeacherDAO? _teachers;
        private readonly StudentGradeDAO? _studentGrades;
        private readonly PenaltyPointDAO? _penaltyPoints;

        public DirectorController(IDirectorRepository directors)
        {
            _directors = directors ?? throw new ArgumentNullException(nameof(directors));
            _teachers = new TeacherDAO();
            _studentGrades = new StudentGradeDAO();
        }

        public Director? GetDirector()
        {
            return _directors.GetDirector();
        }

        public Teacher? GetTeacherById(int teacherId)
        {
            return _directors.GetTeacherById(teacherId);
        }

        public List<Teacher> GetAllTeachers()
        {
            return _directors.GetAllTeachers();
        }

        public void Add(Teacher teacher)
        {
            _directors.AddTeacher(teacher);
        }

        public void Update(Teacher teacher)
        {
            _directors.UpdateTeacher(teacher);
        }

        public void Delete(int teacherId)
        {
            _directors.RemoveTeacher(teacherId);
        }

        public void Subscribe(IObserver observer)
        {
            _directors.Subscribe(observer);
        }

        public List<Teacher> FindTeachersByCriteria(Language language, LanguageLevel levelOfLanguage, DateTime startedWork)
        {
            List<Teacher> teachers = GetAllTeachers();

            var filteredTeachers = teachers.Where(teacher =>
                (language == Language.NULL || (teacher.Languages != null && teacher.Languages.Contains(language))) &&
                (levelOfLanguage == LanguageLevel.NULL || (teacher.LevelOfLanguages != null && teacher.LevelOfLanguages.Contains(levelOfLanguage))) &&
                (startedWork == DateTime.MinValue || (teacher.StartedWork.Date >= startedWork.Date))
            ).ToList();

            return filteredTeachers;
        }

        public Teacher? GetTeacherByCourse(int courseId)
        {
            foreach (Teacher teacher in GetAllTeachers())
            {
                if (teacher.CoursesId != null)
                {
                    foreach (int teacherCourseId in teacher.CoursesId)
                        if (teacherCourseId == courseId) return teacher;
                }
            }
            return null;
        }

        public List<Course> GetAvailableCourses(int teacherId)
        {
            Teacher? teacher = GetTeacherById(teacherId);
            List<Course>? allCourses = _teachers?.GetAllCourses();
            List<int>? allTeacherCourses = teacher?.CoursesId;
            DateTime currentTime = DateTime.Now;

            List<Course> availableCourses = new List<Course>();

            if (allCourses != null && allTeacherCourses != null)
            {
                foreach (Course course in allCourses)
                {
                    if (allTeacherCourses.Contains(course.Id))
                    {
                        availableCourses.Add(course);
                    }
                }
            }

            return availableCourses;
        }

        public void RemoveCourseFromList(int teacherId, int courseId)
        {
            Teacher? teacher = GetTeacherById(teacherId);
            teacher?.CoursesId?.Remove(courseId);
            Update(teacher);
        }
      
        public void GenerateFirstReport()
        {
            TeacherDAO teacherDAO = new TeacherDAO();
            StudentDAO studentDAO = new StudentDAO();
            PdfGenerator pdf = new PdfGenerator();
            pdf.AddTitle("Number of penalty points in the last year");
            pdf.AddTable(teacherDAO.GetPenaltyPointsLastYearPerCourse());
            pdf.AddPage();
            pdf.AddTitle("Average points of students by penalties");
            for(int i = 0; i < 3; i++)
            {
                pdf.AddSubtitle("Number of penalty points: " + i);
                pdf.AddTable(studentDAO.GetStudentsAveragePointsPerPenalty()[i]);
            }
           
            pdf.Save(@"C:\\Users\\Milan\\Desktop\\Projekat\\LangLang\\data\\nekipdf.pdf");
        }
      
        public int GetAverageTeacherGrade(int teacherId)
        {
            int result = 0;
            List<StudentGrade> teachersGrades = _studentGrades.GetStudentGradeByTeacher(teacherId);
            foreach (StudentGrade grade in teachersGrades)
            {
                result += grade.Value;
            }
            return result == 0 ? 0 : result / teachersGrades.Count;
        }

        public void GetLanguageReport()
        {
            Dictionary<Language, int> numberOfCourses = GetNumberOfCourses();
            Dictionary<Language, int> numberOfExamTerms = GetNumberOfExamTerms();
            Dictionary<Language, int> penaltyPoints = new Dictionary<Language, int>();
            Dictionary<Language, int> values = new Dictionary<Language, int>();

        }

        public Dictionary<Language, int> GetLanguagesInt()
        {
            Dictionary<Language, int> languages = new Dictionary<Language, int>();
            var langs = Enum.GetValues(typeof(Language)).Cast<Language>().ToList();

            foreach (Language language in langs)
            {
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
            var courses = _teachers.FindCoursesByCriteria(Language.NULL, LanguageLevel.NULL, DateTime.Now.AddYears(-1), 0, null);

            foreach (var course in courses)
                numberOfCourses[course.Language] += 1;

            return numberOfCourses;
        }

        public Dictionary<Language, int> GetNumberOfExamTerms()
        {
            Dictionary<Language, int> numberOfExamTerms = GetLanguagesInt();
            var examTerms = _teachers.FindExamTermsByCriteria(Language.NULL, LanguageLevel.NULL, DateTime.Now.AddYears(-1));

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
                        {
                            levels.Add(course.Level);
                            sum += 1;
                        }
                    }
                }

                double averageNumber = levels.Count() / sum;
                numberOfPenaltyPoints.Add(number.Key, averageNumber);
            }

            return numberOfPenaltyPoints;
        }
    }
}
