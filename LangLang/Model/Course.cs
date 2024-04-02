using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LangLang.Storage.Serialization;
using LangLang.Model.Enums;

namespace LangLang.Model
{
    public class Course : ISerializable
    {
        private int courseID;
        private Language language;
        private LanguageLevel languageLevel;
        private int duration;
        private List<DayOfWeek> workDays;
        private DateTime startDate;
        private bool isOnline;
        private int currentlyEnrolled;
        private int maxEnrolledStudents;
        private List<int> examTerms;

        public int CourseID
        {
            get { return courseID; }
            set { courseID = value; }
        }

        public Language Language
        {
            get { return language; }
            set { language = value; }
        }

        public LanguageLevel Level
        {
            get { return languageLevel; }
            set { languageLevel = value; }
        }

        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }
        public List<DayOfWeek> WorkDays
        {
            get { return workDays; }
            set { workDays = value; }
        }

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        public bool IsOnline
        {
            get { return isOnline; }
            set { isOnline = value; }
        }
        public int CurrentlyEnrolled
        {
            get { return currentlyEnrolled; }
            set { currentlyEnrolled = value; }
        }

        public int MaxEnrolledStudents
        {
            get { return maxEnrolledStudents; }
            set { maxEnrolledStudents = value; }
        }

        public List<int> ExamTerms
        {
            get { return examTerms; }
            set { examTerms = value; }
        }

        public Course()
        {
        }
        public Course(int courseID, Language language, LanguageLevel languageLevel, int duration, List<DayOfWeek> workDays, DateTime startDate, bool isOnline, int currentlyEnrolled, int maxEnrolledStudents, List<int> examTerms)
        {
            this.courseID = courseID;
            this.language = language;
            this.languageLevel = languageLevel;
            this.duration = duration;
            this.workDays = workDays;
            this.startDate = startDate;
            this.isOnline = isOnline;
            this.currentlyEnrolled = currentlyEnrolled;
            this.maxEnrolledStudents = maxEnrolledStudents;
            this.examTerms = examTerms;
        }
        public Course(Language language, LanguageLevel languageLevel, int duration, List<DayOfWeek> workDays, DateTime startDate, bool isOnline, int currentlyEnrolled, int maxEnrolledStudents, List<int> examTerms)
        {
            this.language = language;
            this.languageLevel = languageLevel;
            this.duration = duration;
            this.workDays = workDays;
            this.startDate = startDate;
            this.isOnline = isOnline;
            this.currentlyEnrolled = currentlyEnrolled;
            this.maxEnrolledStudents = maxEnrolledStudents;
            this.examTerms = examTerms;
        }

        public string[] ToCSV()
        {
            string workDaysStr = string.Join(",", workDays.Select(d => d.ToString()));

            string examTermsStr = string.Join(",", examTerms);

            string[] csvValues =
            {
                CourseID.ToString(),
                Language.ToString(),
                Level.ToString(),
                Duration.ToString(),
                workDaysStr,
                StartDate.ToString("yyyy-MM-dd"),
                IsOnline.ToString(),
                CurrentlyEnrolled.ToString(),
                MaxEnrolledStudents.ToString(),
                examTermsStr
            };

            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            CourseID = int.Parse(values[0]);
            Language = (Language)Enum.Parse(typeof(Language), values[1]);
            Level = (LanguageLevel)Enum.Parse(typeof(LanguageLevel), values[2]);
            Duration = int.Parse(values[3]);
            WorkDays = values[4].Split(',').Select(d => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), d)).ToList();
            StartDate = DateTime.ParseExact(values[5], "yyyy-MM-dd", null);
            IsOnline = bool.Parse(values[6]);
            CurrentlyEnrolled = int.Parse(values[7]);
            MaxEnrolledStudents = int.Parse(values[8]);
            ExamTerms = values[9].Split(',').Select(int.Parse).ToList();
        }

    }
}
