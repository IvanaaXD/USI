using LangLang.Controller;
using LangLang.Domain.Model;
using LangLang.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace LangLang.DTO
{
    public class ExamTermDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private int examID;
        private int courseID;
        private DateTime examDate;
        private string examTime;
        private int maxStudents;
        private int currentlyAttending;
        private bool confirmed;
        private bool informed;
        private string languageAndLevel;
        private int gradeValue;
        private int points;

        private readonly TeacherController _teacherController;
        private readonly ExamTermController _examTermController;
        private readonly ExamTermGradeController _examTermGrade = new ExamTermGradeController(new ExamTermGradeRepository());
        private readonly Teacher teacher;
        ExamTermGrade grade;

        public ExamTermDTO(TeacherController teacherController, Teacher teacher)
        {
            _teacherController = teacherController;
            _examTermController = new ExamTermController(new ExamTermRepository(), teacherController);
            this.teacher = teacher;
        }
        public List<string> LanguageAndLevelValues
        {
            get
            {
                List<string> languageLevelNames = new List<string>();

                TeacherRepository teacherDAO = new TeacherRepository();
                List<Course> courses = teacherDAO.GetAllCourses();

                foreach (Course course in courses)
                {
                    languageLevelNames.Add($"{course.Language} {course.Level}");
                }

                return languageLevelNames;
            }
        }

        public int ExamID
        {
            get { return examID; }
            set { SetProperty(ref examID, value); }
        }

        public int CourseID
        {
            get { return courseID; }
            set { SetProperty(ref courseID, value); }
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

        public bool Confirmed
        {
            get { return confirmed; }
            set { SetProperty(ref confirmed, value); }
        }
        public bool Informed
        {
            get { return informed; }
            set { SetProperty(ref informed, value); }
        }
        public string LanguageAndLevel
        {
            get { return languageAndLevel; }
            set { SetProperty(ref languageAndLevel, value); }
        }

        public int Points
        {
            get { return points; }
            set { SetProperty(ref points, value); }
        }

        public int GradeValue
        {
            get { return gradeValue; }
            set { SetProperty(ref gradeValue, value); }
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
                    case "ExamDate":
                        if (ExamDate != null && ExamDate <= DateTime.Today)
                            return "Exam date cannot be in the past";
                        break;
                    case "StartTime":
                        if (ExamTime != null && !_TimeRegex.IsMatch(ExamTime))
                            return "Format is not good. Try again.";
                        break;
                    case "CurrentlyAttending":
                        if (CurrentlyAttending < 0 || (CurrentlyAttending > MaxStudents))
                            return "Number of attending students on the exam can't be less than 0 or greater than max number of students.";
                        break;

                    case "MaxStudents":
                        if (MaxStudents <= 0)
                            return "Max number of students must be above 0 for exam";
                        if (MaxStudents > 550)
                            return "Max number of students must be <= 550 on the exam";
                        break;


                }
                return null;
            }
        }
        private readonly string[] _validatedProperties = { "ExamDate", "StartTime", "CurrentlyAttending", "MaxStudents" };

        public bool IsValid
        {
            get
            {
                foreach (var property in _validatedProperties)
                {
                   
                    if (this[property] != null)
                        return false;
                }
                if (!string.IsNullOrEmpty(IsValidExamTermTimeslot()))
                    return false;
                return true;

            }
        }

        private string IsValidExamTermTimeslot()
        {
            DateTime combinedDateTime = examDate.Date + TimeSpan.Parse(examTime);

            ExamTerm exam = new ExamTerm
            {
                ExamID = ExamID,
                CourseID = CourseID,
                ExamTime = combinedDateTime,
                MaxStudents = MaxStudents,
                CurrentlyAttending = CurrentlyAttending,
                Confirmed = Confirmed,
                Informed = Informed
            };
            if (!_examTermController.ValidateExamTimeslot(exam, this.teacher))
                return "Cannot create exam because of exam time overlaps!";
            return null;
        }
        public ExamTerm ToExamTerm()
        {
            DateTime combinedDateTime = examDate.Date + TimeSpan.Parse(examTime);

            return new ExamTerm
            {
                ExamID = ExamID,
                CourseID = CourseID,
                ExamTime = combinedDateTime,
                MaxStudents = MaxStudents,
                CurrentlyAttending = CurrentlyAttending,
                Confirmed = Confirmed,
                Informed = Informed
            };
        }

        public ExamTermDTO(ExamTerm examTerm, Teacher teacher)
        {
            examID = examTerm.ExamID;
            courseID = examTerm.CourseID;
            examDate = examTerm.ExamTime;
            maxStudents = examTerm.MaxStudents;
            currentlyAttending = examTerm.CurrentlyAttending;
            confirmed = examTerm.Confirmed;
            informed = examTerm.Informed;
            TeacherRepository teacherDAO = new TeacherRepository();
            languageAndLevel = teacherDAO.FindLanguageAndLevel(courseID);
            this.teacher = teacher;
        }
        public ExamTermDTO(ExamTerm examTerm)
        {
            examID = examTerm.ExamID;
            courseID = examTerm.CourseID;
            examDate = examTerm.ExamTime;
            maxStudents = examTerm.MaxStudents;
            currentlyAttending = examTerm.CurrentlyAttending;
            confirmed = examTerm.Confirmed;
            informed = examTerm.Informed;
            TeacherRepository teacherDAO = new TeacherRepository();
            languageAndLevel = teacherDAO.FindLanguageAndLevel(courseID);
        }
        public ExamTermDTO(ExamTerm examTerm, int studentId)
        {
            examID = examTerm.ExamID;
            courseID = examTerm.CourseID;
            examDate = examTerm.ExamTime;
            maxStudents = examTerm.MaxStudents;
            currentlyAttending = examTerm.CurrentlyAttending;
            confirmed = examTerm.Confirmed;
            informed = examTerm.Informed;

            TeacherRepository teacherDAO = new TeacherRepository();
            languageAndLevel = teacherDAO.FindLanguageAndLevel(courseID);
            //TeacherController teacherController = new TeacherController();
            grade = _examTermGrade.GetExamTermGradeByStudentExam(studentId, examTerm.ExamID);
            if (grade == null)
            {
                gradeValue = 0;
                points = 0;
            }
            else
            {
                gradeValue = grade.Value;
                points = grade.ReadingPoints + grade.ListeningPoints + grade.SpeakingPoints + grade.WritingPoints;
            }

        }
    }
}
