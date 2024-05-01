using System.Collections.Generic;
using System.Linq;
using LangLang.Observer;
using LangLang.Storage;

namespace LangLang.Model.DAO
{
    public class GradeDAO : Subject
    {
        private readonly List<Grade> _grades;
        private readonly Storage<Grade> _storage;

        public GradeDAO()
        {
            _storage = new Storage<Grade>("grades.csv");
            _grades = _storage.Load();
        }

        private int GenerateId()
        {
            if (_grades.Count == 0) return 0;
            return _grades.Last().Id + 1;
        }

        public Grade AddGrade(Grade grade)
        {
            grade.Id = GenerateId();
            _grades.Add(grade);
            _storage.Save(_grades);
            NotifyObservers();
            return grade;
        }

        public Grade? UpdateGrade(Grade grade)
        {
            Grade? oldGrade = GetGradeById(grade.Id);
            if (oldGrade == null) return null;

            oldGrade.StudentId = grade.StudentId;
            oldGrade.TeacherId = grade.TeacherId;
            oldGrade.ExamId = grade.ExamId;
            oldGrade.ReadingPoints = grade.ReadingPoints;
            oldGrade.SpeakingPoints = grade.SpeakingPoints;
            oldGrade.WritingPoints = grade.WritingPoints;
            oldGrade.ListeningPoints = grade.ListeningPoints;
            oldGrade.Value = grade.Value;

            _storage.Save(_grades);
            NotifyObservers();
            return oldGrade;
        }

        public Grade? RemoveGrade(int id)
        {
            Grade? grade = GetGradeById(id);
            if (grade == null) return null;

            _grades.Remove(grade);
            _storage.Save(_grades);
            NotifyObservers();
            return grade;
        }
        public Grade? GetGradeById(int id)
        {
            return _grades.Find(v => v.Id == id);
        }
        public Grade? GetGradeByStudentTeacherExam(int studentId, int teacherId, int examId)
        {
            return _grades.Find(grade => grade.StudentId == studentId && grade.TeacherId == teacherId && grade.ExamId == examId);
        }
        public Grade? GetGradeByStudentExam(int studentId, int examId)
        {
            return _grades.Find(grade => grade.StudentId == studentId && grade.ExamId == examId);
        }
        public List<Grade> GetGradesByTeacherExam(int teacherId, int examId)
        {
            return _grades.Where(grade => grade.TeacherId == teacherId && grade.ExamId == examId).ToList();
        }

        public List<Grade> GetAllGrades()
        {
            return _grades;
        }

    }
}
