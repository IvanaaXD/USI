using LangLang.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using LangLang.Controller;

namespace LangLang.DTO
{
    public class CourseDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private int id;
        private Language language;
        private LanguageLevel languageLevel;
        private string duration;
        private List<DayOfWeek> workDays;
        private DateTime startDate;
        private string startTime;
        private bool isOnline;
        private int currentlyEnrolled;
        private string maxEnrolledStudents;
        private List<int> examTerms;
        private bool hasTeacher;

        private readonly CourseController _courseController;
        private Domain.Model.Teacher teacher;

        public CourseDTO(CourseController courseController, Domain.Model.Teacher teacher)
        {
            _courseController = courseController;
            this.teacher = teacher;
        }
        public CourseDTO(CourseController courseController)
        {
            _courseController = courseController;
        }
        public void SetTeacher(Domain.Model.Teacher teacher, Domain.Model.Course course)
        {
            this.teacher = teacher;
            hasTeacher = true;
            teacher.CoursesId.Add(course.Id);
        }
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

        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
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

        public string Duration
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

        public string MaxEnrolledStudents
        {
            get { return maxEnrolledStudents; }
            set { SetProperty(ref maxEnrolledStudents, value); }
        }
        public bool HasTeacher
        {
            get { return hasTeacher; }
            set { SetProperty(ref hasTeacher, value); }
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
                        if (Duration == null)
                            return "Course duration must be >= 0";
                        if (Duration == "")
                            return "Course duration must be >= 0";
                        int durationValue;
                        if (!int.TryParse(Duration, out durationValue))
                            return "Invalid input for duration. Please enter a valid numeric value.";

                        if (durationValue < 0)
                            return "Course duration must be >= 0";
                        if (durationValue > 20)
                            return "Course duration must be <= 20";

                        break;
                    case "StartDate":
                        if (StartDate < DateTime.Today)
                            return "Start date cannot be in the past";
                        break;
                    case "StartTime":
                        if (!_TimeRegex.IsMatch(StartTime))
                            return "Format is not good. Try again.";
                        break;
                    case "CurrentlyEnrolled":
                        if (CurrentlyEnrolled < 0 || (!IsOnline && CurrentlyEnrolled > int.Parse(MaxEnrolledStudents)))
                            return "Number of enrolled students can't be less than 0 or greater than max enrolled";
                        break;
                    case "MaxEnrolledStudents":
                        if (MaxEnrolledStudents == null || MaxEnrolledStudents == "")
                            return "Value must be >=0";
                        if (int.Parse(MaxEnrolledStudents) < 0)
                            return "Value must be >= 0";
                        if (int.Parse(MaxEnrolledStudents) > 150)
                            return "Value must be <= 150";
                        if (int.Parse(MaxEnrolledStudents) == 0 && !IsOnline)
                            return "Offline courses can't have 0 students";
                        break;
                    case "WorkDays":
                        if (WorkDays == null || !WorkDays.Any())
                            return "At least one work day must be chosen";
                        break;
                }

                return null;
            }
        }

        private string IsValidCourseTimeslot()
        {
            DateTime combinedDateTime = StartDate.Date + TimeSpan.Parse(StartTime);

            Domain.Model.Course course = new Domain.Model.Course
            {
                Id = id,
                Language = language,
                Level = languageLevel,
                Duration = int.Parse(duration),
                WorkDays = workDays,
                StartDate = combinedDateTime,
                IsOnline = isOnline,
                CurrentlyEnrolled = currentlyEnrolled,
                MaxEnrolledStudents = int.Parse(maxEnrolledStudents),
                ExamTerms = examTerms
            };
            if (this.teacher == null)
                return "Cannot create course!";
            if (!_courseController.ValidateCourseTimeslot(course, this.teacher))
                return "Cannot create course because of course time overlaps!";
            return null;
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
                if (!string.IsNullOrEmpty(IsValidCourseTimeslot()))
                    return false;
                return true;

            }
        }


        public Domain.Model.Course ToCourse()
        {
            string startTimes = startDate.ToString().Split(" ")[1];
            TimeSpan timeSpan = TimeSpan.Parse(startTimes);

            DateTime combinedDateTime = startDate.Date + timeSpan;
            /*TimeSpan timeSpan = TimeSpan.Parse(startTime);
            DateTime combinedDateTime = startDate.Date + timeSpan;*/

            if (examTerms == null)
            {
                examTerms = new List<int>();
            }
            if (isOnline)
            {
                maxEnrolledStudents = "0";
            }

            return new Domain.Model.Course
            {
                Id = id,
                Language = language,
                Level = languageLevel,
                Duration = int.Parse(duration),
                WorkDays = workDays,
                StartDate = combinedDateTime,
                IsOnline = isOnline,
                CurrentlyEnrolled = currentlyEnrolled,
                MaxEnrolledStudents = int.Parse(maxEnrolledStudents),
                ExamTerms = examTerms
            };
        }

        public CourseDTO()
        {
        }

        public CourseDTO(Domain.Model.Course course)
        {
            id = course.Id;
            language = course.Language;
            languageLevel = course.Level;
            duration = course.Duration.ToString();

            workDays = course.WorkDays;
            startDate = course.StartDate;
            isOnline = course.IsOnline;
            currentlyEnrolled = course.CurrentlyEnrolled;
            maxEnrolledStudents = course.MaxEnrolledStudents.ToString();
            examTerms = course.ExamTerms;
        }

        public CourseDTO(CourseController courseController, Domain.Model.Course course, Domain.Model.Teacher teacher)
        {
            _courseController = courseController;
            this.teacher = teacher;
            id = course.Id;
            language = course.Language;
            languageLevel = course.Level;
            duration = course.Duration.ToString();

            workDays = course.WorkDays;
            startDate = course.StartDate;
            isOnline = course.IsOnline;
            currentlyEnrolled = course.CurrentlyEnrolled;
            maxEnrolledStudents = course.MaxEnrolledStudents.ToString();

            examTerms = course.ExamTerms;
        }

    }
}
