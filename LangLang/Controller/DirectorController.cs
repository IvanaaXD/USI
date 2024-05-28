using LangLang.Observer;
using System;
using System.Collections.Generic;
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
        public void Update(Director director)
        {
            _directors.UpdateDirector(director);
        }
        public void Delete(int teacherId)
        {
            Teacher teacher = GetTeacherById(teacherId);
            var courses = _courseController.GetAllCourses();

            Update(_courseController.DeleteCoursesByTeacher(teacher));
            Update(_examTermController.DeleteExamTermsByTeacher(teacher, courses));

            _directors.RemoveTeacher(teacherId);
        }

        public List<Course> GetActiveCourses(Teacher teacher)
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
            _teachers.Subscribe(observer);
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

        public List<Teacher> GetAllTeachers(int page, int pageSize, string sortCriteria, List<Teacher> teachers)
        {
            return _directors.GetAllTeachers(page, pageSize, sortCriteria, teachers);
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

        public Teacher? GetTeacherByExamTerm(int examTermId)
        {
            foreach (Teacher teacher in GetAllTeachers())
            {
                if (teacher.ExamsId != null)
                {
                    foreach (int teacherExamTermId in teacher.ExamsId)
                        if (teacherExamTermId == examTermId) return teacher;
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
        public List<Teacher> GetCompatibleTeachers(Language language, LanguageLevel level)
        {
            List<Teacher> compatibleTeachers = new List<Teacher>();
            List<Teacher> allTeachers = GetAllTeachers();
            foreach (Teacher teacher in allTeachers)
            {
                for (int i = 0; i < teacher.Languages.Count; i++)
                {
                    if (teacher.Languages[i] == language && teacher.LevelOfLanguages[i] == level)
                    {
                        compatibleTeachers.Add(teacher);
                        break;
                    }
                }
            }
            return compatibleTeachers;
        }
        public List<Teacher> GetAvailableTeachers(Course course)
        {
            List<Teacher> compatibleTeachers = GetCompatibleTeachers(course.Language, course.Level);
            List<Teacher> availableTeachers = new List<Teacher>();
            foreach (Teacher teacher in compatibleTeachers)
            {
                if (_courseController.ValidateCourseTimeslot(course, teacher))
                    availableTeachers.Add(teacher);
            }
            return availableTeachers;
        }
        public List<Teacher> GetAvailableTeachers(ExamTerm examTerm)
        {
            List<Teacher> compatibleTeachers = GetCompatibleTeachers(examTerm.Language, examTerm.Level);
            List<Teacher> availableTeachers = new();
            foreach (Teacher teacher in compatibleTeachers)
            {
                if (_examTermController.ValidateExamTimeslot(examTerm, teacher))
                    availableTeachers.Add(teacher);
            }
            return availableTeachers;
        }

        public int FindMostAppropriateTeacher(Course course)
        {
            List<Teacher> availableTeachers = GetAvailableTeachers(course);
            if (availableTeachers.Count == 0)
                return -1;

            Dictionary<int, double> teacherGrade = new();
            foreach (Teacher teacher in availableTeachers)
            {
                teacherGrade.Add(teacher.Id, GetAverageTeacherGrade(teacher.Id));
            }

            int teacherWithMaxGradeId = teacherGrade.OrderByDescending(kv => kv.Value).First().Key;
            int firstUngradedTeacherId;
            if (teacherGrade.ContainsValue(0))
            {
                firstUngradedTeacherId = teacherGrade.FirstOrDefault(pair => pair.Value == 0).Key;
            }
            return teacherWithMaxGradeId;
        }
        public int FindMostAppropriateTeacher(ExamTerm examTerm)
        {
            List<Teacher> availableTeachers = GetAvailableTeachers(examTerm);
            if (availableTeachers.Count == 0)
                return -1;

            Dictionary<int, double> teacherGrade = new();
            foreach (Teacher teacher in availableTeachers)
            {
                teacherGrade.Add(teacher.Id, GetAverageTeacherGrade(teacher.Id));
            }

            int teacherWithMaxGradeId = teacherGrade.OrderByDescending(kv => kv.Value).First().Key;
            int firstUngradedTeacherId;
            if (teacherGrade.ContainsValue(0))
            {
                firstUngradedTeacherId = teacherGrade.FirstOrDefault(pair => pair.Value == 0).Key;
            }
            return teacherWithMaxGradeId;
        }

    }
}
