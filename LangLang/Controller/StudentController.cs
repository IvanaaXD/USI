using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangLang.Model;
using LangLang.Model.DAO;
using LangLang.Observer;

namespace LangLang.Controller
{
    public class StudentsController
    {
        private readonly StudentDAO _students;

        public StudentsController()
        {
            _students = new StudentDAO();
        }

        public List<Student> GetAllStudents()
        {
            return _students.GetAllStudents();
        }

        public void Add(Student student)
        {
            _students.AddStudent(student);
        }

        public void Delete(int studentId)
        {
            _students.RemoveStudent(studentId);
        }

        public void Update(Student student)
        {
            _students.UpdateStudent(student);
        }
        public void Subscribe(IObserver observer)
        {
            _students.Subscribe(observer);
        }

        public List<Course> GetAvailableCourses(int studentId)
        {
            return _students.GetAvailableCourses(studentId);
        }
        public List<ExamTerm> GetAvailableExamTerms(int studentId)
        {
            return _students.GetAvailableExamTerms(studentId);
        }
        public List<ExamTerm> GetRegisteredExamTerms(int studentId)
        {
            return _students.GetRegisteredExamTerms(studentId);
        }
        public List<ExamTerm> GetCompletedExamTerms(int studentId)
        {
            return _students.GetCompletedExamTerms(studentId);
        }

        public Student? GetStudentById(int studentId)
        {
            return _students.GetStudentById(studentId);
        }
        public bool IsEmailUnique(string email)
        {
            return _students.IsEmailUnique(email);
        }
    }
}
