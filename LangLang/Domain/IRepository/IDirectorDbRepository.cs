using LangLang.Domain.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Domain.IRepository
{
    public interface IDirectorDbRepository
    {
        Director? GetDirector();
        Teacher GetById(int id);
        Teacher Add(Teacher teacher);
        Teacher? Update(Teacher teacher);
        Director? Update(Director director);
        Teacher? Remove(int teacherId);
        List<Teacher> GetAll();
        List<Teacher> GetAllTeachers(int page, int pageSize, string sortCriteria, List<Teacher> teachersToPaginate);
        void Subscribe(IObserver observer);
    }
}
