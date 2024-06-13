using LangLang.Domain.Model;
using LangLang.Observer;
using System.Collections.Generic;

namespace LangLang.Domain.IRepository
{
    public interface IStudentRepository
    {
        Student? GetStudentById(int id);
        Student AddStudent(Student Student);
        Student UpdateStudent(Student Student);
        Student RemoveStudent(int id);
        List<Student> GetAllStudents();
        void Subscribe(IObserver observer);
    }
}
