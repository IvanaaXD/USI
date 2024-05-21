using LangLang.Domain.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Domain.IRepository
{
    public interface ICourseGradeRepository : IObserver
    {
        CourseGrade AddGrade(CourseGrade grade);
        CourseGrade? UpdateGrade(CourseGrade grade);
        CourseGrade? RemoveGrade(int id);
        bool IsStudentGraded(int studentId, int courseId);
        CourseGrade? GetGradeById(int id);
        CourseGrade? GetCourseGradeByStudentTeacher(int studentId, int teacherId, int courseId);
        CourseGrade? GetCourseGradeByStudent(int studentId, int courseId);
        List<CourseGrade> GetCourseGradesByTeacherCourse(int teacherId, int courseId);
        List<CourseGrade> GetAllCourseGrades();
        void Subscribe(IObserver observer);

    }
}
