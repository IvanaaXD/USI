using LangLang.Observer;
using System.Collections.Generic;
using LangLang.Domain.Model;

namespace LangLang.Domain.IRepository
{
    public interface ICourseRepository : IObserver
    {
        Course? GetCourseById(int id);
        Course AddCourse(Course course);
        Course UpdateCourse(Course course);
        Course RemoveCourse(int id);
        List<Course> GetAllCourses();
        void Subscribe(IObserver observer);
    }
}
