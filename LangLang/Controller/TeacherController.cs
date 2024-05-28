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
        private readonly IDirectorRepository _director;
        private readonly IExamTermRepository _examTerms;
        private readonly IPenaltyPointRepository _penaltyPoints;

        public TeacherController()
        {
            _teachers = Injector.CreateInstance<ITeacherRepository>();
            _courses = Injector.CreateInstance<ICourseRepository>();
            _examTerms = Injector.CreateInstance<IExamTermRepository>();
            _director = Injector.CreateInstance<IDirectorRepository>(); 
            _penaltyPoints = Injector.CreateInstance<IPenaltyPointRepository>();
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

            foreach(Teacher teacher in _director.GetAllTeachers())
            {
                if (teacher.ExamsId.Contains(id))
                {
                    teacher.ExamsId.Remove(id);
                    _director.UpdateTeacher(teacher);
                }
            }
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
        
        public void Subscribe(IObserver observer)
        {
            _teachers.Subscribe(observer);
            _courses.Subscribe(observer);
            _examTerms.Subscribe(observer);
        }
        public List<ExamTerm> GetAvailableExamTerms(Teacher teacher)
        {
            List<int> allTeacherExams = teacher.ExamsId;

            List<ExamTerm> availableExams = new List<ExamTerm>();

            foreach (int examId in allTeacherExams)
            {
                availableExams.Add(_examTerms.GetExamTermById(examId));
            }
            return availableExams;
        }

        public List<PenaltyPoint> GetAllPenaltyPoints()
        {
            return _penaltyPoints.GetAllPenaltyPoints();
        }
    }
}
