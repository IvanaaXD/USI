using LangLang.Repository;
using LangLang.Domain.Model.Enums;
using LangLang.Domain.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using LangLang.Domain.IRepository;
using System.IO;

namespace LangLang.Controller
{
    public class ExamTermController
    {
        private readonly IExamTermRepository _exams;
        private readonly TeacherController teacherController;

        public ExamTermController(IExamTermRepository exams, TeacherController teacherController)
        {
            _exams = exams ?? throw new ArgumentNullException(nameof(exams));
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
       
        public bool ValidateExamTimeslot(ExamTerm exam, Teacher teacher)
        {
            bool isOverlap = CheckExamOverlap(exam, teacher);
            if (!isOverlap)
                return isOverlap;
            return true;
        }
        private bool CheckExamOverlap(ExamTerm exam, Teacher teacher)
        {
            bool isSameTeacherCourseOverlap = CheckTeacherExamOverlapsCourses(exam, teacher);
            if (isSameTeacherCourseOverlap)
                return false;

            bool isSameTeacherExamOverlap = CheckTeacherExamsOverlap(exam, teacher);
            if (isSameTeacherExamOverlap)
                return false;

            return true;
        }
       
        public void Subscribe(IObserver observer)
        {
            _exams.Subscribe(observer);
        }
        
        public bool CheckTeacherExamOverlapsCourses(ExamTerm examTerm, Teacher teacher)
        {
            int courseDurationInMinutes = 90;
            int examDurationInMinutes = 240;

            System.DateTime examStartTime = examTerm.ExamTime;
            DateTime examEndTime = examStartTime.AddMinutes(examDurationInMinutes);

            List<Course> teacherCourses = teacherController.GetAvailableCourses(teacher);
            foreach (Course course in teacherCourses)
            {
                if (!course.WorkDays.Contains(examTerm.ExamTime.DayOfWeek))
                    continue;

                DateTime courseStartTime = course.StartDate;
                DateTime courseEndTime = courseStartTime.AddMinutes(courseDurationInMinutes);

                DateTime maxStartTime = courseStartTime > examStartTime ? courseStartTime : examStartTime;
                DateTime minEndTime = courseEndTime < examEndTime ? courseEndTime : examEndTime;

                if ((courseStartTime == examStartTime || courseEndTime == examEndTime) ||
                    (maxStartTime < minEndTime))
                    return true;
            }
            return false;
        }
        public ExamTerm ConfirmExamTerm(int examTermId)
        {
            ExamTerm examTerm = GetExamTermById(examTermId);
            examTerm.Confirmed = true;
            _exams.UpdateExamTerm(examTerm);
            return examTerm;
        }

        public List<ExamTerm> FindExamTermsByCriteria(Language? language, LanguageLevel? level, DateTime? examDate)
        {
            List<ExamTerm> allExams = GetAllExamTerms();
            var filteredExams = new List<ExamTerm>();

            foreach (var exam in allExams)
            {
                Course course = teacherController.GetCourseById(exam.CourseID);

                bool matchesLanguage = !language.HasValue || course.Language == language;
                bool matchesLevel = !level.HasValue || course.Level == level;
                bool matchesExamDate = !examDate.HasValue || exam.ExamTime.Date >= examDate.Value.Date;

                if (matchesLanguage && matchesLevel && matchesExamDate)
                    filteredExams.Add(exam);
            }
            return filteredExams;
        }

        public bool CheckTeacherExamsOverlap(ExamTerm examTerm, Teacher teacher)
        {
            int examDurationInMinutes = 240;

            System.DateTime examStartTime = examTerm.ExamTime;
            DateTime examEndTime = examStartTime.AddMinutes(examDurationInMinutes);

            List<ExamTerm> teacherExams = teacherController.GetAvailableExamTerms(teacher);
            foreach (ExamTerm secondExam in teacherExams)
            {
                if (examTerm.ExamID == secondExam.ExamID)
                    continue;

                DateTime secondExamStartTime = secondExam.ExamTime;
                DateTime secondExamEndTime = secondExamStartTime.AddMinutes(examDurationInMinutes);

                DateTime maxStartTime = examStartTime > secondExamStartTime ? examStartTime : secondExamStartTime;
                DateTime minEndTime = examEndTime < secondExamEndTime ? examEndTime : secondExamEndTime;

                if ((examStartTime == secondExamStartTime && examEndTime == secondExamEndTime) ||
                    (maxStartTime < minEndTime))
                    return true;
            }
            return false;
        }

    }
}


