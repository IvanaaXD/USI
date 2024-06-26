using LangLang.Domain.IUtility;
using LangLang.Domain.Model;
using LangLang.DTO;
using System;

namespace LangLang.Domain.Utility
{
    public class AcceptEnterCourseStrategy : IMailStrategy
    {
        public string GenerateMailMessage(Course course)
        {
            return $"You have been accepted to course {course.Language.ToString()} {course.Level.ToString()}";
        }

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
            throw new NotImplementedException();
        }

        public string GenerateMailMessage(CourseGradeDTO studentCourseGrade, Course course)
        {
            throw new NotImplementedException();
        }

        public string GenerateMailMessage(ExamTermGrade examTermGrade, ExamTerm examTerm)
        {
            throw new NotImplementedException();
        }
    }
}

