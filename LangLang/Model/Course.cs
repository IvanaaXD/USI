using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LangLang.Model
{
    public enum Level
    {
        A1,
        A2,
        B1,
        B2,
        C1,
        C2
    }

    public class Course
    {
        private int courseID;
        private string language;
        private Level level;
        private int duration;
        private List<DayOfWeek> workDays;
        private DateTime startDate;
        private bool isOnline;
        private int maxEnrolledStudents;
        private List<int> examTerms;

        public int CourseID
        {
            get { return courseID; }
            set { courseID = value; }
        }

        public string Language
        {
            get { return language; }
            set { language = value; }
        }

        public Level Level
        {
            get { return level; }
            set { level = value; }
        }

        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }
        public List<DayOfWeek> WorkDays
        {
            get { return workDays; }
            set { workDays = value; }
        }

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        public bool IsOnline
        {
            get { return isOnline; }
            set { isOnline = value; }
        }

        public int MaxEnrolledStudents
        {
            get { return maxEnrolledStudents; }
            set { maxEnrolledStudents = value; }
        }

        public List<int> ExamTerms
        {
            get { return examTerms; }
            set {  examTerms = value; }
        }

        public Course()
        {
        }
        public Course(int courseID, string language, Level level, int duration, List<DayOfWeek> workDays, DateTime startDate, bool isOnline, int maxEnrolledStudents, List<int> examTerms)
        {
            this.courseID = courseID;
            this.language = language;
            this.level = level;
            this.duration = duration;
            this.workDays = workDays;
            this.startDate = startDate;
            this.isOnline = isOnline;
            this.maxEnrolledStudents = maxEnrolledStudents;
            this.examTerms = examTerms;
        }

        public string[] ToCSV()
        {
            string workDaysStr = string.Join(",", workDays.Select(d => d.ToString()));

            string examTermsStr = string.Join(",", examTerms);

            string[] csvValues =
            {
                CourseID.ToString(),
                Language,
                Level.ToString(),
                Duration.ToString(),
                workDaysStr,
                StartDate.ToString("yyyy-MM-dd"),
                IsOnline.ToString(),
                MaxEnrolledStudents.ToString(),
                examTermsStr
            };

            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            if (values.Length != 9)
            {
                throw new ArgumentException("Invalid number of values in CSV");
            }

            CourseID = int.Parse(values[0]);
            Language = values[1];
            Level = (Level)Enum.Parse(typeof(Level), values[2]);
            Duration = int.Parse(values[3]);
            WorkDays = values[4].Split(',').Select(d => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), d)).ToList();
            StartDate = DateTime.ParseExact(values[5], "yyyy-MM-dd", null);
            IsOnline = bool.Parse(values[6]);
            MaxEnrolledStudents = int.Parse(values[7]);
            ExamTerms = values[8].Split(',').Select(int.Parse).ToList();
        }

    }
}
