using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangLang.Model.DAO;
using LangLang.Model;

namespace LangLang.Controller
{
    public class DirectorController
    {
        private readonly DirectorDAO _directorDao;

        public DirectorController()
        {
            _directorDao = new DirectorDAO();
        }

        public List<Teacher> GetAllTeachers()
        {
            return _directorDao.GetAllTeachers();
        }

        public void Add(Teacher teacher)
        {
            _directorDao.AddTeacher(teacher);
        }

        public void Delete(int teacherId)
        {
            _directorDao.RemoveTeacher(teacherId);
        }

        public void Update(Teacher teacher)
        {
            _directorDao.UpdateTeacher(teacher);

        }

        public void Subscribe(IObserver observer)
        {
            _directorDao.Subscribe(observer);
        }
    }
}

