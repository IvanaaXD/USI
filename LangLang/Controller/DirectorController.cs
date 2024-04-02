using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangLang.Model.DAO;
using LangLang.Model;
using LangLang.Model.Enums;

namespace LangLang.Controller
{
    public class DirectorController
    {
        private readonly DirectorDAO _directorDao;

        public DirectorController()
        {
            _directorDao = new DirectorDAO();
        }

        public Director GetDirector()
        {
            return _directorDao.GetDirector();
        }

        public Teacher GetTeacherById(int teacherId)
        {
            return _directorDao.GetTeacherById(teacherId);
        }

        public List<Teacher> GetAllTeachers()
        {
            return _directorDao.GetAllTeachers();
        }

        public bool IsEmailUnique(string email)
        {
            return _directorDao.IsEmailUnique(email);
        }

        public void Add(Teacher teacher)
        {
            _directorDao.AddTeacher(teacher);
        }

        public void Delete(int teacherId)
        {
            _directorDao.RemoveTeacher(teacherId);
        }

        public void AddCourseId(int courseId, int teacherId)
        {
            _directorDao.AddCourseId(courseId, teacherId);
        }

        public List<Course> GetAvailableCourses(int teacherId)
        {
            return _directorDao.GetAvailableCourses(teacherId);
        }

        public void Update(Teacher teacher)
        {
            _directorDao.UpdateTeacher(teacher);

        }

        public void Subscribe(IObserver observer)
        {
            _directorDao.Subscribe(observer);
        }

        public List<Teacher> FindTeachersByCriteria(Language language, LanguageLevel level, DateTime startedWork)
        {
            return _directorDao.FindTeachersByCriteria(language, level, startedWork);
        }
    }
}

