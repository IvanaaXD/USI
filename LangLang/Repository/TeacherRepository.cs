using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using LangLang.Domain.Model;
using LangLang.Controller;
using LangLang.Domain.Model.Enums;
using LangLang.Observer;
using LangLang.Storage;
using LangLang.Domain.IRepository;

namespace LangLang.Repository
{
    public class TeacherRepository : Subject
    {
        private readonly List<Course> _courses;
        private readonly Storage<Course> _courseStorage;
        private readonly List<ExamTerm> _examTerms;
        private readonly Storage<ExamTerm> _examTermsStorage;
        private readonly List<Mail> _mails;
        private readonly Storage<Mail> _mailsStorage;

        public TeacherRepository()
        {
            _courseStorage = new Storage<Course>("course.csv");
            _courses = _courseStorage.Load();
            _examTermsStorage = new Storage<ExamTerm>("exam.csv");
            _examTerms = _examTermsStorage.Load();
            _mailsStorage = new Storage<Mail>("mails.csv");
            _mails = _mailsStorage.Load();
        }

        private int GenerateMailId()
        {
            if (_mails.Count == 0) return 0;
            return _mails.Last().Id + 1;
        }

        public Mail SendMail(Mail mail)
        {
            mail.Id = GenerateMailId();
            _mails.Add(mail);
            _mailsStorage.Save(_mails);
            NotifyObservers();
            return mail;
        }
        public Mail AnswerMail(int mailId)
        {
            Mail? mail = GetMailById(mailId);
            mail.Answered = true;
            _mailsStorage.Save(_mails);
            NotifyObservers();
            return mail;
        }

        public Mail? RemoveMail(int id)
        {
            Mail? mail = GetMailById(id);
            if (mail == null) return null;

            _mails.Remove(mail);
            _mailsStorage.Save(_mails);
            NotifyObservers();
            return mail;
        }

        public Course? GetCourseById(int id)
        {
            return _courses.Find(v => v.Id == id);
        }

        public ExamTerm GetExamTermById(int id)

        {
            return _examTerms.Find(et => et.ExamID == id);
        }
        public Mail? GetMailById(int id)
        {
            return _mails.Find(v => v.Id == id);
        }
        public List<Course> GetAllCourses()
        {
            return _courses;
        }

        public List<ExamTerm> GetAllExamTerms()
        {
            return _examTerms;
        }

        public Course GetCourseByExamId(int id)
        {
            Course course = null;
            List<ExamTerm> examTerms = GetAllExamTerms();
            foreach (ExamTerm examTerm in examTerms)
            {
                if (examTerm.ExamID == id)
                {

                    course = GetCourseById(examTerm.CourseID);
                    break;
                }
            }
            return course;
        }


        public List<Mail> GetAllMail()
        {
            return _mails;
        }


        public List<Mail> GetSentCourseMail(Teacher teacher, int courseId)
        {
            List<Mail> filteredMails = new List<Mail>();

            foreach (Mail mail in _mails)
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

            foreach (Mail mail in _mails)
            {
                if (mail.Receiver == teacher.Email && mail.CourseId == courseId)
                {
                    filteredMails.Add(mail);
                }
            }
            return filteredMails;
        }

        public List<Course> GetAvailableCourses(Teacher teacher)
        {
            List<Course> allCourses = GetAllCourses();
            List<int> allTeacherCourses = teacher.CoursesId;

            List<Course> availableCourses = new List<Course>();

            foreach (Course course in allCourses)
            {
                if (allTeacherCourses.Contains(course.Id))
                {
                    availableCourses.Add(course);
                }
            }
            return availableCourses;
        }

        public List<ExamTerm> GetAvailableExamTerms(Teacher teacher)
        {
            List<ExamTerm> allExamTerms = GetAllExamTerms();
            List<Course> allTeacherCourses = GetAvailableCourses(teacher);

            List<ExamTerm> availableExamTerms = new();
            List<int> examTermIds = new();

            foreach (Course course in allTeacherCourses)
            {
                foreach (int examId in course.ExamTerms)
                {
                    examTermIds.Add(examId);
                }
            }

            foreach (ExamTerm examTerm in allExamTerms)
            {
                if (examTermIds.Contains(examTerm.ExamID))
                {
                    availableExamTerms.Add(examTerm);
                }
            }

            return availableExamTerms;
        }

        public String FindLanguageAndLevel(int courseID)
        {
            String res = "";

            Course course = GetAllCourses().FirstOrDefault(c => c.Id == courseID);

            if (course != null)
            {
                res = $"{course.Language}, {course.Level}";
            }
            else
            {
                res = "Language and level not found";
            }

            return res;
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
