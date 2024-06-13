using LangLang.Domain.IRepository;
using LangLang.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Controller
{
    public class TeacherDbController
    {
        private readonly ITeacherDbRepository _teacherRepository;

        public TeacherDbController(ITeacherDbRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }
        /* TODO
        public List<Teacher> GetAllTeachers()
        {
            return 
        }*/

        public Teacher GetTeacherById(int id)
        {
            return _teacherRepository.GetById(id);
        }

        public void AddTeacher(Teacher teacher)
        {
            _teacherRepository.Add(teacher);
        }

        public void UpdateTeacher(Teacher teacher)
        {
            _teacherRepository.Update(teacher);
        }

        public void DeleteTeacher(int id)
        {
            _teacherRepository.Delete(id);
        }
    }
}
