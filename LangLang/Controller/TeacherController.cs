using LangLang.Repository;
using LangLang.Domain.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.Domain.IRepository;

namespace LangLang.Controller
{
    public class TeacherController
    {
        private readonly ITeacherRepository _teachers;
        private readonly ICourseRepository _courses;
        private readonly IExamTermRepository _examTerms;
        private readonly PenaltyPointRepository _penaltyPoints;

        public TeacherController()
        {
            _teachers = Injector.CreateInstance<ITeacherRepository>();
            _courses = Injector.CreateInstance<ICourseRepository>();
            _examTerms = Injector.CreateInstance<IExamTermRepository>();
            _penaltyPoints = Injector.CreateInstance<PenaltyPointRepository>();
        }
        public Course? GetCourseById(int courseId)
        {
            return _courses.GetCourseById(courseId);
        }
        public ExamTerm? GetExamTermById(int examId)
        {
            return _examTerms.GetExamTermById(examId);
        }
        public List<Course> GetAllCourses()
        {
            return _courses.GetAllCourses();
        }
        public List<ExamTerm> GetAllExamTerms()
        {
            return _examTerms.GetAllExamTerms();
        }

        public ExamTerm? RemoveExamTerm(int id)
        {
            ExamTerm? examTerm = GetExamTermById(id);
            if (examTerm == null) return null;

            int courseId = examTerm.CourseID;
            Course? course = _courses.GetCourseById(courseId);
            course.ExamTerms.Remove(id);
            _courses.UpdateCourse(course);

            _examTerms.RemoveExamTerm(examTerm.ExamID);
            return examTerm;
        }

        public List<Course> GetAvailableCourses(Teacher teacher)
        {
            List<int> allTeacherCourses = teacher.CoursesId;

            List<Course> availableCourses = new List<Course>();

            foreach (int courseId in allTeacherCourses)
            {
                availableCourses.Add(_courses.GetCourseById(courseId));
            }
            return availableCourses;
        }

        public bool CheckExamOverlap(int ExamID, DateTime ExamDate)
        {
            int examDurationInMinutes = 240;

            DateTime examStartDateTime = ExamDate;
            DateTime examEndDateTime = examStartDateTime.AddMinutes(examDurationInMinutes);

            IEnumerable<dynamic> overlappingExams = _examTerms.GetAllExamTerms()
                .Where(item =>
                {
                    bool isDifferentId = item.ExamID != ExamID;

                    DateTime itemExamDateTime = item.ExamTime;

                    bool isOverlap = isDifferentId && (itemExamDateTime < examEndDateTime && itemExamDateTime.AddMinutes(examDurationInMinutes) > examStartDateTime);

                    return isOverlap;
                });

            return !overlappingExams.Any();
        }
        public Course GetCourseByExamId(int id)
        {
            Course course = null;
            List<ExamTerm> examTerms = GetAllExamTerms();
            foreach (ExamTerm examTerm in examTerms)
            {
                if (examTerm.ExamID == id)
                {

                    course = GetCourseById(examTerm.CourseID);
                    break;
                }
            }
            return course;
        }

        public void Subscribe(IObserver observer)
        {
            _teachers.Subscribe(observer);
            _courses.Subscribe(observer);
            _examTerms.Subscribe(observer);
        }
        public List<ExamTerm> GetAvailableExamTerms(Teacher teacher)
        {
            List<ExamTerm> allExamTerms = _examTerms.GetAllExamTerms();
            List<Course> allTeacherCourses = GetAvailableCourses(teacher);

            List<ExamTerm> availableExamTerms = new();
            List<int> examTermIds = new();

            foreach (Course course in allTeacherCourses)
            {
                if (course != null)
                {
                    foreach (int examId in course.ExamTerms)
                    {
                        examTermIds.Add(examId);
                    }
                }
                
            }

            foreach (ExamTerm examTerm in allExamTerms)
            {
                if (examTermIds.Contains(examTerm.ExamID))
                {
                    availableExamTerms.Add(examTerm);
                }
            }

            return availableExamTerms;
        }

        public List<PenaltyPoint> GetAllPenaltyPoints()
        {
            return _penaltyPoints.GetAllPenaltyPoints();
        }
    }
}
