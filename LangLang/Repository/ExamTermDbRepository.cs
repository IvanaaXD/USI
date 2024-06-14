using LangLang.Data;
using LangLang.Domain.IRepository;
using LangLang.Domain.Model;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
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
        public void Remove(ExamTerm examTerm)
        {
            _context.ExamTerms.Remove(examTerm);
            _context.SaveChanges();
            _subject.NotifyObservers();
        }
        public void Update(ExamTerm examTerm)
        {
            _context.ExamTerms.Update(examTerm);
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
    }
}