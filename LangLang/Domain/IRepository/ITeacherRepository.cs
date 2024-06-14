using LangLang.Domain.Model;
using System.Collections.Generic;
using LangLang.Observer;

namespace LangLang.Domain.IRepository
{
    public interface ITeacherRepository 
    {
        void Subscribe(IObserver observer);
    }
}
