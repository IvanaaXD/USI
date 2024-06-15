using LangLang.Domain.IUtility;
using LangLang.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Domain.Utility
{
    public class SortByLanguage : ISortStrategy
    {
        public IEnumerable<ExamTerm> Sort(IEnumerable<ExamTerm> exams)
        {
            return exams.OrderBy(x => x.Language);
        }
        public IEnumerable<Course> Sort(IEnumerable<Course> courses)
        {
            return courses.OrderBy(x => x.Language);
        }

        public IEnumerable<Teacher> Sort(IEnumerable<Teacher> teachers)
        {
            throw new NotImplementedException();
        }
    }
}
