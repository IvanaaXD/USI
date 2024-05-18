using LangLang.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using LangLang.Observer;
using LangLang.Storage;

namespace LangLang.Repository
{
    public class StudentGradeDAO : Subject
    {
        private readonly List<StudentGrade> _StudentGrades;
        private readonly Storage<StudentGrade> _storage;

        public StudentGradeDAO()
        {
            _storage = new Storage<StudentGrade>("studentGrades.csv");
            _StudentGrades = _storage.Load();
        }

        private int GenerateId()
        {
            if (_StudentGrades.Count == 0) return 0;
            return _StudentGrades.Last().Id + 1;
        }

        public StudentGrade AddGrade(StudentGrade StudentGrade)
        {
            StudentGrade.Id = GenerateId();
            _StudentGrades.Add(StudentGrade);
            _storage.Save(_StudentGrades);
            NotifyObservers();
            return StudentGrade;
        }

        public StudentGrade? UpdateGrade(StudentGrade StudentGrade)
        {
            StudentGrade? oldStudentGrade = GetStudentGradeById(StudentGrade.Id);
            if (oldStudentGrade == null) return null;

            oldStudentGrade.StudentId = StudentGrade.StudentId;
            oldStudentGrade.TeacherId = StudentGrade.TeacherId;
            oldStudentGrade.CourseId = StudentGrade.CourseId;
            oldStudentGrade.Value = StudentGrade.Value;

            _storage.Save(_StudentGrades);
            NotifyObservers();
            return oldStudentGrade;
        }

        public StudentGrade? RemoveGrade(int id)
        {
            StudentGrade? StudentGrade = GetStudentGradeById(id);
            if (StudentGrade == null) return null;

            _StudentGrades.Remove(StudentGrade);
            _storage.Save(_StudentGrades);
            NotifyObservers();
            return StudentGrade;
        }
        public bool IsTeacherStudentGraded(int teacherId)
        {
            foreach (var StudentGrade in _StudentGrades)
            {
                if (StudentGrade.TeacherId == teacherId)
                {
                    return true;
                }
            }
            return false;
        }

        public StudentGrade? GetStudentGradeById(int id)
        {
            return _StudentGrades.Find(v => v.Id == id);
        }
        public StudentGrade? GetStudentGradeByStudentTeacher(int studentId, int teacherId, int courseId)
        {
            return _StudentGrades.Find(StudentGrade => StudentGrade.StudentId == studentId && StudentGrade.TeacherId == teacherId && StudentGrade.CourseId == courseId);
        }
        public StudentGrade? GetStudentGradeByStudent(int studentId, int courseId)
        {
            return _StudentGrades.Find(StudentGrade => StudentGrade.StudentId == studentId && StudentGrade.CourseId == courseId);
        }
        public List<StudentGrade> GetStudentGradesByTeacherCourse(int teacherId, int courseId)
        {
            return _StudentGrades.Where(StudentGrade => StudentGrade.TeacherId == teacherId && StudentGrade.CourseId == courseId).ToList();
        }
        public List<StudentGrade> GetStudentGradeByTeacher(int teacherId)
        {
            return _StudentGrades.Where(StudentGrade => StudentGrade.TeacherId == teacherId).ToList();
        }

        public List<StudentGrade> GetAllStudentGrades()
        {
            return _StudentGrades;
        }
    }
}
