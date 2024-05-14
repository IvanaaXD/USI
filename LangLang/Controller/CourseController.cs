using LangLang.Model.DAO;
using LangLang.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.Model.Enums;

namespace LangLang.Controller
{
    public class CourseController
    {
        private readonly CourseDAO _courses;
        private readonly TeacherController teacherController;

        public CourseController(TeacherController teacherController)
        {
            _courses = new CourseDAO(teacherController);
            this.teacherController = teacherController;
        }
        public Course? GetCourseById(int courseId)
        {
            return _courses.GetCourseById(courseId);
        }
        public List<Course> GetAllCourses()
        {
            return _courses.GetAllCourses();
        }

        public List<CourseGrade> GetAllCourseGrades()
        {
            return teacherController.GetAllCourseGrades();
        }

        public List<Course> GetAvailableCourses(Teacher teacher)
        {
            return _courses.GetAvailableCourses(teacher);
        }
        public Course AddCourse(Course course)
        {
            return _courses.AddCourse(course);
        }
        public void UpdateCourse(Course course)
        {
            _courses.UpdateCourse(course);
        }

        public bool ValidateCourseTimeslot(Course course, Teacher teacher)
        {
            bool isOverlap = CheckCourseOverlap(course, teacher);
            if (!isOverlap)
                return isOverlap;
            return true;
        }
        private bool CheckCourseOverlap(Course course, Teacher teacher)
        {
            List<Course> allAvailableCourses = _courses.GetAllCourses();
            List<ExamTerm> allAvailableExams = teacherController.GetAllExamTerms();

            bool isSameTeacherCourseOverlap = _courses.CheckTeacherCoursesOverlap(course, teacher);
            if (isSameTeacherCourseOverlap)
                return false;

            bool isSameTeacherExamOverlap = _courses.CheckTeacherCourseExamOverlap(course, teacher);
            if (isSameTeacherExamOverlap)
                return false;

            if (!course.IsOnline)
            {
                bool isClassroomOverlap = _courses.CheckClassroomOverlap(course, allAvailableCourses, allAvailableExams);
                if (isClassroomOverlap)
                    return false;
            }
            return true;

        }
        
        public void DeleteCourse(int courseId)
        {
            _courses.RemoveCourse(courseId);
        }

        public void Subscribe(IObserver observer)
        {
            _courses.Subscribe(observer);
        }

        public void IncrementCourseCurrentlyEnrolled(int courseId)
        {
            _courses.IncrementCourseCurrentlyEnrolled(courseId);
        }

        public void DecrementCourseCurrentlyEnrolled(int courseId)
        {
            _courses.DecrementCourseCurrentlyEnrolled(courseId);
        }

        public List<Course> FindCoursesByCriteria(Language? language, LanguageLevel? level, DateTime? startDate, int duration, bool? isOnline)
        {
            return _courses.FindCoursesByCriteria(language, level, startDate, duration, isOnline);
        }

        public bool IsStudentAccepted(Student student, int courseId)
        {
            return _courses.IsStudentAccepted(student, courseId);
        }
    }
}
