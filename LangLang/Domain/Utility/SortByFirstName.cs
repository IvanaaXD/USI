using LangLang.Domain.IUtility;
using LangLang.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Domain.Utility
{
    public class SortByFirstName : ISortStrategy
    {
        public IEnumerable<Teacher> Sort(IEnumerable<Teacher> teachers)
        {
            return teachers.OrderBy(x => x.FirstName);
        }

        public IEnumerable<ExamTerm> Sort(IEnumerable<ExamTerm> exams)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Course> Sort(IEnumerable<Course> courses)
        {
            throw new NotImplementedException();
        }
    }
}
