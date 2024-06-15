using LangLang.Domain.IUtility;
using LangLang.Domain.Model;
using LangLang.DTO;
using System;

namespace LangLang.Domain.Utility
{
    public class PenaltyPointMailStrategy : IMailStrategy
    {
        public string GenerateMailMessage(int id, Student student, Course course, Teacher teacher)
        {
            if (id == 1)
            {
                return $"You have gotten a penalty point from course {course.Language.ToString()}" +
                       $" {course.Level.ToString()}. Reason: Student didn't attend a course class.";

            }
            else if (id == 2)
            {
                return $"You have gotten a penalty point from course {course.Language.ToString()}" +
                       $" {course.Level.ToString()}. Reason: Student is bothering other students during class.";
            }
            else 
            {
                return $"You have gotten a penalty point from course {course.Language.ToString()}" +
                       $" {course.Level.ToString()}. Reason: Student didn't do homework.";
            }
        }

        public string GenerateMailMessage(Student student, Course course, Director director)
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