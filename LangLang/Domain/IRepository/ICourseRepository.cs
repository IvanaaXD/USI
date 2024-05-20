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
        List<Course> GetCoursesByTeacher(int teacherId);
        void Subscribe(IObserver observer);
    }
}
