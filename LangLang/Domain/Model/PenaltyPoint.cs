using LangLang.Storage.Serialization;
using System;

namespace LangLang.Domain.Model
{
    public class PenaltyPoint : ISerializable
    {
        private int id;
        private int studentId;
        private int courseId;
        private DateTime dateSent;
        private bool isDeleted;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public int StudentId
        {
            get { return studentId; }
            set { studentId = value; }
        }

        public int CourseId
        {
            get { return courseId; }
            set { courseId = value; }
        }

        public DateTime DateSent
        {
            get { return dateSent; }
            set { dateSent = value; }
        }

        public bool IsDeleted
        {
            get { return isDeleted; }
            set { isDeleted = value; }
        }
        public PenaltyPoint() { }

        public PenaltyPoint(int studentId, int courseId, DateTime dateSent, bool isDeleted)
        {
            this.studentId = studentId;
            this.courseId = courseId;
            this.dateSent = dateSent;
            this.isDeleted = isDeleted;
        }

        public string[] ToCSV()
        {
            string[] csvValues =
            {
                id.ToString(),
                studentId.ToString(),
                courseId.ToString(),
                dateSent.ToString("yyyy-MM-dd HH:mm"),
                isDeleted.ToString(),
            };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            studentId = int.Parse(values[1]);
            courseId = int.Parse(values[2]);
            dateSent = DateTime.ParseExact(values[3], "yyyy-MM-dd HH:mm", null);
            isDeleted = bool.Parse(values[4]);
        }

    }
}
