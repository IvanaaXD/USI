using LangLang.Repository;
using LangLang.Domain.Model;
using LangLang.Observer;
using System.Collections.Generic;

namespace LangLang.Controller
{
    public class MailController
    {
        private readonly MailDAO _mails;

        public MailController()
        {
            _mails = new MailDAO();
        }

        public List<Mail> GetAllMails()
        {
            return _mails.GetAllMails();
        }

        public void Send(Mail mail)
        {
            _mails.SendMail(mail);
        }

        public void Delete(int mailId)
        {
            _mails.RemoveMail(mailId);
        }

/*        public void Update(Mail mail)
        {
            _mails.UpdateMail(mail);
        }*/
        public void Subscribe(IObserver observer)
        {
            _mails.Subscribe(observer);
        }
        public List<Mail> GetAllMail()
        {
            return _mails.GetAllMails();
        }
        public Mail PrepareQuitCourseMail(string senderEmail, string receiverEmail, int courseId)
        {
            return _mails.PrepareQuitCourseMail(senderEmail, receiverEmail, courseId);
        }
    }
}
