using LangLang.Repository;
using LangLang.Domain.Model.Enums;
using LangLang.Domain.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using LangLang.Domain.IRepository;

namespace LangLang.Controller
{
    public class ExamTermController
    {
        private readonly ExamTermDAO _exams;
        private readonly IExamTermGradeRepository _examTermGrades;
        private readonly TeacherController teacherController;

        public ExamTermController(TeacherController teacherController, IExamTermGradeRepository examTermGrades)
        {
            _exams = new ExamTermDAO(teacherController);
            _examTermGrades = examTermGrades ?? throw new ArgumentNullException(nameof(examTermGrades));
            this.teacherController = teacherController;
        }
        public ExamTerm? GetExamTermById(int examId)
        {
            return _exams.GetExamTermById(examId);
        }
        public List<ExamTerm> GetAllExamTerms()
        {
            return _exams.GetAllExamTerms();
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
        public void AddExamTerm(ExamTerm examTerm)
        {
            _exams.AddExamTerm(examTerm);
        }
        public void UpdateExamTerm(ExamTerm examTerm)
        {
            _exams.UpdateExamTerm(examTerm);
        }
        public ExamTermGrade GradeStudent(ExamTermGrade grade)
        {
            return _examTermGrades.AddGrade(grade);
        }
        public ExamTerm ConfirmExamTerm(int examTermId)
        {
            return _exams.ConfirmExamTerm(examTermId);
        }
        
        public bool IsStudentGraded(int studentId, int examId)
        {
            return _examTermGrades.IsStudentGraded(studentId, examId);
        }
        public bool ValidateExamTimeslot(ExamTerm exam, Teacher teacher)
        {
            bool isOverlap = CheckExamOverlap(exam, teacher);
            if (!isOverlap)
                return isOverlap;
            return true;
        }
        private bool CheckExamOverlap(ExamTerm exam, Teacher teacher)
        {
            bool isSameTeacherCourseOverlap = _exams.CheckTeacherExamOverlapsCourses(exam, teacher);
            if (isSameTeacherCourseOverlap)
                return false;

            bool isSameTeacherExamOverlap = _exams.CheckTeacherExamsOverlap(exam, teacher);
            if (isSameTeacherExamOverlap)
                return false;

            return true;
        }
       
        public void Subscribe(IObserver observer)
        {
            _exams.Subscribe(observer);
        }
        public List<ExamTerm> FindExamTermsByCriteria(Language? language, LanguageLevel? level, DateTime? examDate)
        {
            return _exams.FindExamTermsByCriteria(language, level, examDate);
        }
      
    }
}


