using LangLang.Controller;
using LangLang.Domain.Model;
using LangLang.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ConsoleLangLang.DTO
{
    public class ExamTermDTO : INotifyPropertyChanged
    {
        private int examID;
        private DateTime examDate;
        private string examTime;
        private int maxStudents;
        private int currentlyAttending;
        private bool confirmed;
        private bool informed;
        private Language language;
        private LanguageLevel level;

        private readonly ExamTermController _examTermController;
        private Teacher teacher;

        public ExamTermDTO(TeacherController teacherController, Teacher teacher)
        {
            _examTermController = Injector.CreateInstance<ExamTermController>();
            this.teacher = teacher;
        }

        public ExamTermDTO()
        {
            _examTermController = Injector.CreateInstance<ExamTermController>();
        }

        public void SetTeacher(Teacher teacher)
        {
            this.teacher = teacher;
        }

        private int ExamID
        {
            get { return examID; }
            set { SetProperty(ref examID, value); }
        }

        public DateTime ExamDate
        {
            get { return examDate; }
            set { SetProperty(ref examDate, value); }
        }

        public string ExamTime
        {
            get { return examTime; }
            set { SetProperty(ref examTime, value); }
        }

        public int MaxStudents
        {
            get { return maxStudents; }
            set { SetProperty(ref maxStudents, value); }
        }

        public int CurrentlyAttending
        {
            get { return currentlyAttending; }
            set { SetProperty(ref currentlyAttending, value); }
        }

        public Language Language
        {
            get { return language; }
            set { SetProperty(ref language, value); }
        }

        public LanguageLevel Level
        {
            get { return level; }
            set { SetProperty(ref level, value); }
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

        public string ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case "ExamDate":
                    if (ExamDate <= DateTime.Today)
                        return "Exam date cannot be in the past";
                    break;
                case "ExamTime":
                    if (!_TimeRegex.IsMatch(ExamTime))
                        return "Invalid time format. Use HH:mm format.";
                    break;
                case "CurrentlyAttending":
                    if (CurrentlyAttending < 0 || CurrentlyAttending > MaxStudents)
                        return "Number of attending students must be between 0 and Max Students.";
                    break;
                case "MaxStudents":
                    if (MaxStudents <= 0)
                        return "Max students must be greater than 0.";
                    if (MaxStudents > 550)
                        return "Max students cannot exceed 550.";
                    break;
            }
            return null;
        }

        private readonly string[] _validatedProperties = { "ExamDate", "ExamTime", "CurrentlyAttending", "MaxStudents" };

        public bool IsValid()
        {
            foreach (var property in _validatedProperties)
            {
                if (ValidateProperty(property) != null)
                    return false;
            }
            if (!string.IsNullOrEmpty(IsValidExamTermTimeslot()))
                return false;
            return true;
        }

        private string IsValidExamTermTimeslot()
        {
            DateTime combinedDateTime = examDate.Date + TimeSpan.Parse(examTime);

            ExamTerm exam = new ExamTerm
            {
                ExamID = 0,
                ExamTime = combinedDateTime,
                MaxStudents = MaxStudents,
                CurrentlyAttending = CurrentlyAttending,
                Confirmed = false,
                Informed = false
            };
            //if (!_examTermController.ValidateExamTimeslot(exam, this.teacher))
               // return "Cannot create exam term due to time overlap.";
            return null;
        }

        public ExamTerm ToModelClass()
        {
            DateTime combinedDateTime;
            if (!string.IsNullOrEmpty(examTime))
                combinedDateTime = examDate.Date + TimeSpan.Parse(examTime);
            else
                combinedDateTime = DateTime.Now;

            return new ExamTerm
            {
                ExamID = this.examID,
                ExamTime = combinedDateTime,
                MaxStudents = MaxStudents,
                CurrentlyAttending = CurrentlyAttending,
                Language = Language,
                Level = Level,
                Confirmed = false,
                Informed = false
            };
        }

        public ExamTermDTO ToDTO(ExamTerm examTerm)
        {
            return new ExamTermDTO
            {
                examID = examTerm.ExamID,
                language = examTerm.Language,
                level = examTerm.Level,
                examDate = examTerm.ExamTime.Date,
                examTime = examTerm.ExamTime.ToString("HH:mm"),
                maxStudents = examTerm.MaxStudents,
                currentlyAttending = examTerm.CurrentlyAttending,
                confirmed = examTerm.Confirmed,
                informed = examTerm.Informed
            };
        }
    }
}
