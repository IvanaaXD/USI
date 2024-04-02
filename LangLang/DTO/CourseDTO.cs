using LangLang.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LangLang.Model;
using System.Text.RegularExpressions;
using System.Configuration;

namespace LangLang.DTO
{
    public class CourseDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private int courseID;
        private Language language;
        private LanguageLevel languageLevel;
        private int duration;
        private List<DayOfWeek> workDays;
        private DateTime startDate;
        private string startTime;
        private bool isOnline;
        private int currentlyEnrolled;
        private int maxEnrolledStudents;
        private List<int> examTerms;

        public List<string> LanguageAndLevelValues
        {
            get
            {
                List<string> languageLevelNames = new List<string>();

                var languages = Enum.GetValues(typeof(Language)).Cast<Language>().ToList();
                var levels = Enum.GetValues(typeof(LanguageLevel)).Cast<LanguageLevel>().ToList();

                foreach (var language in languages)
                {
                    foreach (var level in levels)
                    {
                        languageLevelNames.Add($"{language} {level}");
                    }
                }
                return languageLevelNames;
            }
        }

        public List<DayOfWeek> DayOfWeekValues
        {
            get
            {
                var days = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();
                return days;
            }
        }

        public int CourseID
        {
            get { return courseID; }
            set { SetProperty(ref courseID, value); }
        }

        public Language Language
        {
            get { return language; }
            set { SetProperty(ref language, value); }
        }

        public LanguageLevel Level
        {
            get { return languageLevel; }
            set { SetProperty(ref languageLevel, value); }
        }

        public int Duration
        {
            get { return duration; }
            set { SetProperty(ref duration, value); }
        }

        public List<DayOfWeek> WorkDays
        {
            get { return workDays; }
            set { SetProperty(ref workDays, value); }
        }

        public DateTime StartDate
        {
            get { return startDate; }
            set { SetProperty(ref startDate, value); }
        }

        public string StartTime
        {
            get { return startTime; }
            set { SetProperty(ref startTime, value); }
        }

        public bool IsOnline
        {
            get { return isOnline; }
            set { SetProperty(ref isOnline, value); }
        }

        public int CurrentlyEnrolled
        {
            get { return currentlyEnrolled; }
            set { SetProperty(ref currentlyEnrolled, value); }
        }

        public int MaxEnrolledStudents
        {
            get { return maxEnrolledStudents; }
            set { SetProperty(ref maxEnrolledStudents, value); }
        }

        public List<int> ExamTerms
        {
            get { return examTerms; }
            set { SetProperty(ref examTerms, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public string Error => null;

        private Regex _TimeRegex = new Regex(@"^(?:[01]\d|2[0-3]):(?:[0-5]\d)$");

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Duration":
                        if (Duration <= 0 || Duration > 50)
                            return "Duration must be > 1 and < 50 weeks";
                        break;
                    case "StartDate":
                        if (StartDate < DateTime.Today)
                            return "Start date cannot be in the past";
                        break;
                    case "StartTime":
                        if (!_TimeRegex.IsMatch(StartTime))
                            return "Format is not good. Try again.";
                        break;
                    case "IsOnline":
                        if (IsOnline && MaxEnrolledStudents != 0)
                            return "Max enrolled students must be 0 for online courses";
                        if (!IsOnline && MaxEnrolledStudents > 150)
                            return "Max enrolled students must be <= 150 for offline courses";
                        break;
                    case "CurrentlyEnrolled":
                        if (CurrentlyEnrolled < 0 || (!IsOnline && CurrentlyEnrolled > MaxEnrolledStudents))
                            return "Number of enrolled students can't be less than 0 or greater than max enrolled";
                        break;
                    case "MaxEnrolledStudents":
                        if (IsOnline && MaxEnrolledStudents != 0)
                            return "Max enrolled students must be 0 for online courses";
                        if (!IsOnline && MaxEnrolledStudents > 150)
                            return "Max enrolled students must be <= 150 for offline courses";
                        break;
                    case "WorkDays":
                        if (WorkDays == null || !WorkDays.Any())
                            return "At least one work day must be chosen";
                        break;
                }
                return null;
            }
        }
        private readonly string[] _validatedProperties = { "Duration", "StartDate", "StartTime", "IsOnline", "CurrentlyEnrolled", "MaxEnrolledStudents", "WorkDays" };

        public bool IsValid
        {
            get
            {
                foreach (var property in _validatedProperties)
                {
                    if (this[property] != null)
                        return false;
                }
                return true;

            }
        }


        public Course ToCourse()
        {
            TimeSpan timeSpan = TimeSpan.Parse(startTime);

            DateTime combinedDateTime = startDate.Date + timeSpan;

            if (examTerms == null)
            {
                examTerms = new List<int>();
            }

            return new Course
            {
                Language = language,
                Level = languageLevel,
                Duration = duration,
                WorkDays = workDays,
                StartDate = combinedDateTime,
                IsOnline = isOnline,
                CurrentlyEnrolled = currentlyEnrolled,
                MaxEnrolledStudents = maxEnrolledStudents,
                ExamTerms = examTerms
            };
        }

        public CourseDTO()
        {
        }

        public CourseDTO(Course course)
        {
            courseID = course.CourseID;
            language = course.Language;
            languageLevel = course.Level;
            duration = course.Duration;

            workDays = course.WorkDays;
            startDate = course.StartDate;
            isOnline = course.IsOnline;
            currentlyEnrolled = course.CurrentlyEnrolled;
            maxEnrolledStudents = course.MaxEnrolledStudents;

            examTerms = course.ExamTerms;
        }

    }
}
