using LangLang.Domain.IUtility;
using LangLang.Domain.Model;
using LangLang.DTO;
using System;

namespace LangLang.Domain.Utility
{
    public class StudentGradeStrategy : IMailStrategy
    {
        public string GenerateMailMessage(ExamTermGrade examTermGrade, ExamTerm examTerm)
        {

            if (examTermGrade.Value > 5)
                return $"You have passed exam {examTerm.Language.ToString()} {examTerm.Level.ToString()}" +
                       $"with grade {examTermGrade.ToString()}";
            else
                return $"You have failed exam {examTerm.Language.ToString()} {examTerm.Level.ToString()}";
        }

        public string GenerateMailMessage(Course course)
        {
            throw new NotImplementedException();
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
    }
}
