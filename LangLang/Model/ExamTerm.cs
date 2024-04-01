using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Model
{
    public class ExamTerm
    {
        private int examID;
        private int courseID;
        private DateTime examTime;
        private int maxStudents;

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

        public ExamTerm()
        {
        }

        public ExamTerm(int examID, int courseID, DateTime examTime, int maxStudents)
        {
            this.examID = examID;
            this.courseID = courseID;
            this.examTime = examTime;
            this.maxStudents = maxStudents;
        }

        public override string ToString()
        {
            return $"ExamID: {examID}, CourseID: {courseID}, ExamTime: {examTime}, MaxStudents: {maxStudents}";
        }

        public string[] ToCSV()
        {
            string[] csvValues =
            {
                examID.ToString(),
                courseID.ToString(),
                examTime.ToString(),
                maxStudents.ToString()
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
        }
    }
}

