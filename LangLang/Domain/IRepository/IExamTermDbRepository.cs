using LangLang.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Domain.IRepository
{
    public interface IExamTermDbRepository
    {
        ExamTerm GetById(int id);
        void Add(ExamTerm examTerm);
        void Update(ExamTerm examTerm);
        void Remove(ExamTerm examTerm);
        void Delete(int id);
    }
}
