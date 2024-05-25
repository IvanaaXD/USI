using LangLang.Domain.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Domain.IRepository
{
    public interface IStudentGradeRepository
    {
        StudentGrade? GetStudentGradeById(int id);
        StudentGrade AddGrade(StudentGrade StudentGrade);
        StudentGrade UpdateGrade(StudentGrade StudentGrade);
        StudentGrade RemoveGrade(int id);
        List<StudentGrade> GetAllStudentGrades();
        public StudentGrade? GetStudentGradeByStudentTeacher(int studentId, int teacherId, int courseId);
        public StudentGrade? GetStudentGradeByStudent(int studentId, int courseId);
        public List<StudentGrade> GetStudentGradesByTeacherCourse(int teacherId, int courseId);
        public List<StudentGrade> GetStudentGradeByTeacher(int teacherId);
        void Subscribe(IObserver observer);
    }
}
