using System;
using System.Linq;
using LangLang.Controller;
using LangLang.Domain.Model.Enums;
using LangLang.Observer;
using LangLang.Storage;
using System.Collections.Generic;
using LangLang.Domain.Model;

namespace LangLang.Repository
{
    public class ExamTermDAO : Subject
    {
        private readonly List<ExamTerm> _examTerms;
        private readonly Storage<ExamTerm> _examTermsStorage;
        private readonly TeacherController teacherController;

        public ExamTermDAO()
        {
            _examTermsStorage = new Storage<ExamTerm>("exam.csv");
            _examTerms = _examTermsStorage.Load();
        }
        private int GenerateExamId()
        {
            if (_examTerms.Count == 0) return 0;
            return _examTerms.Last().ExamID + 1;
        }
        public ExamTerm AddExamTerm(ExamTerm examTerm)
        {
            examTerm.ExamID = GenerateExamId();
            _examTerms.Add(examTerm);
            _examTermsStorage.Save(_examTerms);
            NotifyObservers();
            return examTerm;
        }
        public ExamTerm? UpdateExamTerm(ExamTerm examTerm)
        {
            ExamTerm? oldExamTerm = GetExamTermById(examTerm.ExamID);
            if (oldExamTerm == null) return null;

            oldExamTerm.CourseID = examTerm.CourseID;
            oldExamTerm.ExamTime = examTerm.ExamTime;
            oldExamTerm.MaxStudents = examTerm.MaxStudents;

            _examTermsStorage.Save(_examTerms);
            NotifyObservers();
            return oldExamTerm;
        }
        public ExamTerm GetExamTermById(int id)
        {
            return _examTerms.Find(et => et.ExamID == id);
        }
        public List<ExamTerm> GetAllExamTerms()
        {
            return _examTerms;
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

        public string FindLanguageAndLevel(int courseID)
        {
            string res = "";
            Course course = teacherController.GetAllCourses().FirstOrDefault(c => c.Id == courseID);

            if (course != null)
                res = $"{course.Language}, {course.Level}";
            else
                res = "Language and level not found";
            return res;
        }
        public void DecrementExamTermCurrentlyAttending(int examTermId)
        {
            ExamTerm examTerm = GetExamTermById(examTermId);
            --examTerm.CurrentlyAttending;
            UpdateExamTerm(examTerm);
        }
        public ExamTerm ConfirmExamTerm(int examTermId)
        {
            ExamTerm examTerm = GetExamTermById(examTermId);
            examTerm.Confirmed = true;
            _examTermsStorage.Save(_examTerms);
            NotifyObservers();
            return examTerm;
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
