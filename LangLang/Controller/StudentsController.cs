using System.Collections.Generic;
using LangLang.Repository;
using LangLang.Observer;
using LangLang.Domain.Model;

namespace LangLang.Controller
{
    public class StudentsController
    {
        private readonly StudentDAO _students;
        private readonly StudentGradeDAO _studentGrades;

        public StudentsController()
        {
            _students = new StudentDAO();
            _studentGrades = new StudentGradeDAO();
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
        public List<Course> GetRegisteredCourses(int studentId)
        {
            return _students.GetRegisteredCourses(studentId);
        }
        public List<Course> GetCompletedCourses(int studentId)
        {
            return _students.GetCompletedCourses(studentId);
        }
        public List<ExamTerm> GetRegisteredExamTerms(int studentId)
        {
            return _students.GetRegisteredExamTerms(studentId);
        }
        public List<ExamTerm> GetCompletedExamTerms(int studentId)
        {
            return _students.GetCompletedExamTerms(studentId);
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

        public List<Student> GetAllStudentsCompletedCourse(int courseId)
        {
            return _students.GetAllStudentsCompletedCourse(courseId);
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
        public Course GetActiveCourse(int studentId)
        {
            return _students.GetActiveCourse(studentId);
        }
        public bool IsQuitCourseMailSent(int studentId, int courseId)
        {
            return _students.IsQuitCourseMailSent(studentId, courseId);
        }
        public StudentGrade GradeStudentCourse(StudentGrade grade)
        {
            return _studentGrades.AddGrade(grade);
        }

        public Student GetStudentByEmail(string email)
        {
            return _students.GetStudentByEmail(email);
        }
        public int IsSomeCourseCompleted(int studentId)
        {
            return _students.IsSomeCourseCompleted(studentId);
        }
        public bool IsEnterCourseRequestAccepted(int studentId)
        {
            return _students.IsEnterCourseRequestAccepted(studentId);
        }
        public int GetCompletedCourseNumber(int studentId)
        {
            return _students.GetCompletedCourseNumber(studentId);
        }
        public int GetPassedExamsNumber(int studentId)
        {
            return _students.GetPassedExamsNumber(studentId);
        }
 
        public void ProcessPenaltyPoints()
        {
           _students.ProcessPenaltyPoints();
        }

        public int GetPenaltyPointCount(int studentId)
        {
            return _students.GetPenaltyPointCount(studentId);
        }

        public void DeactivateStudentAccount(Student student)
        {
            _students.DeactivateStudentAccount(student);
        }
    }
}
