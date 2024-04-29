using LangLang.Observer;
using System;
using System.Collections.Generic;
using LangLang.Model.DAO;
using LangLang.Model;
using LangLang.Model.Enums;

namespace LangLang.Controller
{
    public class DirectorController
    {
        private readonly DirectorDAO _directors;

        public DirectorController()
        {
            _directors = new DirectorDAO();
        }

        public Director GetDirector()
        {
            return _directors.GetDirector();
        }

        public Teacher GetTeacherById(int teacherId)
        {
            return _directors.GetTeacherById(teacherId);
        }

        public List<Teacher> GetAllTeachers()
        {
            return _directors.GetAllTeachers();
        }

        public bool IsEmailUnique(string email)
        {
            return _directors.IsEmailUnique(email);
        }

        public void Add(Teacher teacher)
        {
            _directors.AddTeacher(teacher);
        }

        public void Delete(int teacherId)
        {
            _directors.RemoveTeacher(teacherId);
        }

        public List<Course> GetAvailableCourses(int teacherId)
        {
            return _directors.GetAvailableCourses(teacherId);
        }

        public void Update(Teacher teacher)
        {
            _directors.UpdateTeacher(teacher);

        }

        public void Subscribe(IObserver observer)
        {
            _directors.Subscribe(observer);
        }

        public List<Teacher> FindTeachersByCriteria(Language language, LanguageLevel level, DateTime startedWork)
        {
            return _directors.FindTeachersByCriteria(language, level, startedWork);
        }
    }
}
