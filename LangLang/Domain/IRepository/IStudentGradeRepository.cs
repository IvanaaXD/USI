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
        void Subscribe(IObserver observer);
    }
}
