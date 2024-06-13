using LangLang.Domain.IRepository;
using LangLang.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Controller
{
    public class CourseDbController
    {
        private readonly ICourseDbRepository _courseRepository;

        public CourseDbController(ICourseDbRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<IEnumerable<Course>> GetAllCourses()
        {
            return await _courseRepository.GetAllAsync();
        }

        public async Task<Course> GetCourseById(int id)
        {
            return await _courseRepository.GetByIdAsync(id);
        }

        public async Task AddCourse(Course course)
        {
            await _courseRepository.AddAsync(course);
        }

        public async Task UpdateCourse(Course course)
        {
            await _courseRepository.UpdateAsync(course);
        }

        public async Task DeleteCourse(int id)
        {
            await _courseRepository.DeleteAsync(id);
        }
    }
}
