using LangLang.Domain.IUtility;
using LangLang.Domain.Model;
using LangLang.DTO;
using System;

namespace LangLang.Domain.Utility
{
    public class TopStudentsMailStrategy : IMailStrategy
    {
        public string GenerateMailMessage(Student student, Course course, Director director)
        {
            return $"Dear {student.FirstName}, Congratulations on being one of the top students in {course.Language} {course.Level}!" +
                   $" Your hard work and dedication have truly paid off. Thank you for your outstanding performance. Best regards, {director.FirstName}";
        }

        public string GenerateMailMessage(int id, Student student, Course course, Teacher teacher)
        {
            throw new NotImplementedException();
        }

        public string GenerateMailMessage(string rejectReason, Student student, Course course, Teacher teacher)
        {
            throw new NotImplementedException();
        }

        public string GenerateMailMessage(CourseGradeDTO studentCourseGrade, Course course)
        {
            throw new NotImplementedException();
        }

        public string GenerateMailMessage(Course course)
        {
            throw new NotImplementedException();
        }

        public string GenerateMailMessage(ExamTermGrade examTermGrade, ExamTerm examTerm)
        {
            throw new NotImplementedException();
        }
    }
}
