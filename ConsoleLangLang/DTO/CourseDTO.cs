using LangLang.Controller;
using LangLang.Domain.Model;
using LangLang.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleLangLang.ConsoleApp.DTO
{
    public class CourseDTO
    {
        private int Id { get; set; }
        public Language Language { get; set; }
        public LanguageLevel LanguageLevel { get; set; }
        public string Duration { get; set; }
        public List<DayOfWeek> WorkDays { get; set; }
        public DateTime StartDate { get; set; }
        public string StartTime { get; set; }
        public bool IsOnline { get; set; }
        private int CurrentlyEnrolled { get; set; }
        public string MaxEnrolledStudents { get; set; }

        private readonly CourseController _courseController;
        private Teacher teacher;

        /*public CourseDTO(Teacher teacher)
        {
            _courseController = Injector.CreateInstance<CourseController>();
            this.teacher = teacher;
        }*/
        public CourseDTO()
        {
            _courseController = Injector.CreateInstance<CourseController>();
        }
        public void SetTeacher(Teacher teacher, Course course)
        {
            this.teacher = teacher;
            teacher.CoursesId.Add(course.Id);
        }
      /*  public List<string> LanguageAndLevelValues
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
        }*/

        private Regex _TimeRegex = new Regex(@"^(?:[01]\d|2[0-3]):(?:[0-5]\d)$");

        public string ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case "Duration":
                    if (string.IsNullOrEmpty(Duration))
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
                    if (string.IsNullOrEmpty(MaxEnrolledStudents))
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

        private string IsValidCourseTimeslot()
        {
            DateTime combinedDateTime = StartDate.Date + TimeSpan.Parse(StartTime);

            Course course = new Course
            {
                Language = this.Language,
                Level = this.LanguageLevel,
                Duration = int.Parse(this.Duration),
                WorkDays = this.WorkDays,
                StartDate = combinedDateTime,
                IsOnline = this.IsOnline,
                CurrentlyEnrolled = this.CurrentlyEnrolled,
                MaxEnrolledStudents = int.Parse(this.MaxEnrolledStudents)
            };
            if (this.teacher != null && !_courseController.ValidateCourseTimeslot(course, this.teacher))
                return "Cannot create course because of course time overlaps!";
            return null;
        }

        public bool IsValid()
        {
            string[] validatedProperties = { "Duration", "StartDate", "StartTime", "IsOnline", "CurrentlyEnrolled", "MaxEnrolledStudents", "WorkDays" };
            foreach (var property in validatedProperties)
            {
                if (ValidateProperty(property) != null)
                    return false;
            }
            if (!string.IsNullOrEmpty(IsValidCourseTimeslot()))
                return false;
            return true;
        }


        public Course ToModelClass()
        {
            if (Duration == null)
                return new Course();
            TimeSpan timeSpan = TimeSpan.Parse(StartTime);
            DateTime combinedDateTime = StartDate.Date + timeSpan;

            if (IsOnline)
                MaxEnrolledStudents = "0";

            return new Course
            {
                Id = this.Id,
                Language = this.Language,
                Level = this.LanguageLevel,
                Duration = int.Parse(this.Duration),
                WorkDays = this.WorkDays,
                StartDate = combinedDateTime,
                IsOnline = this.IsOnline,
                CurrentlyEnrolled = 0,
                MaxEnrolledStudents = int.Parse(this.MaxEnrolledStudents)
            };
        }

        public CourseDTO ToDTO(Course course)
        {
            string startTime = course.StartDate.ToString().Split(" ")[1];
            string startDate = course.StartDate.ToString().Split(" ")[0];
            DateTime date = DateTime.Parse(startDate);

            return new CourseDTO
            {
                Id = course.Id,
                Language = course.Language,
                LanguageLevel = course.Level,
                Duration = course.Duration.ToString(),

                WorkDays = course.WorkDays,
                StartDate = date,
                StartTime = startTime,
                IsOnline = course.IsOnline,
                CurrentlyEnrolled = course.CurrentlyEnrolled,
                MaxEnrolledStudents = course.MaxEnrolledStudents.ToString()
            };
        }

    }
}
