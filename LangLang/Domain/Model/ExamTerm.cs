using System;
using LangLang.Domain.Model.Enums;
using LangLang.Storage.Serialization;

namespace LangLang.Domain.Model
{
    public class ExamTerm : ISerializable
    {
        private int examID;
        private Language language;
        private LanguageLevel languageLevel;
        private DateTime examTime;
        private int maxStudents;
        private int currentlyAttending;
        private bool confirmed;
        private bool informed;

        public int ExamID
        {
            get { return examID; }
            set { examID = value; }
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
        public DateTime ExamTime
        {
            get { return examTime; }
            set { examTime = value; }
        }

        public int MaxStudents
        {
            get { return maxStudents; }
            set { maxStudents = value; }
        }

        public int CurrentlyAttending
        {
            get { return currentlyAttending; }
            set { currentlyAttending = value; }
        }

        public bool Confirmed
        {
            get { return confirmed; }
            set { confirmed = value; }
        }

        public bool Informed
        {
            get { return informed; }
            set { informed = value; }
        }
        public ExamTerm()
        {
        }
        public ExamTerm(int examID, Language language, LanguageLevel languageLevel,int courseID, DateTime examTime, int maxStudents, int currentlyAttending)
        {
            this.examID = examID;
            this.language = language;
            this.languageLevel = languageLevel;
            this.examTime = examTime;
            this.maxStudents = maxStudents;
            this.currentlyAttending = currentlyAttending;
            this.confirmed = false;
            this.informed = false;
        }
        public ExamTerm(int examID, int courseID, DateTime examTime, int maxStudents, int currentlyAttending)
        {
            this.examID = examID;
            this.examTime = examTime;
            this.maxStudents = maxStudents;
            this.currentlyAttending = currentlyAttending;
            this.confirmed = false;
            this.informed = false;  
        }

        public override string ToString()
        {
            return $"ExamID: {examID}, ExamTime: {examTime}, MaxStudents: {maxStudents}, CurrentlyAttending:{currentlyAttending}, Confirmed:{confirmed}, Informed:{informed}";
        }

        public string[] ToCSV()
        {
            string[] csvValues =
            {
                examID.ToString(),
                Language.ToString(),
                Level.ToString(),
                examTime.ToString("yyyy-MM-dd HH:mm"),
                maxStudents.ToString(),
                currentlyAttending.ToString(),
                confirmed.ToString(),
                informed.ToString()
            };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            ExamID = int.Parse(values[0]);
            Language = (Language)Enum.Parse(typeof(Language), values[2]);
            Level = (LanguageLevel)Enum.Parse(typeof(LanguageLevel), values[3]);
            ExamTime = DateTime.ParseExact(values[4], "yyyy-MM-dd HH:mm", null);
            MaxStudents = int.Parse(values[5]);
            CurrentlyAttending = int.Parse(values[6]);
            Confirmed = bool.Parse(values[7]);
            Informed = bool.Parse(values[8]);
        }
    }
}

