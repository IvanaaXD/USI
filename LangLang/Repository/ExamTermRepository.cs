using System;
using System.Linq;
using LangLang.Observer;
using LangLang.Storage;
using System.Collections.Generic;
using LangLang.Domain.Model;
using LangLang.Domain.IRepository;

namespace LangLang.Repository
{
    public class ExamTermRepository : Subject, IExamTermRepository
    {
        private readonly List<ExamTerm> _examTerms;
        private readonly Storage<ExamTerm> _examTermsStorage;
        public ExamTermRepository()
        {
            _examTermsStorage = new Storage<ExamTerm>("exam.csv");
            _examTerms = _examTermsStorage.Load();
        }
        private int GenerateExamId()
        {
            if (_examTerms.Count == 0) return 0;
            return _examTerms.Last().ExamID + 1;
        }
        public ExamTerm AddExamTerm(ExamTerm examTerm)
        {
            examTerm.ExamID = GenerateExamId();
            _examTerms.Add(examTerm);
            _examTermsStorage.Save(_examTerms);
            NotifyObservers();
            return examTerm;
        }
        public ExamTerm? UpdateExamTerm(ExamTerm examTerm)
        {
            ExamTerm? oldExamTerm = GetExamTermById(examTerm.ExamID);
            if (oldExamTerm == null) return null;

            oldExamTerm.ExamTime = examTerm.ExamTime;
            oldExamTerm.MaxStudents = examTerm.MaxStudents;
            oldExamTerm.Language = examTerm.Language;   
            oldExamTerm.Level = examTerm.Level; 
            oldExamTerm.Confirmed = examTerm.Confirmed; 
            oldExamTerm.CurrentlyAttending = examTerm.CurrentlyAttending;   

            _examTermsStorage.Save(_examTerms);
            NotifyObservers();
            return oldExamTerm;
        }
        
        public ExamTerm? RemoveExamTerm(int id)
        {
            ExamTerm? examTerm = GetExamTermById(id);
            if (examTerm == null) return null;

            _examTerms.Remove(examTerm);
            _examTermsStorage.Save(_examTerms);
            NotifyObservers();
            return examTerm;
        }
        
        public ExamTerm GetExamTermById(int id)
        {
            return _examTerms.Find(et => et.ExamID == id);
        }
        public List<ExamTerm> GetAllExamTerms()
        {
            return _examTerms;
        }
        public List<ExamTerm> GetAllExamTerms(int page, int pageSize, string sortCriteria, List<ExamTerm> examsToPaginate)
        {
            IEnumerable<ExamTerm> exams = examsToPaginate;

            switch (sortCriteria)
            {
                case "Datetime":
                    exams = examsToPaginate.OrderBy(x => x.ExamTime);
                    break;
                case "Language":
                    exams = examsToPaginate.OrderBy(x => x.Language);
                    break;
                case "Level":
                    exams = examsToPaginate.OrderBy(x => x.Level);
                    break;
            }
            exams = exams.Skip((page - 1) * pageSize).Take(pageSize);
            return exams.ToList();
        }
        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
