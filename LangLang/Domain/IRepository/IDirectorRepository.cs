using LangLang.Observer;
using System.Collections.Generic;
using LangLang.Domain.Model;

namespace LangLang.Domain.IRepository
{
    public interface IDirectorRepository : IObserver
    {
        Director? GetDirector();
        Teacher AddTeacher(Teacher teacher);
        Teacher? UpdateTeacher(Teacher? teacher);
        Director? UpdateDirector(Director? director);
        Teacher? RemoveTeacher(int id);
        Teacher? GetTeacherById(int id);
        List<Teacher> GetAllTeachers();
        List<Teacher> GetAllTeachers(int page, int pageSize, string sortCriteria, List<Teacher> teachersToPaginate);
        void Subscribe(IObserver observer);
    }
}
