using LangLang.Domain.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Domain.IRepository
{
    public interface IExamTermDbRepository : IObserver
    {
        List<ExamTerm> GetAll();
        ExamTerm GetById(int id);
        List<ExamTerm> GetAllExamTerms(int page, int pageSize, string sortCriteria, List<ExamTerm> exams);
        void Add(ExamTerm examTerm);
        ExamTerm Update(ExamTerm examTerm);
        void Remove(ExamTerm examTerm);
        void Delete(int id);
        void Subscribe(IObserver observer);
        void Update();
    }
}