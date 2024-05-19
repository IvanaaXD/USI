using LangLang.Domain.Model;
using LangLang.Observer;
using System.Collections.Generic;

namespace LangLang.Domain.IRepository
{
    public interface IExamTermGradeRepository : IObserver
    {
        ExamTermGrade AddGrade(ExamTermGrade grade);
        ExamTermGrade? UpdateGrade(ExamTermGrade grade);
        ExamTermGrade? RemoveGrade(int id);
        bool IsStudentGraded(int studentId, int examId);
        ExamTermGrade? GetGradeById(int id);
        ExamTermGrade? GetExamTermGradeByStudentTeacherExam(int studentId, int teacherId, int examId);
        ExamTermGrade? GetExamTermGradeByStudentExam(int studentId, int examId);
        List<ExamTermGrade> GetExamTermGradesByTeacherExam(int teacherId, int examId);
        List<ExamTermGrade> GetExamTermGradeByExam(int examTermId);
        List<ExamTermGrade> GetAllExamTermGrades();
        void Subscribe(IObserver observer);

    }

}
