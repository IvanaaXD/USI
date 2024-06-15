
using LangLang.Domain.Model;
using LangLang.DTO;

namespace LangLang.Domain.IUtility
{
    public interface IMailStrategy
    {
        string GenerateMailMessage(Student student, Course course, Director director);

        string GenerateMailMessage(int id, Student student, Course course, Teacher teacher);

        string GenerateMailMessage(string rejectReason, Student student, Course course, Teacher teacher);

        string GenerateMailMessage(CourseGradeDTO studentCourseGrade, Course course);

    }

}
