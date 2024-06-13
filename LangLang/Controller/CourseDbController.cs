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

       
        public Course GetCourseById(int id)
        {
            return _courseRepository.GetById(id);
        }

        public void AddCourse(Course course)
        {
            _courseRepository.Add(course);
        }

        public void UpdateCourse(Course course)
        {
            _courseRepository.Update(course);
        }

        public void DeleteCourse(int id)
        {
            _courseRepository.Delete(id);
        }
    }
}
