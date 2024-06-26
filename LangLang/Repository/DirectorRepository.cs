using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.Observer;
using LangLang.Storage;
using LangLang.Domain.Model;
using LangLang.Domain.IRepository;
using LangLang.Domain.IUtility;

namespace LangLang.Repository
{
    public class DirectorRepository : Subject, IDirectorRepository
    {
        private readonly List<Teacher> _teachers;
        private readonly List<Director> _director;
        private readonly Storage<Teacher> _storageTeacher;
        private readonly Storage<Director> _storageDirector;

        private CourseRepository courseRepository;

        public DirectorRepository()
        {
            _storageTeacher = new Storage<Teacher>("teachers.csv");
            _storageDirector = new Storage<Director>("director.csv");
            _teachers = _storageTeacher.Load();
            _director = _storageDirector.Load();
            courseRepository = new CourseRepository();
        }

        public Director? GetDirector()
        {
            return _director.Find(d => d.Id == 0);
        }

        private int GenerateId()
        {
            if (_teachers.Count == 0) return 0;
            return _teachers.Last().Id + 1;
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
        public Teacher Add(Teacher teacher)
        {
            teacher.Id = GenerateId();
            _teachers.Add(teacher);
            _storageTeacher.Save(_teachers);
            NotifyObservers();
            return teacher;
        }

        public Teacher? Update(Teacher? teacher)
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

            _storageTeacher.Save(_teachers);
            NotifyObservers();
            return oldTeacher;
        }

        public Teacher? Remove(int id)
        {
            Teacher? teacher = GetById(id);
            if (teacher == null) return null;

            if (teacher.CoursesId != null)
            {
                foreach (int courseId in teacher.CoursesId)
                    courseRepository.Remove(courseId);
            }

            _teachers.Remove(teacher);
            _storageTeacher.Save(_teachers);
            NotifyObservers();
            return teacher;
        }


        public Teacher? GetById(int id)
        {
            return _teachers.Find(t => t.Id == id);
        }

        public List<Teacher> GetAll()
        {
            return _teachers;
        }

        public void Update()
        {
            throw new NotImplementedException();
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
        public List<Teacher> GetAllTeachers(int page, int pageSize, ISortStrategy sortStrategy, List<Teacher> teachersToPaginate)
        {
            IEnumerable<Teacher> teachers = sortStrategy.Sort(teachersToPaginate);
            teachers = teachers.Skip((page - 1) * pageSize).Take(pageSize);
            return teachers.ToList();
        }
    }
}
