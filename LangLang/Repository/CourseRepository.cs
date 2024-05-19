using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using LangLang.Domain.Model;
using LangLang.Controller;
using LangLang.Domain.Model.Enums;
using LangLang.Observer;
using LangLang.Storage;
using LangLang.Domain.IRepository;

namespace LangLang.Repository
{
    public class CourseRepository : Subject, ICourseRepository
    {
        private readonly List<Course> _courses;
        private readonly Storage<Course> _courseStorage;

        private readonly TeacherDAO teacherDAO;

        public CourseRepository()
        {
            _courseStorage = new Storage<Course>("course.csv");
            _courses = _courseStorage.Load();
            teacherDAO = new TeacherDAO();
        }

        private int GenerateCourseId()
        {
            if (_courses.Count == 0) return 0;
            return _courses.Last().Id + 1;
        }

        public Course AddCourse(Course course)
        {
            course.Id = GenerateCourseId();
            _courses.Add(course);
            _courseStorage.Save(_courses);
            NotifyObservers();
            return course;
        }

        public Course? UpdateCourse(Course course)
        {
            Course? oldCourse = GetCourseById(course.Id);
            if (oldCourse == null) return null;

            oldCourse.Language = course.Language;
            oldCourse.Level = course.Level;
            oldCourse.Duration = course.Duration;
            oldCourse.WorkDays = course.WorkDays;
            oldCourse.StartDate = course.StartDate;
            oldCourse.IsOnline = course.IsOnline;
            oldCourse.CurrentlyEnrolled = course.CurrentlyEnrolled;
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
            foreach (int examTermId in course.ExamTerms)
            {
                teacherDAO.RemoveExamTerm(examTermId);
            }

            _courseStorage.Save(_courses);
            NotifyObservers();
            return course;
        }

        public Course? GetCourseById(int id)
        {
            return _courses.Find(v => v.Id == id);
        }

        public List<Course> GetAllCourses()
        {
            return _courses;
        }
        public void Update()
        {
            throw new NotImplementedException();
        }

    }
}
