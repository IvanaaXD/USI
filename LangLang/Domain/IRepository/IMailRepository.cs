using LangLang.Domain.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Domain.IRepository
{
    public interface IMailRepository
    {
        Mail? GetMailById(int id);
        Mail AddMail(Mail Mail);
        Mail UpdateMail(Mail Mail);
        Mail RemoveMail(int id);
        List<Mail> GetAllMails();
        void Subscribe(IObserver observer);
    }
}
