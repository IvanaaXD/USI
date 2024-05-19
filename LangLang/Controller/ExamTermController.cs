using LangLang.Repository;
using LangLang.Domain.Model.Enums;
using LangLang.Domain.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using LangLang.Domain.IRepository;
using System.Linq;

namespace LangLang.Controller
{
    public class ExamTermController
    {
        private readonly IExamTermRepository _exams;
        private readonly TeacherController teacherController;

        public ExamTermController(IExamTermRepository _exams, TeacherController teacherController)
        {
            _exams = _exams ?? throw new ArgumentNullException(nameof(_exams));
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

        public void AddExamTerm(ExamTerm examTerm)
        {
            _exams.AddExamTerm(examTerm);
        }

        public void UpdateExamTerm(ExamTerm examTerm)
        {
            _exams.UpdateExamTerm(examTerm);
        }

        public ExamTerm ConfirmExamTerm(int examTermId)
        {
            return _exams.ConfirmExamTerm(examTermId);
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

        public List<ExamTerm> FindExamTermsByDate(DateTime? startDate)
        {
            var filteredCourses = _exams.GetAllExamTerms().Where(exam =>
                (exam.ExamTime.Date >= (startDate.Value.Date) && exam.ExamTime.Date <= DateTime.Today.Date)
            ).ToList();

            return filteredCourses;
        }

    }
}


