using LangLang.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace LangLang.DTO
{
    public class ExamTermDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private int examID;
        private int courseID;
        private DateTime examTime;
        private int maxStudents;
        private int currentlyAttending;

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

        public DateTime ExamTime
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

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "StartDate":
                        if (ExamTime < DateTime.Today)
                            return "Start date cannot be in the past";
                        break;
                    case "CurrentlyAttending":
                        if (CurrentlyAttending < 0 ||  (CurrentlyAttending > MaxStudents))
                            return "Number of attenging students can't be less than 0 or greater than max number of students.";
                        break;
                    case "MaxStudents":
                        if (MaxStudents != 0)
                            return "Max number of students must be 0 for online courses";
                        break;

                }
                return null;
            }
        }

        public bool IsValid
        {
            get
            {
                // Add validation logic if needed
                return true;
            }
        }

        public ExamTerm ToExamTerm()
        {
            return new ExamTerm
            {
                ExamID = ExamID,
                CourseID = CourseID,
                ExamTime = ExamTime,
                MaxStudents = MaxStudents,
                CurrentlyAttending = CurrentlyAttending
            };
        }

        public ExamTermDTO()
        {

        }

        public ExamTermDTO(ExamTerm examTerm)
        {
            examID = examTerm.ExamID;
            courseID = examTerm.CourseID;
            examTime = examTerm.ExamTime;
            maxStudents = examTerm.MaxStudents;
            currentlyAttending = examTerm.CurrentlyAttending;
        }
    }
}
