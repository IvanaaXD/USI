using LangLang.Observer;
using System;
using System.Collections.Generic;
using LangLang.Repository;
using LangLang.Domain.Model;
using LangLang.Domain.Model.Enums;
using System.Linq;
using LangLang.Domain.IRepository;
using System.Windows.Input;
using System.DirectoryServices.ActiveDirectory;
using Org.BouncyCastle.Asn1.Mozilla;
using System.Reflection.Metadata;
using iText.Kernel.Pdf;

namespace LangLang.Controller
{
    public class DirectorController
    {
        private readonly IDirectorRepository _directors;
        private readonly TeacherRepository? _teachers;
        private readonly ExamTermDAO? _examTerms;
        private readonly StudentGradeRepository? _studentGrades;
        private readonly PenaltyPointDAO? _penaltyPoints;
        private readonly CourseController? _courseController;
        private readonly CourseGradeRepository? _courseGrade;
        private readonly TeacherController? _teacherController;
        private readonly ExamTermController? _examTermController;
        private readonly ExamTermGradeRepository? _examTermGrades;

        public DirectorController(IDirectorRepository directors)
        {
            _directors = directors ?? throw new ArgumentNullException(nameof(directors));
            _teachers = new TeacherRepository();
            _examTerms = new ExamTermDAO();
            _studentGrades = new StudentGradeRepository();
            _penaltyPoints = new PenaltyPointDAO();
            _examTermGrades = new ExamTermGradeRepository();
            _courseGrade = new CourseGradeRepository();

            _teacherController = new TeacherController();
            _examTermController = new ExamTermController(new ExamTermRepository(), new TeacherController());
            _courseController = new CourseController(new CourseRepository(), new TeacherController());

        }
        public DirectorController()
        {
            _directors = Injector.CreateInstance<IDirectorRepository>();
            _teachers = new TeacherRepository();
            _examTerms = new ExamTermDAO();
            _studentGrades = new StudentGradeRepository();
            _penaltyPoints = new PenaltyPointDAO();
            _examTermGrades = new ExamTermGradeRepository();

            _teacherController = new TeacherController();
            _examTermController = new ExamTermController(new ExamTermRepository(), new TeacherController());
            _courseController = new CourseController(new CourseRepository(), new TeacherController());

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
            Teacher teacher = GetTeacherById(teacherId);

            DeleteCoursesByTeacher(teacher);
            DeleteExamTermsByTeacher(teacher);
            _directors.RemoveTeacher(teacherId);
        }

        public void Subscribe(IObserver observer)
        {
            _directors.Subscribe(observer);
        }

        public void DeleteCoursesByTeacher(Teacher teacher)
        {
            var courses = _courseController.GetAllCourses();
            var teacherCourses = teacher.CoursesId;

            foreach (var course in courses)
            {
                if (teacherCourses.Contains(course.Id) && course.StartDate > DateTime.Today.Date)
                    teacherCourses.Remove(course.Id);
            }

            teacher.CoursesId = teacherCourses;
            Update(teacher);
        }

        public void DeleteExamTermsByTeacher(Teacher teacher)
        {
            var examTerms = _examTermController.GetAllExamTerms();
            var courses = _courseController.GetAllCourses();
            var teacherExamTerms = teacher.CoursesId;

            foreach (var examTerm in examTerms)
            {
                var course = _courseController.GetCourseById(examTerm.CourseID);
                if (teacherExamTerms.Contains(examTerm.CourseID) &&  course.StartDate > DateTime.Today.Date)
                    teacherExamTerms.Remove(course.Id);
            }

            teacher.CoursesId = teacherExamTerms;
            Update(teacher);
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

        public void RemoveCourseFromList(int teacherId, int courseId)
        {
            Teacher? teacher = GetTeacherById(teacherId);
            teacher?.CoursesId?.Remove(courseId);
            Update(teacher);
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

        public List<Mail> GetSentCourseMail(Teacher teacher, int courseId)
        {
            return _teachers.GetSentCourseMail(teacher, courseId);
        }
        public List<Mail> GetReceivedCourseMails(Teacher teacher, int courseId)
        {
            return _teachers.GetReceivedCourseMails(teacher, courseId);
        }


        public List<Course> GetAvailableCourses(Teacher teacher)
        {
            return _teachers.GetAvailableCourses(teacher);
        }

        public Mail SendMail(Mail mail)
        {
            return _teachers.SendMail(mail);
        }

        public Mail AnswerMail(int mailId)
        {
            return _teachers.AnswerMail(mailId);
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
    }
}
