using LangLang.Model.DAO;
using LangLang.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.Model.Enums;

namespace LangLang.Controller
{
    public class TeacherController
    {
        private readonly TeacherDAO _teachers;
        private readonly ExamTermGradeDAO _examTermGrades;
        private readonly CourseGradeDAO _courseGrades;

        public TeacherController()
        {
            _teachers = new TeacherDAO();
            _examTermGrades = new ExamTermGradeDAO();
            _courseGrades = new CourseGradeDAO();
        }
        public Course? GetCourseById(int courseID)
        {
            return _teachers.GetCourseById(courseID);
        }
        public ExamTerm? GetExamTermById(int examId)
        {
            return _teachers.GetExamTermById(examId);
        }
        public List<Course> GetAllCourses()
        {
            return _teachers.GetAllCourses();
        }
        public List<ExamTerm> GetAllExamTerms()
        {
            return _teachers.GetAllExamTerms();
        }

        public List<Mail> GetAllMails()
        {
            return _teachers.GetAllMails();
        }

        public List<ExamTermGrade> GetAllExamTermGrades()
        {
            return _examTermGrades.GetAllExamTermGrades();
        }

        public List<ExamTermGrade> GetExamTermGradesByTeacherExam(int teacherId, int examTermId)
        {
            return _examTermGrades.GetExamTermGradesByTeacherExam(teacherId, examTermId);
        }

        public ExamTermGrade? GetExamTermGradeByStudentTeacherExam(int  studentId, int teacherId, int examTermId) 
        {
            return _examTermGrades.GetExamTermGradeByStudentTeacherExam(studentId, teacherId, examTermId);
        }

        public List<Course> GetAvailableCourses(Teacher teacher)
        {
            return _teachers.GetAvailableCourses(teacher);
        }
        public Course AddCourse(Course course)
        {
            return _teachers.AddCourse(course);
        }
        public void AddExamTerm(ExamTerm examTerm)
        {
            _teachers.AddExamTerm(examTerm);
        }

        public void UpdateCourse(Course course)
        {
            _teachers.UpdateCourse(course);
        }

        public void UpdateExamTerm(ExamTerm examTerm)
        {
            _teachers.UpdateExamTerm(examTerm);
        }

        public ExamTermGrade GradeStudent(ExamTermGrade grade)
        {
            return _examTermGrades.AddGrade(grade);
        }

        public void GradeStudentCourse(CourseGrade grade)
        {
            _courseGrades.AddGrade(grade);
        }

        public ExamTerm ConfirmExamTerm(int examTermId)
        {
            return _teachers.ConfirmExamTerm(examTermId);
        }

        public bool IsStudentGradedExamTerm(int studentId)
        {
            return _examTermGrades.IsStudentGraded(studentId);
        }

        public bool CheckExamOverlap(int ExamID, DateTime ExamDate)
        {
            int examDurationInMinutes = 240;

            DateTime examStartDateTime = ExamDate;
            DateTime examEndDateTime = examStartDateTime.AddMinutes(examDurationInMinutes);

            IEnumerable<dynamic> overlappingExams = _teachers.GetAllExamTerms()
                .Where(item =>
                {
                    bool isDifferentId = item.ExamID != ExamID;

                    DateTime itemExamDateTime = item.ExamTime;

                    bool isOverlap = isDifferentId && (itemExamDateTime < examEndDateTime && itemExamDateTime.AddMinutes(examDurationInMinutes) > examStartDateTime);

                    return isOverlap;
                });

            /*
            IEnumerable<dynamic> possibleOverlappingCourses =  _teachers.GetAllCourses()
        .Where(c =>
        {
            DateTime courseStartDateTime = c.StartDate;
            DateTime courseEndDateTime = c.StartDate.AddDays(c.Duration * 7);

            bool possible =
                (courseStartDateTime >= examStartDateTime && courseStartDateTime <= examEndDateTime) ||
                (courseEndDateTime >= examStartDateTime && courseEndDateTime <= examEndDateTime) ||
                (courseStartDateTime <= examStartDateTime && courseEndDateTime >= examEndDateTime);
            if (possible == true)
            {

                DayOfWeek dayOfWeek = ExamDate.DayOfWeek;
                if (c.WorkDays.Any(d => d == ExamDate.DayOfWeek))
                {
                    
                }
            }

                
            return true;
        });*/



            return !overlappingExams.Any();
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
            List<Course> allAvailableCourses = _teachers.GetAllCourses();
            List<ExamTerm> allAvailableExams = _teachers.GetAllExamTerms();

            bool isSameTeacherCourseOverlap = _teachers.CheckTeacherCoursesOverlap(course, teacher);
            if (isSameTeacherCourseOverlap)
                return false;

            bool isSameTeacherExamOverlap = _teachers.CheckTeacherCourseExamOverlap(course, teacher);
            if (isSameTeacherExamOverlap)
                return false;

            if (!course.IsOnline)
            {
                bool isClassroomOverlap = _teachers.CheckClassroomOverlap(course, allAvailableCourses, allAvailableExams);
                if (isClassroomOverlap)
                    return false;
            }
            return true;

        }

        public void DeleteCourse(int courseId)
        {
            _teachers.RemoveCourse(courseId);
        }

        public void DeleteExamTerm(int examId)
        {
            _teachers.RemoveExamTerm(examId);
        }
        public void Subscribe(IObserver observer)
        {
            _teachers.Subscribe(observer);
        }

        public void IncrementCourseCurrentlyEnrolled(int courseId)
        {
            _teachers.IncrementCourseCurrentlyEnrolled(courseId);
        }

        public void DecrementCourseCurrentlyEnrolled(int courseId)
        {
            _teachers.DecrementCourseCurrentlyEnrolled(courseId);
        }

        public List<Course> FindCoursesByCriteria(Language? language, LanguageLevel? level, DateTime? startDate, int duration, bool? isOnline)
        {
            return _teachers.FindCoursesByCriteria(language, level, startDate, duration, isOnline);
        }
        public List<ExamTerm> FindExamTermsByCriteria(Language? language, LanguageLevel? level, DateTime? examDate)
        {
            return _teachers.FindExamTermsByCriteria(language, level, examDate);
        }
    }
}
