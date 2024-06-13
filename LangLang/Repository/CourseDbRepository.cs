using LangLang.Data;
using LangLang.Domain.IRepository;
using LangLang.Domain.Model;
using LangLang.Observer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LangLang.Repository
{
    public class CourseDbRepository : ICourseDbRepository
    {
        private readonly AppDbContext _context;
        private readonly Subject _subject;

        public CourseDbRepository(AppDbContext context)
        {
            _context = context;
            _subject = new Subject();
        }
        public void Add(Course course)
        {
            _context.Courses.Add(course);
            _context.SaveChanges();
            _subject.NotifyObservers();

        }

        public Course GetById(int id)
        {
            var course = _context.Courses.Find(id);
            if (course == null)
            {
                throw new KeyNotFoundException($"Course with ID {id} not found.");
            }

            return course;
        }
        public void Remove(Course course)
        {
            _context.Courses.Remove(course);
            _context.SaveChanges();
            _subject.NotifyObservers();
        }
        public void Update(Course course)
        {
            _context.Courses.Update(course);
            _context.SaveChanges();
            _subject.NotifyObservers();
        }

        public void Delete(int id)
        {
            var course = GetById(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                _context.SaveChanges();
                _subject.NotifyObservers();
            }
        }
    }
}