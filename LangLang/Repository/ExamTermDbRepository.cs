using LangLang.Data;
using LangLang.Domain.IRepository;
using LangLang.Domain.Model;
using System.Collections.Generic;
using LangLang.Observer;
using System;
using System.Linq;

namespace LangLang.Repository
{
    public class ExamTermDbRepository : IExamTermDbRepository
    {
        private readonly AppDbContext _context;
        private readonly Subject _subject;
        public ExamTermDbRepository(AppDbContext context)
        {
            _context = context;
            _subject = new Subject();
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
            _subject.NotifyObservers();
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
            _subject.NotifyObservers();
        }
        public void Update(ExamTerm examTerm)
        {
            var existingExamTerm = _context.ExamTerms.Find(examTerm.ExamID);
            if (existingExamTerm == null)
            {
                throw new KeyNotFoundException($"ExamTerm with ID {examTerm.ExamID} not found.");
            }
            existingExamTerm.ExamTime = examTerm.ExamTime;
            existingExamTerm.MaxStudents = examTerm.MaxStudents;
            existingExamTerm.Language = examTerm.Language;
            existingExamTerm.Level = examTerm.Level;
            existingExamTerm.Confirmed = examTerm.Confirmed;
            existingExamTerm.CurrentlyAttending = examTerm.CurrentlyAttending;

            _context.SaveChanges();
            _subject.NotifyObservers();
        }


        public void Delete(int id)
        {
            var examTerm = GetById(id);
            if (examTerm != null)
            {
                _context.ExamTerms.Remove(examTerm);
                _context.SaveChanges();
                _subject.NotifyObservers();
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
        public void Update()
        {
            throw new NotImplementedException();
        }
        public void Subscribe(IObserver observer)
        {

        }
    }
}