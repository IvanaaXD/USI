using LangLang.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.Domain.IUtility;

namespace LangLang.Domain.Utility
{
    public class SortByLastName:ISortStrategy
    {
        public IEnumerable<Teacher> Sort(IEnumerable<Teacher> teachers)
        {
            return teachers.OrderBy(x => x.LastName);
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
