using LangLang.Domain.IUtility;
using LangLang.Domain.Model;
using LangLang.DTO;
using System.Windows.Input;

public class MailMessageGenerator
{
    private readonly IMailStrategy _mailStrategy;

    public MailMessageGenerator(IMailStrategy mailStrategy)
    {
        _mailStrategy = mailStrategy;
    }

    public string GenerateMailMessage(Student student, Course course, Director director)
    {
        return _mailStrategy.GenerateMailMessage(student, course, director);
    }

    public string GenerateMailMessage(int id, Student student, Course course, Teacher teacher)
    {
        return _mailStrategy.GenerateMailMessage(id, student, course, teacher);
    }

    public string GenerateMailMessage(string rejectReason, Student student, Course course, Teacher teacher)
    {
        return _mailStrategy.GenerateMailMessage(rejectReason, student, course, teacher);
    }

    public string GenerateMailMessage(CourseGradeDTO studentCourseGrade, Course course)
    {
        return _mailStrategy.GenerateMailMessage(studentCourseGrade, course);
    }

    public string GenerateMailMessage(Course course)
    {
        return _mailStrategy.GenerateMailMessage(course);
    }

    public string GenerateMailMessage(ExamTermGrade examTermGrade, ExamTerm examTerm)
    {
        return _mailStrategy.GenerateMailMessage(examTermGrade, examTerm);
    }
}
