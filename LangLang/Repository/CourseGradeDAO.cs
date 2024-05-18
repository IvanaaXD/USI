using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using LangLang.Observer;
using LangLang.Storage;
using LangLang.Model;

namespace LangLang.Model.DAO
{
    public class CourseGradeDAO : Subject
    {
        private readonly List<CourseGrade> _grades;
        private readonly Storage<CourseGrade> _storage;

        public CourseGradeDAO()
        {
            _storage = new Storage<CourseGrade>("courseGrades.csv");
            _grades = _storage.Load();
        }

        private int GenerateId()
        {
            if (_grades.Count == 0) return 0;
            return _grades.Last().Id + 1;
        }

        public CourseGrade AddGrade(CourseGrade grade)
        {
            grade.Id = GenerateId();
            _grades.Add(grade);
            _storage.Save(_grades);
            NotifyObservers();
            return grade;
        }

        public CourseGrade? UpdateGrade(CourseGrade grade)
        {
            CourseGrade? oldGrade = GetGradeById(grade.Id);
            if (oldGrade == null) return null;

            oldGrade.StudentId = grade.StudentId;
            oldGrade.TeacherId = grade.TeacherId;
            oldGrade.CourseId = grade.CourseId;
            oldGrade.StudentActivityValue = grade.StudentActivityValue;
            oldGrade.StudentKnowledgeValue = grade.StudentKnowledgeValue;

            _storage.Save(_grades);
            NotifyObservers();
            return oldGrade;
        }

        public CourseGrade? RemoveGrade(int id)
        {
            CourseGrade? grade = GetGradeById(id);
            if (grade == null) return null;

            _grades.Remove(grade);
            _storage.Save(_grades);
            NotifyObservers();
            return grade;
        }
        public bool IsStudentGraded(int studentId, int courseId)
        {
            foreach (var grade in _grades)
            {
                if (grade.StudentId == studentId && grade.CourseId == courseId)
                {
                    return true;
                }
            }
            return false;
        }

        public CourseGrade? GetGradeById(int id)
        {
            return _grades.Find(v => v.Id == id);
        }
        public CourseGrade? GetCourseGradeByStudentTeacher(int studentId, int teacherId, int courseId)
        {
            return _grades.Find(grade => grade.StudentId == studentId && grade.TeacherId == teacherId && grade.CourseId == courseId);
        }
        public CourseGrade? GetCourseGradeByStudent(int studentId, int courseId)
        {
            return _grades.Find(grade => grade.StudentId == studentId && grade.CourseId == courseId);
        }
        public List<CourseGrade> GetCourseGradesByTeacherCourse(int teacherId, int courseId)
        {
            return _grades.Where(grade => grade.TeacherId == teacherId && grade.CourseId == courseId).ToList();
        }

        public List<CourseGrade> GetAllCourseGrades()
        {
            return _grades;
        }
    }
}
