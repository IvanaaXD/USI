using LangLang.Controller;
using LangLang.Model.Enums;
using LangLang.Observer;
using LangLang.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LangLang.Model.DAO
{
    public class MailDAO : Subject
    {
        private readonly List<Mail> _mails;
        private readonly Storage<Mail> _storage;

        private readonly StudentsController studentController;
        private readonly TeacherController teacherController;
        private readonly DirectorController directorController;

        public MailDAO()
        {
            _storage = new Storage<Mail>("mails.csv");
            _mails = _storage.Load();
        }

        private int GenerateId()
        {
            if (_mails.Count == 0) return 0;
            return _mails.Last().Id + 1;
        }

        public Mail SendMail(Mail mail)
        {
            mail.Id = GenerateId();
            _mails.Add(mail);
            _storage.Save(_mails);
            NotifyObservers();
            return mail;
        }

        /*public Mail? UpdateMail(Mail mail)
        {
            Mail? oldMail = GetMailById(mail.Id);
            if (oldMail == null) return null;

            // update

            _storage.Save(_mails);
            NotifyObservers();
            return oldMail;
        }*/

        public Mail? RemoveMail(int id)
        {
            Mail? mail = GetMailById(id);
            if (mail == null) return null;

            _mails.Remove(mail);
            _storage.Save(_mails);
            NotifyObservers();
            return mail;
        }
        public Mail? GetMailById(int id)
        {
            return _mails.Find(v => v.Id == id);
        }
        public List<Mail> GetAllMails()
        {
            return _mails;
        }
        public List<Mail> GetSentMails(Teacher teacher, int courseId)
        {
            List<Mail> filteredMails = new List<Mail>();

            foreach (Mail mail in _mails)
            {
                if (mail.Sender == teacher.Email)
                {
                    filteredMails.Add(mail);
                }
            }
            return filteredMails;
        }

        public List<Mail> GetReceivedMails(Teacher teacher, int courseId)
        {
            List<Mail> filteredMails = new List<Mail>();

            foreach (Mail mail in _mails)
            {
                if (mail.Receiver == teacher.Email)
                {
                    filteredMails.Add(mail);
                }
            }
            return filteredMails;
        }
        public Mail PrepareQuitCourseMail(string senderEmail, string receiverEmail, int courseId)
        {
            Mail mail = new Mail();
            mail.Sender = senderEmail;
            mail.Receiver = receiverEmail;
            mail.TypeOfMessage = TypeOfMessage.QuitCourseRequest;
            mail.DateOfMessage = DateTime.Now;
            mail.CourseId = courseId;
            mail.Answered = false;
            mail.Message = "";

            return mail;
        }

        public bool IsQuitCourseMailSent(string studentEmail, int courseId)
        {
            foreach(Mail mail in _mails)
            {
                if (mail.Sender == studentEmail && mail.CourseId == courseId && mail.TypeOfMessage == TypeOfMessage.QuitCourseRequest)
                    return true;
            }
            return false;
        }

    }
}
