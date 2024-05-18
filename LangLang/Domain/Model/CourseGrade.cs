using LangLang.Storage.Serialization;

namespace LangLang.Model
{
    public class CourseGrade : ISerializable
    {
        private int id;
        private int studentId;
        private int teacherId;
        private int courseId;
        private int studentActivityValue;
        private int studentKnowledgeValue;

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

        public int CourseId
        {
            get { return courseId; }
            set { courseId = value; }
        }

        public int StudentActivityValue
        {
            get { return studentActivityValue; }
            set { studentActivityValue = value; }
        }
        public int StudentKnowledgeValue
        {
            get { return studentKnowledgeValue; }
            set { studentKnowledgeValue = value; }
        }

        public CourseGrade()
        {
        }

        public CourseGrade(int studentId, int teacherId, int courseId, int studentActivityValue, int studentKnowledgeValue)
        {
            this.studentId = studentId;
            this.teacherId = teacherId;
            this.courseId = courseId;
            this.studentActivityValue = studentActivityValue;
            StudentKnowledgeValue = studentKnowledgeValue;
        }

        public override string ToString()
        {
            return $"Id: {id}, StudentId: {studentId}, TeacherId: {teacherId}, CourseId: {courseId}, ActivityGrade: {studentActivityValue}, KnowledgeGrade: {studentKnowledgeValue}";
        }

        public string[] ToCSV()
        {
            string[] csvValues =
            {
                id.ToString(),
                studentId.ToString(),
                teacherId.ToString(),
                courseId.ToString(),
                studentActivityValue.ToString(),
                studentKnowledgeValue.ToString()
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
            CourseId = int.Parse(values[3]);
            StudentActivityValue = int.Parse(values[4]);
            StudentKnowledgeValue = int.Parse(values[5]);
        }
    }
}
