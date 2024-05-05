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
        public void ProcessPenaltyPoints()
        {
            _students.ProcessPenaltyPoints();   
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

        public List<Course> GetRegisteredCourses(int studentId)
        {
            return _students.GetRegisteredCourses(studentId);
        }
        public List<Course> GetCompletedCourses(int studentId)
        {
            return _students.GetCompletedCourses(studentId);
        }
        public List<Course> GetPassedCourses(int studentId)
        {
            return _students.GetPassedCourses(studentId);

        }

        public Student? GetStudentById(int studentId)
        {
            return _students.GetStudentById(studentId);
        }
        public List<Student> GetAllStudentsRequestingCourse(int courseId)
        {
            return _students.GetAllStudentsRequestingCourse(courseId);
        }

        public List<Student> GetAllStudentsEnrolledCourse(int courseId)
        {
            return _students.GetAllStudentsForCourse(courseId);
        }

        public List<Student> GetAllStudentsForCourseGrading(int courseId)
        {
            return _students.GetAllStudentsForCourseGrading(courseId);
        }

        public List<Student> GetAllStudentsForExamTerm(int examTermId)
        {
            return _students.GetAllStudentsForExamTerm(examTermId);
        }

        public bool IsEmailUnique(string email)
        {
            return _students.IsEmailUnique(email);
        }
        public bool RegisterForCourse(int studentId, int courseId)
        {
            return _students.RegisterForCourse(studentId, courseId);
        }
        public bool CancelCourseRegistration(int studentId, int courseId)
        {
            return _students.CancelCourseRegistration(studentId, courseId);
        }
        public bool RegisterForExam(int studentId, int examId)
        {
            return _students.RegisterForExam(studentId, examId);
        }
        public bool CancelExamRegistration(int studentId, int examId)
        {
            return _students.CancelExamRegistration(studentId, examId);
        }

        public bool IsStudentAttendingCourse(int studentId)
        {
            return _students.IsStudentAttendingCourse(studentId);
        }

        public bool GivePenaltyPoint(int studentId)
        {
            return _students.GivePenaltyPoint(studentId);
        }
        }
    } 
