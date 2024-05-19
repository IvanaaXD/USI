using LangLang.Domain.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Domain.IRepository
{
    public interface IExamTermRepository : IObserver
    {
        ExamTerm? AddExamTerm(ExamTerm examTerm);
        ExamTerm? UpdateExamTerm(ExamTerm examTerm);
        ExamTerm? GetExamTermById(int id);
        public List<ExamTerm> GetAllExamTerms();
        void Subscribe(IObserver observer);
    }
}
