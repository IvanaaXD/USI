using LangLang.Controller;
using LangLang.Domain.Model.Enums;
using LangLang.Observer;
using LangLang.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.Domain.Model;

namespace LangLang.Repository
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

        public Mail? UpdateMail(Mail mail)
        {
            Mail? oldMail = GetMailById(mail.Id);
            if (oldMail == null) return null;

            oldMail.Sender = mail.Sender;
            oldMail.Receiver = mail.Receiver;
            oldMail.CourseId = mail.CourseId;
            oldMail.TypeOfMessage = mail.TypeOfMessage;
            oldMail.DateOfMessage = mail.DateOfMessage;
            oldMail.Message = mail.Message;
            oldMail.Answered = mail.Answered;

            _storage.Save(_mails);
            NotifyObservers();
            return oldMail;
        }

        public void SetMailToAnswered(Mail mail)
        {
            mail.Answered = true;
            UpdateMail(mail);
        }

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
        public List<Mail> GetSentMails(Student student)
        {
            List<Mail> filteredMails = new List<Mail>();

            foreach (Mail mail in _mails)
            {
                if (mail.Sender == student.Email)
                {
                    filteredMails.Add(mail);
                }
            }
            return filteredMails;
        }

        public List<Mail> GetReceivedMails(Student student)
        {
            List<Mail> filteredMails = new List<Mail>();

            foreach (Mail mail in _mails)
            {
                if (mail.Receiver == student.Email)
                {
                    filteredMails.Add(mail);
                }
            }
            return filteredMails;
        }
        public List<Mail> GetUnreadReceivedMails(Student student)
        {
            List<Mail> filteredMails = new List<Mail>();

            foreach (Mail mail in _mails)
            {
                if (mail.Receiver == student.Email && mail.Answered == false)
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
                if (mail.Sender == studentEmail && mail.CourseId == courseId && mail.TypeOfMessage == TypeOfMessage.QuitCourseRequest)
                    return true;
            return false;
        }
        public bool IsTopStudentsMailSent(int courseId)
        {
            foreach (Mail mail in _mails)
                if (mail.CourseId == courseId && mail.TypeOfMessage == TypeOfMessage.TopStudentsMessage)
                    return true;
            return false;
        }
    }
}
