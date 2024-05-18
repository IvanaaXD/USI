using LangLang.Domain.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LangLang.DTO
{
    public class ExamTermGradeDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private int id;
        private int studentId;
        private int teacherId;
        private int examId;
        private int valueOfGrade;
        private int readingPoints;
        private int speakingPoints;
        private int writingPoints;
        private int listeningPoints;
        private string? firstName;
        private string? lastName;
        private string? email;

        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        public int StudentId
        {
            get { return studentId; }
            set { SetProperty(ref studentId, value); }
        }

        public int TeacherId
        {
            get { return teacherId; }
            set { SetProperty(ref teacherId, value); }
        }

        public int ExamId
        {
            get { return examId; }
            set { SetProperty(ref examId, value); }
        }

        public int Value
        {
            get { return valueOfGrade; }
            set { SetProperty(ref valueOfGrade, value); }
        }

        public int ReadingPoints
        {
            get { return readingPoints; }
            set { SetProperty(ref readingPoints, value); }
        }

        public int SpeakingPoints
        {
            get { return speakingPoints; }
            set { SetProperty(ref speakingPoints, value); }
        }

        public int WritingPoints
        {
            get { return writingPoints; }
            set { SetProperty(ref writingPoints, value); }
        }

        public int ListeningPoints
        {
            get { return listeningPoints; }
            set { SetProperty(ref listeningPoints, value); }
        }

        public string? FirstName
        {
            get { return firstName; }
            set { SetProperty(ref firstName, value); }
        }

        public string? LastName
        {
            get { return lastName; }
            set { SetProperty(ref lastName, value); }
        }

        public string? Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
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
                    case "ReadingPoints":
                        if (ReadingPoints < 0)
                            return "Reading cannot be negative";
                        if (ReadingPoints > 60)
                            return "Reading cannot be over 60";
                        break;
                    case "SpeakingPoints":
                        if (SpeakingPoints < 0)
                            return "Speaking cannot be negative";
                        if (SpeakingPoints > 50)
                            return "Speaking cannot be over 50";
                        break;
                    case "WritingPoints":
                        if (WritingPoints < 0)
                            return "Writing cannot be negative";
                        if (WritingPoints > 60)
                            return "Writing cannot be over 60";
                        break;
                    case "ListeningPoints":
                        if (ListeningPoints < 0)
                            return "Listening cannot be negative";
                        if (ListeningPoints > 40)
                            return "Listening cannot be over 40";
                        break;
                }
                return null;
            }
        }

        private readonly string[] _validatedProperties = { "ReadingPoints", "SpeakingPoints", "WritingPoints", "ListeningPoints" };

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

        public ExamTermGrade ToGrade()
        {
            return new ExamTermGrade
            {
                Id = id,
                TeacherId = teacherId,
                StudentId = studentId,
                ExamId = examId,
                ReadingPoints = readingPoints,
                SpeakingPoints = speakingPoints,
                WritingPoints = writingPoints,
                ListeningPoints = listeningPoints,
                Value = valueOfGrade,
            };
        }

        public ExamTermGradeDTO() {}

        public ExamTermGradeDTO(ExamTermGrade grade, Student student)
        {
            id = grade.Id;
            studentId = grade.StudentId;
            teacherId = grade.TeacherId;
            examId = grade.ExamId;
            readingPoints = grade.ReadingPoints;
            speakingPoints = grade.SpeakingPoints;
            writingPoints = grade.WritingPoints;
            listeningPoints = grade.ListeningPoints;
            valueOfGrade = grade.Value;
            firstName = student.FirstName;
            lastName = student.LastName;
            email = student.Email;
        }
    }
}
