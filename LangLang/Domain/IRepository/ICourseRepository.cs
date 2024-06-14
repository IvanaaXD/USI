using LangLang.Observer;
using System.Collections.Generic;
using LangLang.Domain.Model;

namespace LangLang.Domain.IRepository
{
    public interface ICourseRepository : IObserver
    {
        Course? GetById(int id);
        Course Add(Course course);
        Course Update(Course course);
        Course Remove(int id);
        List<Course> GetAll();
        List<Course> GetAllCourses(int page, int pageSize, string sortCriteria, List<Course> courses);
        void Subscribe(IObserver observer);
    }
}
