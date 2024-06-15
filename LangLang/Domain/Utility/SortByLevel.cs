using LangLang.Domain.IUtility;
using LangLang.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Domain.Utility
{
    public class SortByLevel : ISortStrategy
    {
        public IEnumerable<ExamTerm> Sort(IEnumerable<ExamTerm> exams)
        {
            return exams.OrderBy(x => x.Level);
        }
        public IEnumerable<Course> Sort(IEnumerable<Course> exams)
        {
            return exams.OrderBy(x => x.Level);
        }
    }
}
