using LangLang.Storage.Serialization;

namespace LangLang.Domain.Model
{
    public class ExamTermGrade : ISerializable
    {
        private int id;
        private int studentId;
        private int teacherId;
        private int examId;
        private int readingPoints;
        private int speakingPoints;
        private int writingPoints;
        private int listeningPoints;
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

        public int ReadingPoints
        {
            get { return readingPoints; }
            set { readingPoints = value; }
        }

        public int SpeakingPoints
        {
            get { return speakingPoints; }
            set { speakingPoints = value; }
        }

        public int WritingPoints
        {
            get { return writingPoints; }
            set { writingPoints = value; }
        }

        public int ListeningPoints
        {
            get { return listeningPoints; }
            set { listeningPoints = value; }
        }

        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public ExamTermGrade()
        {
        }

        public ExamTermGrade(int studentId, int teacherId, int examId, int readingPoints, int speakingPoints, int writingPoints, int listeningPoints, int value)
        {
            this.studentId = studentId;
            this.teacherId = teacherId;
            this.examId = examId;
            this.readingPoints = readingPoints;
            this.speakingPoints = speakingPoints;
            this.writingPoints = writingPoints;
            this.listeningPoints = listeningPoints;
            this.value = value;
        }

        public override string ToString()
        {
            return $"ReadingPoints: {readingPoints}, SpeakingPoints: {speakingPoints}, WritingPoints: {writingPoints}, ListeningPoints: {listeningPoints}, Value: {value}";
        }

        public string[] ToCSV()
        {
            string[] csvValues =
            {
                id.ToString(),
                studentId.ToString(),
                teacherId.ToString(),
                examId.ToString(),
                readingPoints.ToString(),
                speakingPoints.ToString(),
                writingPoints.ToString(),
                listeningPoints.ToString(),
                value.ToString()
            };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            if (values.Length == 0)
                return;

            Id = int.Parse(values[0]);
            StudentId = int.Parse(values[1]);
            TeacherId = int.Parse(values[2]);
            ExamId = int.Parse(values[3]);
            ReadingPoints = int.Parse(values[4]);
            SpeakingPoints = int.Parse(values[5]);
            WritingPoints = int.Parse(values[6]);
            ListeningPoints = int.Parse(values[7]);
            Value = int.Parse(values[8]);
        }
    }
}
