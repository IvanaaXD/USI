using LangLang.Data;
using LangLang.Domain.IRepository;
using LangLang.Domain.Model;
using LangLang.Observer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LangLang.Repository
{
    public class CourseDbRepository : Subject, ICourseDbRepository
    {
        private readonly AppDbContext _context;

        public CourseDbRepository(AppDbContext context)
        {
            _context = context;
        }
        private int GenerateExamId()
        {
            int maxId = _context.Courses.Any() ? _context.Courses.Max(e => e.Id) : 0;
            return maxId + 1;
        }
        public Course Add(Course course)
        {
            course.Id = GenerateExamId();
            _context.Courses.Add(course);
            _context.SaveChanges();
            NotifyObservers();
            return course;

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

        public List<Course> GetAll()
        {
            return _context.Courses.ToList();
        }

        public void Remove(Course course)
        {
            _context.Courses.Remove(course);
            _context.SaveChanges();
            NotifyObservers();
        }
        public Course Update(Course course)
        {
            Course? oldCourse = GetById(course.Id);
            if (oldCourse == null)
            {
                throw new KeyNotFoundException($"Course with ID {course.Id} not found.");
                return null;
            }
            oldCourse.Language = course.Language;
            oldCourse.Level = course.Level;
            oldCourse.Duration = course.Duration;
            oldCourse.WorkDays = course.WorkDays;
            oldCourse.StartDate = course.StartDate;
            oldCourse.IsOnline = course.IsOnline;
            oldCourse.CurrentlyEnrolled = course.CurrentlyEnrolled;
            oldCourse.MaxEnrolledStudents = course.MaxEnrolledStudents;

            _context.SaveChanges();
            NotifyObservers();
            return oldCourse;
        }

        public void Delete(int id)
        {
            var course = GetById(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                _context.SaveChanges();
                NotifyObservers();
            }
        }
        public List<Course> GetAllCourses(int page, int pageSize, string sortCriteria, List<Course> coursesToPaginate)
        {
            IEnumerable<Course> courses = coursesToPaginate;

            switch (sortCriteria)
            {
                case "StartDate":
                    courses = courses.OrderBy(x => x.StartDate);
                    break;
                case "Language":
                    courses = courses.OrderBy(x => x.Language);
                    break;
                case "Level":
                    courses = courses.OrderBy(x => x.Level);
                    break;
            }

            courses = courses.Skip((page - 1) * pageSize).Take(pageSize);

            return courses.ToList();
        }
        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}