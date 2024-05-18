using LangLang.Observer;
using System.Collections.Generic;
using LangLang.Domain.Model;

namespace LangLang.Domain.IRepository
{
    public interface IDirectorRepository : IObserver
    {
        Director GetDirector();
        Teacher AddTeacher(Teacher teacher);
        Teacher UpdateTeacher(Teacher teacher);
        Teacher RemoveTeacher(int id);
        Teacher GetTeacherById(int id);
        List<Teacher> GetAllTeachers();
        void Subscribe(IObserver observer);
    }
}
