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
        private readonly ITeacherRepository? _teachers;
        private readonly IStudentGradeRepository? _studentGrades;
        private readonly CourseController? _courseController;
        private readonly ExamTermController? _examTermController;
        private readonly TeacherController? _teacherController;
        private readonly ICourseRepository? _courses;
        private readonly IExamTermRepository? _examTerms;
        private readonly MailController? _mailController;
        public DirectorController()
        {
            _directors = Injector.CreateInstance<IDirectorRepository>();
            _teachers = Injector.CreateInstance<ITeacherRepository>();
            _teacherController = Injector.CreateInstance<TeacherController>();
            _courses = Injector.CreateInstance<ICourseRepository>();
            _examTerms = Injector.CreateInstance<IExamTermRepository>();
            _studentGrades = Injector.CreateInstance<IStudentGradeRepository>();
            _mailController = Injector.CreateInstance<MailController>();
            _examTermController = Injector.CreateInstance<ExamTermController>();
            _courseController = Injector.CreateInstance<CourseController>();
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
            var courses = _courseController.GetAllCourses();

            Update(_courseController.DeleteCoursesByTeacher(teacher));
            Update(_examTermController.DeleteExamTermsByTeacher(teacher, courses));

            _directors.RemoveTeacher(teacherId);
        }

        public List<Course> SetTeacher(Teacher teacher)
        {
            var activeCourses = new List<Course>();
            var courses = _courseController.GetAllCourses();

            foreach (var course in courses)
            {
                if (teacher.CoursesId.Contains(course.Id) && _courseController.IsCourseActive(course))
                {
                    teacher.CoursesId.Remove(course.Id);
                    Update(teacher);
                    activeCourses.Add(course);
                }
            }

            return activeCourses;
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

        public void RemoveCourseFromList(int teacherId, int courseId)
        {
            Teacher? teacher = GetTeacherById(teacherId);
            teacher?.CoursesId?.Remove(courseId);
            Update(teacher);
        }

        public List<Course> GetAvailableCourses(int teacherId)
        {
            Teacher? teacher = GetTeacherById(teacherId);
            List<Course>? allCourses = _courses.GetAllCourses();
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
            return _mailController.GetSentCourseMail(teacher, courseId);
        }
        public List<Mail> GetReceivedCourseMails(Teacher teacher, int courseId)
        {
            return _mailController.GetReceivedCourseMails(teacher, courseId);
        }


        public List<Course> GetAvailableCourses(Teacher teacher)
        {
            return _teacherController.GetAvailableCourses(teacher);
        }

        public void SendMail(Mail mail)
        {
            _mailController.Send(mail);
        }

        public void AnswerMail(int mailId)
        {
            _mailController.SetMailToAnswered(_mailController.GetMailById(mailId));
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
