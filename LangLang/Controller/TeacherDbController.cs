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

        public async Task<IEnumerable<Teacher>> GetAllTeachers()
        {
            return await _teacherRepository.GetAllAsync();
        }

        public async Task<Teacher> GetTeacherById(int id)
        {
            return await _teacherRepository.GetByIdAsync(id);
        }

        public async Task AddTeacher(Teacher teacher)
        {
            await _teacherRepository.AddAsync(teacher);
        }

        public async Task UpdateTeacher(Teacher teacher)
        {
            await _teacherRepository.UpdateAsync(teacher);
        }

        public async Task DeleteTeacher(int id)
        {
            await _teacherRepository.DeleteAsync(id);
        }
    }
}
