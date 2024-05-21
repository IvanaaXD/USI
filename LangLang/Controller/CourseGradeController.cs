using LangLang.Domain.IRepository;
using LangLang.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Controller
{
    public class CourseGradeController
    {
        private readonly ICourseGradeRepository _courseGrades;

        public CourseGradeController(ICourseGradeRepository courseGrades)
        {
            _courseGrades = courseGrades ?? throw new ArgumentNullException(nameof(_courseGrades));
        }
        public CourseGradeController()
        {
            _courseGrades = Injector.CreateInstance<ICourseGradeRepository>();
        }
        public CourseGrade AddGrade(CourseGrade grade)
        {
            return _courseGrades.AddGrade(grade);
        }
        public CourseGrade? UpdateGrade(CourseGrade grade)
        {
            return _courseGrades.UpdateGrade(grade);
        }
        public CourseGrade? RemoveGrade(int id)
        {
            return _courseGrades.RemoveGrade(id);
        }
        public bool IsStudentGraded(int studentId, int courseId)
        {
            return _courseGrades.IsStudentGraded(studentId, courseId);
        }
        public CourseGrade? GetGradeById(int id)
        {
            return _courseGrades.GetGradeById(id);
        }
        public CourseGrade? GetCourseGradeByStudentTeacher(int studentId, int teacherId, int courseId)
        {
            return _courseGrades.GetCourseGradeByStudentTeacher(studentId, teacherId, courseId);
        }
        public CourseGrade? GetCourseGradeByStudent(int studentId, int courseId)
        {
            return _courseGrades.GetCourseGradeByStudent(studentId, courseId);
        }
        public List<CourseGrade> GetCourseGradesByTeacherCourse(int teacherId, int courseId)
        {
            return _courseGrades.GetCourseGradesByTeacherCourse(teacherId, courseId);
        }
        public List<CourseGrade> GetAllCourseGrades()
        {
            return _courseGrades.GetAllCourseGrades();
        }
    }
}
