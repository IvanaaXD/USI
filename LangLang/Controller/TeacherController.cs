using LangLang.Repository;
using LangLang.Domain.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.Domain.IRepository;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace LangLang.Controller
{
    public class TeacherController
    {
        private readonly ITeacherRepository _teachers;
        private readonly ICourseDbRepository _courses;
        private readonly IStudentRepository _students;
        private readonly IDirectorRepository _director;
        private readonly IExamTermDbRepository _examTerms;
        private readonly IPenaltyPointRepository _penaltyPoints;

        public TeacherController()
        {
            _teachers = Injector.CreateInstance<ITeacherRepository>();
            _courses = Injector.CreateInstance<ICourseDbRepository>();
            _students = Injector.CreateInstance<IStudentRepository>();
            _examTerms = Injector.CreateInstance<IExamTermDbRepository>();
            _director = Injector.CreateInstance<IDirectorRepository>(); 
            _penaltyPoints = Injector.CreateInstance<IPenaltyPointRepository>();
        }
        public Course? GetCourseById(int courseId)
        {
            return _courses.GetById(courseId);
        }
        public ExamTerm? GetExamTermById(int examId)
        {
            return _examTerms.GetById(examId);
        }
        public List<Course> GetAllCourses()
        {
            return _courses.GetAll();
        }
        public List<ExamTerm> GetAllExamTerms()
        {
            return _examTerms.GetAll();
        }

        public ExamTerm? RemoveExamTerm(int id)
        {
            ExamTerm? examTerm = GetExamTermById(id);
            if (examTerm == null) return null;
            RemoveExamIdFromTeachers(id);
            RemoveExamIdFromStudents(id);
            
            _examTerms.Delete(examTerm.ExamID);
            return examTerm;
        }
        private void RemoveExamIdFromTeachers(int id)
        {
            foreach (Teacher teacher in _director.GetAllTeachers())
            {
                if (teacher.ExamsId.Contains(id))
                {
                    teacher.ExamsId.Remove(id);
                    _director.UpdateTeacher(teacher);
                }
            }
        }
        private void RemoveExamIdFromStudents(int id)
        {
            foreach (Student student in _students.GetAllStudents())
            {
                if (student.RegisteredExamsIds.Contains(id))
                {
                    student.RegisteredExamsIds.Remove(id);
                    _students.UpdateStudent(student);
                }
                else if (student.PassedExamsIds.Contains(id))
                {
                    student.PassedExamsIds.Remove(id);
                    _students.UpdateStudent(student);
                }
            }
        }
        public List<Course> GetAvailableCourses(Teacher teacher)
        {
            List<int> allTeacherCourses = teacher.CoursesId;

            List<Course> availableCourses = new();

            foreach (int courseId in allTeacherCourses)
            {
                Course? course = _courses.GetById(courseId);
                if (course!=null)
                    availableCourses.Add(course);
            }
            return availableCourses;
        }

        public bool CheckExamOverlap(int ExamID, DateTime ExamDate)
        {
            int examDurationInMinutes = 240;

            DateTime examStartDateTime = ExamDate;
            DateTime examEndDateTime = examStartDateTime.AddMinutes(examDurationInMinutes);

            IEnumerable<dynamic> overlappingExams = _examTerms.GetAll()
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
                availableExams.Add(_examTerms.GetById(examId));
            }
            return availableExams;
        }

        public List<PenaltyPoint> GetAllPenaltyPoints()
        {
            return _penaltyPoints.GetAllPenaltyPoints();
        }
    }
}
