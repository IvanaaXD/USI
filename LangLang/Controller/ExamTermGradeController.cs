using LangLang.Domain.IRepository;
using LangLang.Domain.Model.Enums;
using LangLang.Domain.Model;
using LangLang.Observer;
using LangLang.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Controller
{
    public class ExamTermGradeController { 

        private readonly IExamTermGradeRepository _examTermGrades;

        public ExamTermGradeController(IExamTermGradeRepository _examTermGrades)
        {
            _examTermGrades = _examTermGrades ?? throw new ArgumentNullException(nameof(_examTermGrades));
        }

        public List<ExamTermGrade> GetAllExamTermGrades()
        {
            return _examTermGrades.GetAllExamTermGrades();
        }

        public List<ExamTermGrade> GetExamTermGradesByTeacherExam(int teacherId, int examTermId)
        {
            return _examTermGrades.GetExamTermGradesByTeacherExam(teacherId, examTermId);
        }
        public ExamTermGrade? GetExamTermGradeByStudentExam(int studentId, int examTermId)
        {
            return _examTermGrades.GetExamTermGradeByStudentExam(studentId, examTermId);
        }

        public List<ExamTermGrade> GetExamTermGradeByExam(int examTermId)
        {
            return _examTermGrades.GetExamTermGradeByExam(examTermId);
        }

        public ExamTermGrade? GetExamTermGradeByStudentTeacherExam(int studentId, int teacherId, int examTermId)
        {
            return _examTermGrades.GetExamTermGradeByStudentTeacherExam(studentId, teacherId, examTermId);
        }

        public ExamTermGrade GradeStudent(ExamTermGrade grade)
        {
            return _examTermGrades.AddGrade(grade);
        }

        public bool IsStudentGraded(int studentId, int examId)
        {
            return _examTermGrades.IsStudentGraded(studentId, examId);
        }

    }
}
