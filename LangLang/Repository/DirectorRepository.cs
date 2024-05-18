using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.Observer;
using LangLang.Storage;
using LangLang.Domain.Model;
using LangLang.Domain.IRepository;

namespace LangLang.Repository
{
    public class DirectorRepository : Subject, IDirectorRepository
    {
        private readonly List<Teacher> _teachers;
        private readonly List<Director> _director;
        private readonly Storage<Teacher> _storageTeacher;
        private readonly Storage<Director> _storageDirector;

        private TeacherDAO teacherDAO;

        public DirectorRepository()
        {
            _storageTeacher = new Storage<Teacher>("teachers.csv");
            _storageDirector = new Storage<Director>("director.csv");
            _teachers = _storageTeacher.Load();
            _director = _storageDirector.Load();
            teacherDAO = new TeacherDAO();
        }

        public Director GetDirector()
        {
            return _director.Find(d => d.Id == 0);
        }

        private int GenerateId()
        {
            if (_teachers.Count == 0) return 0;
            return _teachers.Last().Id + 1;
        }

        public Teacher AddTeacher(Teacher teacher)
        {
            teacher.Id = GenerateId();
            _teachers.Add(teacher);
            _storageTeacher.Save(_teachers);
            NotifyObservers();
            return teacher;
        }

        public Teacher UpdateTeacher(Teacher teacher)
        {
            Teacher oldTeacher = GetTeacherById(teacher.Id);
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

        public Teacher RemoveTeacher(int id)
        {
            Teacher teacher = GetTeacherById(id);
            if (teacher == null) return null;
            foreach (int courseid in teacher.CoursesId)
            {
                teacherDAO.RemoveCourse(courseid);
            }

            _teachers.Remove(teacher);
            _storageTeacher.Save(_teachers);
            NotifyObservers();
            return teacher;
        }

        public Teacher GetTeacherById(int id)
        {
            return _teachers.Find(t => t.Id == id);
        }

        public List<Teacher> GetAllTeachers()
        {
            return _teachers;
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
