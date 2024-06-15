using LangLang.Domain.Model;
using System.Collections.Generic;

namespace LangLang.Domain.IUtility
{
    public interface ISortStrategy
    {
        IEnumerable<ExamTerm> Sort(IEnumerable<ExamTerm> exams);
        IEnumerable<Course> Sort(IEnumerable<Course> courses);
        IEnumerable<Teacher> Sort(IEnumerable<Teacher> teachers);
    }
}
