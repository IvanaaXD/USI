using LangLang.Domain.Model;
using LangLang.Domain.Model.Enums;
using LangLang.Observer;
using System;
using System.Collections.Generic;

namespace LangLang.Domain.IRepository
{
    public interface IExamTermRepository : IObserver
    {
        ExamTerm? AddExamTerm(ExamTerm examTerm);
        ExamTerm? UpdateExamTerm(ExamTerm examTerm);
        ExamTerm? GetExamTermById(int id);
        List<ExamTerm> GetAllExamTerms();
        ExamTerm ConfirmExamTerm(int examTermId);
        string FindLanguageAndLevel(int courseID);
        void DecrementExamTermCurrentlyAttending(int examTermId);
        bool CheckTeacherExamOverlapsCourses(ExamTerm examTerm, Teacher teacher);
        bool CheckTeacherExamsOverlap(ExamTerm examTerm, Teacher teacher);
        List<ExamTerm> FindExamTermsByCriteria(Language? language, LanguageLevel? level, DateTime? examDate);
        void Subscribe(IObserver observer);
    }
}
