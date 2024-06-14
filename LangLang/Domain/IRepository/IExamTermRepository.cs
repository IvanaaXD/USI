using LangLang.Domain.Model;
using LangLang.Domain.Model.Enums;
using LangLang.Observer;
using System;
using System.Collections.Generic;

namespace LangLang.Domain.IRepository
{
    public interface IExamTermRepository : IObserver
    {
        ExamTerm? Add(ExamTerm examTerm);
        ExamTerm? Update(ExamTerm examTerm);
        ExamTerm? GetById(int id);
        List<ExamTerm> GetAll();
        ExamTerm? Remove(int id);
        List<ExamTerm> GetAllExamTerms(int page, int pageSize, string sortCriteria, List<ExamTerm> exams);
        void Subscribe(IObserver observer);
    }
}
