using LangLang.Domain.IUtility;
using LangLang.Domain.Model;
using LangLang.DTO;
using System;

namespace LangLang.Domain.Utility
{
    public class TeacherGradeStudentStrategy: IMailStrategy
    {
        public string GenerateMailMessage(CourseGradeDTO studentCourseGrade, Course course)
        {
            return $"Your final grade from course  {course.Language.ToString()}  {course.Level.ToString()}" +
                   $" is {studentCourseGrade.StudentActivityValue.ToString()} for your activity on course," +
                   $"and {studentCourseGrade.StudentKnowledgeValue.ToString()} for knowledge shown during course.";
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
