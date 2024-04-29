using LangLang.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangLang.Storage.Serialization;

namespace LangLang.Model
{
    public class Grade : ISerializable
    {
        private int id;
        private int studentId;
        private int teacherId;
        private int examId;
        private int value;

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

        public int TeacherId
        {
            get { return teacherId; }
            set { teacherId = value; }
        }

        public int ExamId
        {
            get { return examId; }
            set { examId = value; }
        }

        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public Grade()
        {
        }

        public Grade(int studentId, int teacherId, int examTermId, int value)
        {
            this.studentId = studentId;
            this.teacherId = teacherId;
            this.examId = examId;
            this.value = value;
        }

        public override string ToString()
        {
            return $"StudentId: {studentId}, TeacherId: {teacherId}, ExamTermId: {examId}, Value: {value}";
        }

        public string[] ToCSV()
        {
            string[] csvValues =
            {
                studentId.ToString(),
                teacherId.ToString(),
                examId.ToString(),
                value.ToString()
            };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            StudentId = int.Parse(values[0]);
            TeacherId = int.Parse(values[1]);
            ExamId = int.Parse(values[2]);
            Value = int.Parse(values[3]);
        }
    }
}
