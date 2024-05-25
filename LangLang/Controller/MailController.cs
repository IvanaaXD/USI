﻿using LangLang.Repository;
using LangLang.Domain.Model;
using LangLang.Observer;
using System.Collections.Generic;
using LangLang.Domain.Model.Enums;
using System;
using LangLang.Domain.IRepository;

namespace LangLang.Controller
{
    public class MailController
    {
        private readonly IMailRepository _mails;

        public MailController()
        {
            _mails = Injector.CreateInstance<IMailRepository>();
        }
        public void Send(Mail mail)
        {
            _mails.AddMail(mail);
        }
        public void Delete(int mailId)
        {
            _mails.RemoveMail(mailId);
        }
        public void Update(Mail mail)
        {
            _mails.UpdateMail(mail);
        }
        public void Subscribe(IObserver observer)
        {
            _mails.Subscribe(observer);
        }
        public Mail? GetMailById(int id)
        {
            return _mails.GetMailById(id);
        }
        public List<Mail> GetAllMail()
        {
            return _mails.GetAllMails();
        }
        public void SetMailToAnswered(Mail mail)
        {
            mail.Answered = true;
            Update(mail);
        }
        public void ConstructMail(Person sender, Person receiver, Course course, TypeOfMessage type, string body)
        {
            Send(new Mail
            {
                Sender = sender.Email,
                Receiver = receiver.Email,
                CourseId = course.Id,
                TypeOfMessage = type,
                DateOfMessage = DateTime.Now,
                Message = body,
                Answered = false
            });
        }
        /*public List<Mail> GetSentMails(Student student)
        {
            List<Mail> filteredMails = new List<Mail>();

            foreach (Mail mail in _mails.GetAllMails())
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

            foreach (Mail mail in _mails.GetAllMails())
            {
                if (mail.Receiver == student.Email)
                {
                    filteredMails.Add(mail);
                }
            }
            return filteredMails;
        }*/
        public List<Mail> GetUnreadReceivedMails(Student student)
        {
            List<Mail> filteredMails = new List<Mail>();

            foreach (Mail mail in _mails.GetAllMails())
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
            foreach (Mail mail in _mails.GetAllMails())
                if (mail.Sender == studentEmail && mail.CourseId == courseId && mail.TypeOfMessage == TypeOfMessage.QuitCourseRequest)
                    return true;
            return false;
        }
        public bool IsTopStudentsMailSent(int courseId)
        {
            foreach (Mail mail in _mails.GetAllMails())
                if (mail.CourseId == courseId && mail.TypeOfMessage == TypeOfMessage.TopStudentsMessage)
                    return true;
            return false;
        }
        public List<Mail> GetSentCourseMail(Teacher teacher, int courseId)
        {
            List<Mail> filteredMails = new List<Mail>();

            foreach (Mail mail in _mails.GetAllMails())
            {
                if (mail.Sender == teacher.Email && mail.CourseId == courseId)
                {
                    filteredMails.Add(mail);
                }
            }
            return filteredMails;
        }
        public List<Mail> GetReceivedCourseMails(Teacher teacher, int courseId)
        {
            List<Mail> filteredMails = new List<Mail>();

            foreach (Mail mail in _mails.GetAllMails())
            {
                if (mail.Receiver == teacher.Email && mail.CourseId == courseId)
                {
                    filteredMails.Add(mail);
                }
            }
            return filteredMails;
        }
        public bool IsStudentAccepted(Student student, int courseId)
        {
            List<Mail> sentMail = GetAllMail();
            foreach (Mail mail in sentMail)
            {
                if (mail.Receiver == student.Email && mail.CourseId == courseId && mail.TypeOfMessage == TypeOfMessage.AcceptEnterCourseRequestMessage)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
