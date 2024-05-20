﻿using LangLang.Domain.Model;
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
        ExamTerm? RemoveExamTerm(int id);
        void Subscribe(IObserver observer);
    }
}
