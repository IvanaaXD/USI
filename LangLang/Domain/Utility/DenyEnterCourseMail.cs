using LangLang.Domain.IUtility;
using LangLang.Domain.Model;
using LangLang.DTO;
using System;

namespace LangLang.Domain.Utility
{
    public class DenyEnterCourseStrategy : IMailStrategy
    {
        public string GenerateMailMessage(Student student, Course course, Director director)
        {
            throw new NotImplementedException();
        }

        public string GenerateMailMessage(int id, Student student, Course course, Teacher teacher)
        {
            throw new NotImplementedException();
        }

        public string GenerateMailMessage(string rejectReason, Student student, Course course, Teacher teacher)
        {
            return $"You have been rejected from course {course.Language.ToString()}" +
                   $" {course.Level.ToString()}. Reason: {rejectReason}";
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
