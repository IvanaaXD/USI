using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.Storage.Serialization;
using LangLang.Domain.Model.Enums;

namespace LangLang.Domain.Model
{
    public class Course : ISerializable
    {
        private int id;
        private Language language;
        private LanguageLevel languageLevel;
        private int duration;
        private List<DayOfWeek> workDays;
        private DateTime startDate;
        private bool isOnline;
        private int currentlyEnrolled;
        private int maxEnrolledStudents;
        public int Id
        {
            get { return id; }
            set { id = value; }
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
        public Course()
        {
        }
        public Course(int id, Language language, LanguageLevel languageLevel, int duration, List<DayOfWeek> workDays, DateTime startDate, bool isOnline, int currentlyEnrolled, int maxEnrolledStudents)
        {
            this.id = id;
            this.language = language;
            this.languageLevel = languageLevel;
            this.duration = duration;
            this.workDays = workDays;
            this.startDate = startDate;
            this.isOnline = isOnline;
            this.currentlyEnrolled = currentlyEnrolled;
            this.maxEnrolledStudents = maxEnrolledStudents;
        }

        public string[] ToCSV()
        {
            string workDaysStr = string.Join(",", workDays.Select(d => d.ToString()));

            string[] csvValues =
            {
                Id.ToString(),
                Language.ToString(),
                Level.ToString(),
                Duration.ToString(),
                workDaysStr,
                StartDate.ToString("yyyy-MM-dd HH:mm"),
                IsOnline.ToString(),
                CurrentlyEnrolled.ToString(),
                MaxEnrolledStudents.ToString()
            };

            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            Language = (Language)Enum.Parse(typeof(Language), values[1]);
            Level = (LanguageLevel)Enum.Parse(typeof(LanguageLevel), values[2]);
            Duration = int.Parse(values[3]);
            WorkDays = values[4].Split(',').Select(d => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), d)).ToList();
            StartDate = DateTime.ParseExact(values[5], "yyyy-MM-dd HH:mm", null);
            IsOnline = bool.Parse(values[6]);
            CurrentlyEnrolled = int.Parse(values[7]);
            MaxEnrolledStudents = int.Parse(values[8]);
        }
        public override string ToString()
        {
            return $"{language} {languageLevel}";
        }
    }
}
