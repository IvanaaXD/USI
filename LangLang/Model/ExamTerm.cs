using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangLang.Storage.Serialization;

namespace LangLang.Model
{
    public class ExamTerm : ISerializable
    {
        private int examID;
        private int courseID;
        private DateTime examTime;
        private int maxStudents;
        private int currentlyAttending;

        public int ExamID
        {
            get { return examID; }
            set { examID = value; }
        }

        public int CourseID
        {
            get { return courseID; }
            set { courseID = value; }
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

        public ExamTerm()
        {
        }

        public ExamTerm(int examID, int courseID, DateTime examTime, int maxStudents, int currentlyAttending)
        {
            this.examID = examID;
            this.courseID = courseID;
            this.examTime = examTime;
            this.maxStudents = maxStudents;
            this.currentlyAttending = currentlyAttending;
        }

        public override string ToString()
        {
            return $"ExamID: {examID}, CourseID: {courseID}, ExamTime: {examTime}, MaxStudents: {maxStudents}, CurrentlyAttending:{currentlyAttending}";
        }

        public string[] ToCSV()
        {
            string[] csvValues =
            {
                examID.ToString(),
                courseID.ToString(),
                examTime.ToString(),
                maxStudents.ToString(),
                currentlyAttending.ToString()
            };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            if (values.Length != 4)
            {
                //throw new ArgumentException("Invalid number of values in CSV");
            }

            ExamID = int.Parse(values[0]);
            CourseID = int.Parse(values[1]);
            ExamTime = DateTime.Parse(values[2]);
            MaxStudents = int.Parse(values[3]);
            CurrentlyAttending = int.Parse(values[4]);
        }
    }
}

