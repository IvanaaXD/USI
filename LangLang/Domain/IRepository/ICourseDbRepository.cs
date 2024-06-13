using LangLang.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Domain.IRepository
{
    public interface ICourseDbRepository
    {
        Course GetById(int id);
        void Add(Course course);
        void Update(Course course);
        void Remove(Course course);
        void Delete(int id);
    }
}
