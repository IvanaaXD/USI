using LangLang.Model.DAO;
using LangLang.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Controller
{
    public class TeacherController
    {
        private readonly TeacherDAO _coursesExams;

        public TeacherController()
        {
            _coursesExams = new TeacherDAO();
        }
        public List<Course> GetAllCourses()
        {
            return _coursesExams.GetAllCourses();
        }
        public List<ExamTerm> GetAllExamTerms()
        {
            return _coursesExams.GetAllExamTerms();
        }
        public void AddCourse(Course course)
        {
            _coursesExams.AddCourse(course);
        }
        public void AddExamTerm(ExamTerm examTerm)
        {
            _coursesExams.AddExamTerm(examTerm);
        }

        public void UpdateCourse(Course course)
        {
            _coursesExams.UpdateCourse(course);
        }

        public void UpdateExamTerm(ExamTerm examTerm)
        {
            _coursesExams.UpdateExamTerm(examTerm);
        }

        public void DeleteCourse(int courseId)
        {
            _coursesExams.RemoveCourse(courseId);
        }

        public void DeleteExamTerm(int examId)
        {
            _coursesExams.RemoveExamTerm(examId);
        }
        public void Subscribe(IObserver observer)
        {
            _coursesExams.Subscribe(observer);
        }
    }
}
