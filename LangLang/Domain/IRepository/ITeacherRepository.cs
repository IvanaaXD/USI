using LangLang.Domain.Model;
using System.Collections.Generic;
using LangLang.Observer;

namespace LangLang.Domain.IRepository
{
    public interface ITeacherRepository 
    {
        Course GetCourseByExamId(int id);
        string FindLanguageAndLevel(int courseID);
        void Subscribe(IObserver observer);
    }
}
