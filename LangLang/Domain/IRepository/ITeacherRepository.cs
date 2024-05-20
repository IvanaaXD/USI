using LangLang.Domain.Model;
using System.Collections.Generic;

namespace LangLang.Domain.IRepository
{
    public interface ITeacherRepository 
    {
        Mail SendMail(Mail mail);
        Mail AnswerMail(int mailId);
        Mail? RemoveMail(int id);
        Course? GetCourseById(int id);
        ExamTerm GetExamTermById(int id);
        Mail? GetMailById(int id);
        List<Course> GetAllCourses();
        List<ExamTerm> GetAllExamTerms();
        Course GetCourseByExamId(int id);
        List<Mail> GetAllMail();
        List<Mail> GetSentCourseMail(Teacher teacher, int courseId);
        List<Mail> GetReceivedCourseMails(Teacher teacher, int courseId);
        List<Course> GetAvailableCourses(Teacher teacher);
        List<ExamTerm> GetAvailableExamTerms(Teacher teacher);
        string FindLanguageAndLevel(int courseID);
        bool IsStudentAccepted(Student student, int courseId);
    }
}
