using iText.Commons.Utils;
using LangLang.Data;
using LangLang.Domain.IRepository;
using LangLang.Domain.Model;
using LangLang.Observer;
using LangLang.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LangLang.Repository
{
    public class DirectorDbRepository : Subject, IDirectorDbRepository
    {
        private readonly AppDbContext _context;
        private readonly List<Director> _director;
        private readonly Storage<Director> _storageDirector;

        public DirectorDbRepository(AppDbContext context)
        {
            _context = context;
            _storageDirector = new Storage<Director>("director.csv");
            _director = _storageDirector.Load();
        }
        private int GenerateTeacherId()
        {
            int maxId = _context.Teachers.Any() ? _context.Teachers.Max(e => e.Id) : 0;
            return maxId + 1;
        }
        public Director? GetDirector()
        {
            return _director.Find(d => d.Id == 0);
        }
        public Teacher Add(Teacher teacher)
        {
            teacher.Id = GenerateTeacherId();
            _context.Teachers.Add(teacher);
            _context.SaveChanges();
            NotifyObservers();
            return teacher;
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
        public List<Teacher> GetAll()
        {
            return _context.Teachers.ToList();
        }
        public Teacher? Remove(int teacherId)
        {
            var teacher = GetById(teacherId);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
                _context.SaveChanges();
                NotifyObservers();
            }
            return teacher;
        }
        public Teacher? Update(Teacher teacher)
        {
            Teacher? oldTeacher = GetById(teacher.Id);
            if (oldTeacher == null) return null;

            oldTeacher.FirstName = teacher.FirstName;
            oldTeacher.LastName = teacher.LastName;
            oldTeacher.Gender = teacher.Gender;
            oldTeacher.DateOfBirth = teacher.DateOfBirth;
            oldTeacher.PhoneNumber = teacher.PhoneNumber;
            oldTeacher.Email = teacher.Email;
            oldTeacher.Password = teacher.Password;
            oldTeacher.Title = teacher.Title;
            oldTeacher.Languages = teacher.Languages;
            oldTeacher.LevelOfLanguages = teacher.LevelOfLanguages;
            oldTeacher.StartedWork = teacher.StartedWork;
            oldTeacher.AverageRating = teacher.AverageRating;
            oldTeacher.CoursesId = teacher.CoursesId;

            _context.SaveChanges();
            NotifyObservers();
            return oldTeacher;
        }
        public Director? UpdateDirector(Director? director)
        {
            Director? oldDirector = GetDirector();
            if (oldDirector == null) return null;

            oldDirector.FirstName = director.FirstName;
            oldDirector.LastName = director.LastName;
            oldDirector.Gender = director.Gender;
            oldDirector.DateOfBirth = director.DateOfBirth;
            oldDirector.PhoneNumber = director.PhoneNumber;
            oldDirector.Email = director.Email;
            oldDirector.Password = director.Password;
            oldDirector.Title = director.Title;
            oldDirector.CoursesId = director.CoursesId;
            oldDirector.ExamsId = director.ExamsId;

            _storageDirector.Save(_director);
            NotifyObservers();
            return oldDirector;
        }
        public List<Teacher> GetAllTeachers(int page, int pageSize, string sortCriteria, List<Teacher> teachersToPaginate)
        {
            IEnumerable<Teacher> teachers = teachersToPaginate;

            switch (sortCriteria)
            {
                case "FirstName":
                    teachers = teachers.OrderBy(x => x.FirstName);
                    break;
                case "LastName":
                    teachers = teachers.OrderBy(x => x.LastName);
                    break;
                case "StartedWork":
                    teachers = teachers.OrderBy(x => x.StartedWork);
                    break;
            }

            teachers = teachers.Skip((page - 1) * pageSize).Take(pageSize);

            return teachers.ToList();
        }


    }
}
