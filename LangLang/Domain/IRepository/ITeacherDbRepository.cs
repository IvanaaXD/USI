using LangLang.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Domain.IRepository
{
    public interface ITeacherDbRepository
    {
        Teacher GetById(int id);
        void Add(Teacher teacher);
        void Update(Teacher teacher);
        void Remove(Teacher teacher);
        void Delete(int id);
    }
}
