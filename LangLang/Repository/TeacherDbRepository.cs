using LangLang.Data;
using LangLang.Domain.IRepository;
using LangLang.Domain.Model;
using LangLang.Observer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LangLang.Repository
{
    public class TeacherDbRepository : ITeacherDbRepository
    {
        private readonly AppDbContext _context;
        private readonly Subject _subject;
        public TeacherDbRepository(AppDbContext context)
        {
            _context = context;
            _subject = new Subject();
        }
        private int GenerateExamId()
        {
            int maxId = _context.Teachers.Any() ? _context.Teachers.Max(e => e.Id) : 0;
            return maxId + 1;
        }
        public void Add(Teacher teacher)
        {
            teacher.Id = GenerateExamId();
            _context.Teachers.Add(teacher);
            _context.SaveChanges();
            _subject.NotifyObservers();
        }

        public Teacher GetById(int id)
        {
            var teacher = _context.Teachers.Find(id);
            if (teacher == null)
            {
                throw new KeyNotFoundException($"Teacher with ID {id} not found.");
            }

            return teacher;
        }
        public void Remove(Teacher teacher)
        {
            _context.Teachers.Remove(teacher);
            _context.SaveChanges();
            _subject.NotifyObservers();
        }
        public void Update(Teacher teacher)
        {
            _context.Teachers.Update(teacher);
            _context.SaveChanges();
            _subject.NotifyObservers();
        }

        public void Delete(int id)
        {
            var teacher = GetById(id);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
                _context.SaveChanges();
                _subject.NotifyObservers();
            }
            else
                throw new KeyNotFoundException($"Teacher with ID {id} not found.");
        }
        
    }
}
