using LangLang.Data;
using LangLang.Domain.IRepository;
using LangLang.Domain.Model;
using System.Collections.Generic;
using LangLang.Observer;
using System;
using System.Linq;
using LangLang.Domain.IUtility;

namespace LangLang.Repository
{
    public class ExamTermDbRepository : Subject, IExamTermDbRepository
    {
        private readonly AppDbContext _context;
        public ExamTermDbRepository(AppDbContext context)
        {
            _context = context;
        }
        private int GenerateExamId()
        {
            int maxId = _context.ExamTerms.Any() ? _context.ExamTerms.Max(e => e.ExamID) : 0;
            return maxId + 1;
        }
        public void Add(ExamTerm examTerm)
        {
            examTerm.ExamID = GenerateExamId();
            _context.ExamTerms.Add(examTerm);
            _context.SaveChanges();
            NotifyObservers();
        }

        public ExamTerm GetById(int id)
        {
            var examTerm = _context.ExamTerms.Find(id);
            if (examTerm == null)
            {
                throw new KeyNotFoundException($"ExamTerm with ID {id} not found.");
            }

            return examTerm;
        }
        public List<ExamTerm> GetAll()
        {
            return _context.ExamTerms.ToList();
        }
        public void Remove(ExamTerm examTerm)
        {
            _context.ExamTerms.Remove(examTerm);
            _context.SaveChanges();
            NotifyObservers();
        }
        public ExamTerm Update(ExamTerm examTerm)
        {
            ExamTerm? oldExamTerm = GetById(examTerm.ExamID);
            if (oldExamTerm == null)
            {
                throw new KeyNotFoundException($"ExamTerm with ID {examTerm.ExamID} not found.");
            }
            oldExamTerm.ExamTime = examTerm.ExamTime;
            oldExamTerm.MaxStudents = examTerm.MaxStudents;
            oldExamTerm.Language = examTerm.Language;
            oldExamTerm.Level = examTerm.Level;
            oldExamTerm.Confirmed = examTerm.Confirmed;
            oldExamTerm.CurrentlyAttending = examTerm.CurrentlyAttending;

            _context.SaveChanges();
            NotifyObservers();
            return oldExamTerm;
        }
        public void Delete(int id)
        {
            var examTerm = GetById(id);
            if (examTerm != null)
            {
                _context.ExamTerms.Remove(examTerm);
                _context.SaveChanges();
                NotifyObservers();
            }
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
        
        public List<ExamTerm> GetAllExamTerms(int page, int pageSize, ISortStrategy sortStrategy, List<ExamTerm> examsToPaginate)
        {
            IEnumerable<ExamTerm> exams = sortStrategy.Sort(examsToPaginate);
            exams = exams.Skip((page - 1) * pageSize).Take(pageSize);
            return exams.ToList();
        }
        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}