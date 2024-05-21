using LangLang.Controller;
using LangLang.Domain.Model.Enums;
using LangLang.Observer;
using LangLang.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.Domain.Model;
using LangLang.Domain.IRepository;

namespace LangLang.Repository
{
    public class MailRepository : Subject, IMailRepository
    {
        private readonly List<Mail> _mails;
        private readonly Storage<Mail> _storage;

        public MailRepository()
        {
            _storage = new Storage<Mail>("mails.csv");
            _mails = _storage.Load();
        }
        private int GenerateId()
        {
            if (_mails.Count == 0) return 0;
            return _mails.Last().Id + 1;
        }
        public Mail AddMail(Mail mail)
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
    }
}
