using LangLang.Domain.Model;
using LangLang.Observer;
using System.Collections.Generic;
using System.Windows.Documents;

namespace LangLang.Domain.IRepository
{
    public interface ICourseDbRepository : IObserver
    {
        Course GetById(int id);
        Course Add(Course course);
        Course Update(Course course);
        void Remove(Course course);
        void Delete(int id);
        List<Course> GetAll();
        List<Course> GetAllCourses(int page, int pageSize, string sortCriteria, List<Course> coursesToPaginate);
        void Subscribe(IObserver observer);
    }
}
