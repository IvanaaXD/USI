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
        //private readonly IExamTermRepository _exams;
        private readonly IExamTermDbRepository _exams;
        private readonly TeacherController teacherController;
        private readonly IDirectorDbRepository _directorRepository;

        public ExamTermController(IExamTermDbRepository exams, TeacherController teacherController)
        {
            _exams = exams ?? throw new ArgumentNullException(nameof(exams));
            this.teacherController = teacherController;
        }
        public ExamTermController()
        {
            _exams = Injector.CreateInstance<IExamTermDbRepository>();
            this.teacherController = Injector.CreateInstance<TeacherController>();
            _directorRepository = Injector.CreateInstance<IDirectorDbRepository>();   
        }

        public ExamTerm? GetById(int examId)
        {
            return _exams.GetById(examId);
        }
        public List<ExamTerm> GetAllExamTerms()
        {
            return _exams.GetAll();
        }
        public List<ExamTerm> GetAllExamTerms(int page, int pageSize, string sortCriteria, List<ExamTerm> exams)
        {
            return _exams.GetAllExamTerms(page, pageSize, sortCriteria, exams);
        }
        public void Add(ExamTerm examTerm)
        {
            _exams.Add(examTerm);
        }
        public void Update(ExamTerm examTerm)
        {
            _exams.Update(examTerm);
        }

        public void Delete(ExamTerm examTerm)
        {
            teacherController.RemoveExamTerm(examTerm.ExamID);
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
            List<Course> teacherCourses = teacherController.GetAvailableCourses(teacher);
            foreach (Course course in teacherCourses)
            {
                if (!course.WorkDays.Contains(examTerm.ExamTime.DayOfWeek))
                    continue;

                if (CompareCourseExamTime(course,examTerm))
                    return true;
            }
            return false;
        }
        public bool CompareCourseExamTime(Course course, ExamTerm examTerm)
        {
            System.DateTime examStartTime = examTerm.ExamTime;
            DateTime examEndTime = examStartTime.AddMinutes(240); // examDurationInMinutes = 240

            DateTime courseStartTime = course.StartDate;
            DateTime courseEndTime = courseStartTime.AddMinutes(90); // courseDurationInMinutes = 90

            DateTime maxStartTime = courseStartTime > examStartTime ? courseStartTime : examStartTime;
            DateTime minEndTime = courseEndTime < examEndTime ? courseEndTime : examEndTime;

            if ((courseStartTime == examStartTime || courseEndTime == examEndTime) ||
                (maxStartTime < minEndTime))
                return true;
        
           return false;
        }
        public ExamTerm ConfirmExamTerm(int examTermId)
        {
            ExamTerm examTerm = GetById(examTermId);
            examTerm.Confirmed = true;
            _exams.Update(examTerm);
            return examTerm;
        }

        public List<ExamTerm> FindExamTermsByCriteria(Language? language, LanguageLevel? level, DateTime? examDate)
        {
            List<ExamTerm> allExams = GetAllExamTerms();
            var filteredExams = new List<ExamTerm>();

            foreach (var exam in allExams)
            {
                bool matchesLanguage = !language.HasValue || exam.Language == language;
                bool matchesLevel = !level.HasValue || exam.Level == level;
                bool matchesExamDate = !examDate.HasValue || exam.ExamTime.Date >= examDate.Value.Date;

                if (matchesLanguage && matchesLevel && matchesExamDate)
                    filteredExams.Add(exam);
            }
            return filteredExams;
        }

        public bool CheckTeacherExamsOverlap(ExamTerm examTerm, Teacher teacher)
        {

            List<ExamTerm> teacherExams = teacherController.GetAvailableExamTerms(teacher);
            foreach (ExamTerm secondExam in teacherExams)
            {
                if (examTerm.ExamID == secondExam.ExamID)
                    continue;

                
                if (CompareExamsTime(examTerm, secondExam))
                    return true;
            }
            return false;
        }
        public bool CompareExamsTime(ExamTerm examTerm, ExamTerm secondExamTerm)
        {
            System.DateTime examStartTime = examTerm.ExamTime;
            DateTime examEndTime = examStartTime.AddMinutes(240);  // examDurationInMinutes = 240;
            DateTime secondExamStartTime = secondExamTerm.ExamTime;
            DateTime secondExamEndTime = secondExamStartTime.AddMinutes(240);

            DateTime maxStartTime = examStartTime > secondExamStartTime ? examStartTime : secondExamStartTime;
            DateTime minEndTime = examEndTime < secondExamEndTime ? examEndTime : secondExamEndTime;

            if ((examStartTime == secondExamStartTime && examEndTime == secondExamEndTime) ||
                (maxStartTime < minEndTime))
                return true;
            return false;
        }
        public void DecrementExamTermCurrentlyAttending(int examTermId)
        {
            ExamTerm examTerm = GetById(examTermId);
            --examTerm.CurrentlyAttending;
            Update(examTerm);
        }

        public List<ExamTerm> FindExamTermsByDate(DateTime? startDate)
        {
            var filteredCourses = _exams.GetAll()
                .Where(course => course.ExamTime.Date >= startDate.Value.Date && course.ExamTime.Date <= DateTime.Today.Date)
                .ToList();

            return filteredCourses;
        }
        public Teacher DeleteExamTermsByTeacher(Teacher teacher)
        {
            var examTerms = GetAllExamTerms();
            var teacherExamTerms = teacher.ExamsId;
            if (teacherExamTerms == null)
                return teacher;


            foreach (var examTerm in examTerms)
            {
                Director director = _directorRepository.GetDirector();
                if (!director.ExamsId.Contains(examTerm.ExamID) && examTerm.ExamTime.Date > DateTime.Today.Date)
                    teacherExamTerms.Remove(examTerm.ExamID);
            }

            teacher.ExamsId = teacherExamTerms;
            return teacher;
        }
        public List<ExamTerm>? GetExamsForDisplay(bool isSearchClicked, List<ExamTerm> availableExams, Language? selectedLanguage, LanguageLevel? selectedLevel, DateTime? selectedStartDate)
        {
            List<ExamTerm> finalExams = new();
            if (!isSearchClicked)
            {
                return availableExams;
            }

            List<ExamTerm> allFilteredExams = FindExamTermsByCriteria(selectedLanguage, selectedLevel, selectedStartDate);
            foreach (ExamTerm examTerm in allFilteredExams)
            {
                foreach (ExamTerm teacherExam in availableExams)
                {
                    if (teacherExam.ExamID == examTerm.ExamID)
                        finalExams.Add(examTerm);
                }
            }

            return finalExams;
        }
    }
}


