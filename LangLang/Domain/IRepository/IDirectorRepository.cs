using LangLang.Observer;
using System.Collections.Generic;
using LangLang.Domain.Model;
using LangLang.Domain.IUtility;

namespace LangLang.Domain.IRepository
{
    public interface IDirectorRepository : IObserver
    {
        Director? GetDirector();
        Teacher Add(Teacher teacher);
        Teacher? Update(Teacher? teacher);
        Director? UpdateDirector(Director? director);
        Teacher? Remove(int id);
        Teacher? GetById(int id);
        List<Teacher> GetAll();
        List<Teacher> GetAllTeachers(int page, int pageSize, string sortCriteria, List<Teacher> teachersToPaginate);
        List<Teacher> GetAllTeachers(int page, int pageSize, ISortStrategy sortStrategy, List<Teacher> teachersToPaginate);
        void Subscribe(IObserver observer);
    }
}
