using LangLang.Repository;
using LangLang.Domain.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.Domain.Model.Enums;

namespace LangLang.Controller
{
    public class TeacherController
    {
        private readonly TeacherRepository _teachers;
        private readonly CourseRepository _courses;
        private readonly ExamTermRepository _examTerms;
        private readonly PenaltyPointDAO _penaltyPoints;

        public TeacherController()
        {
            _teachers = new TeacherRepository();
            _penaltyPoints = new PenaltyPointDAO();
        }
        public Course? GetCourseById(int courseId)
        {
            return _teachers.GetCourseById(courseId);
        }
        public ExamTerm? GetExamTermById(int examId)
        {
            return _teachers.GetExamTermById(examId);
        }
        public Mail? GetMailById(int mailId)
        {
            return _teachers.GetMailById(mailId);
        }
        public List<Course> GetAllCourses()
        {
            return _teachers.GetAllCourses();
        }
        public List<ExamTerm> GetAllExamTerms()
        {
            return _teachers.GetAllExamTerms();
        }
        public List<Mail> GetAllMail()
        {
            return _teachers.GetAllMail();
        }

        public ExamTerm? RemoveExamTerm(int id)
        {
            ExamTerm? examTerm = GetExamTermById(id);
            if (examTerm == null) return null;

            int courseId = examTerm.CourseID;
            Course? course = _courses.GetCourseById(courseId);
            course.ExamTerms.Remove(id);
            _courses.UpdateCourse(course);

            _examTerms.RemoveExamTerm(examTerm.ExamID);
            return examTerm;
        }

        public List<Mail> GetSentCourseMail(Teacher teacher, int courseId)
        {
            return _teachers.GetSentCourseMail(teacher, courseId);
        }
        public List<Mail> GetReceivedCourseMails(Teacher teacher, int courseId)
        {
            return _teachers.GetReceivedCourseMails(teacher, courseId);
        }


        public List<Course> GetAvailableCourses(Teacher teacher)
        {
            return _teachers.GetAvailableCourses(teacher);
        }

        public Mail SendMail(Mail mail)
        {
            return _teachers.SendMail(mail);
        }

        public Mail AnswerMail(int mailId)
        {
            return _teachers.AnswerMail(mailId);
        }

        public void DeleteMail(int mailId)
        {
            _teachers.RemoveMail(mailId);
        }

        public bool CheckExamOverlap(int ExamID, DateTime ExamDate)
        {
            int examDurationInMinutes = 240;

            DateTime examStartDateTime = ExamDate;
            DateTime examEndDateTime = examStartDateTime.AddMinutes(examDurationInMinutes);

            IEnumerable<dynamic> overlappingExams = _teachers.GetAllExamTerms()
                .Where(item =>
                {
                    bool isDifferentId = item.ExamID != ExamID;

                    DateTime itemExamDateTime = item.ExamTime;

                    bool isOverlap = isDifferentId && (itemExamDateTime < examEndDateTime && itemExamDateTime.AddMinutes(examDurationInMinutes) > examStartDateTime);

                    return isOverlap;
                });

            return !overlappingExams.Any();
        }

        public void Subscribe(IObserver observer)
        {
            _teachers.Subscribe(observer);
        }

        public bool IsStudentAccepted(Student student, int courseId)
        {
            return _teachers.IsStudentAccepted(student, courseId);
        }
        public List<ExamTerm> GetAvailableExamTerms(Teacher teacher)
        {
            return _teachers.GetAvailableExamTerms(teacher);
        }

        public List<PenaltyPoint> GetAllPenaltyPoints()
        {
            return _penaltyPoints.GetAllPenaltyPoints();
        }
    }
}
