using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangLang.Model.Enums;
using LangLang.Observer;
using LangLang.Storage;

namespace LangLang.Model.DAO
{
    public class TeacherDAO : Subject
    {
        private readonly List<Course> _courses;
        private readonly Storage<Course> _courseStorage;

        public TeacherDAO()
        {
            _courseStorage = new Storage<Course>("course.txt");
            _courses = _courseStorage.Load();
        }

        private int GenerateCourseId()
        {
            if (_courses.Count == 0) return 0;
            return _courses.Last().CourseID + 1;
        }

        public Course AddCourse(Course course)
        {
            course.CourseID = GenerateCourseId();
            _courses.Add(course);
            _courseStorage.Save(_courses);
            NotifyObservers();
            return course;
        }

        public Course? UpdateCourse(Course course)
        {
            Course? oldCourse = GetCourseById(course.CourseID);
            if (oldCourse == null) return null;

            oldCourse.Language = course.Language;
            oldCourse.Level = course.Level;
            oldCourse.Duration = course.Duration;
            oldCourse.WorkDays = course.WorkDays;
            oldCourse.StartDate = course.StartDate;
            oldCourse.IsOnline = course.IsOnline;
            oldCourse.MaxEnrolledStudents = course.MaxEnrolledStudents;
            oldCourse.ExamTerms = course.ExamTerms;

            _courseStorage.Save(_courses);
            NotifyObservers();
            return oldCourse;
        }

        public Course? RemoveCourse(int id)
        {
            Course? course = GetCourseById(id);
            if (course == null) return null;

            _courses.Remove(course);
            /*foreach(int examTermId in course.ExamTerms)
            {
                // ToDo : Delete each term
            }*/

            _courseStorage.Save(_courses);
            NotifyObservers();
            return course;
        }

        private Course? GetCourseById(int id)
        {
            return _courses.Find(v => v.CourseID == id);
        }

        public List<Course> GetAllCourses()
        {
            return _courses;
        }
        public List<Course> FindCoursesByCriteria(Language? language, LanguageLevel? level, DateTime? startDate, int duration, bool? isOnline)
        {
            var filteredCourses = _courses.Where(course =>
                (!language.HasValue || course.Language == language.Value) &&
                (!level.HasValue || course.Level == level.Value) &&
                (!startDate.HasValue || course.StartDate.Date == (startDate.Value.Date)) &&
                (duration == 0 || course.Duration == duration) &&
                (!isOnline.HasValue || course.IsOnline == isOnline.Value)
            ).ToList();

            return filteredCourses;
        }
    }
}
