using LangLang.Storage.Serialization;

namespace LangLang.Domain.Model
{
    public class StudentGrade : Grade, ISerializable
    {
        private int value;

        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public StudentGrade()
        {
        }

        public StudentGrade(int studentId, int teacherId, int courseId, int value) : base(studentId,teacherId,courseId)
        {
            this.value = value;
        }
        public override string ToString()
        {
            return $"Id: {id}, StudentId: {studentId}, TeacherId: {teacherId}, CourseId: {courseId}, Value: {value}";
        }

        public string[] ToCSV()
        {
            string[] csvValues =
            {
                id.ToString(),
                studentId.ToString(),
                teacherId.ToString(),
                courseId.ToString(),
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
            CourseId = int.Parse(values[3]);
            Value = int.Parse(values[4]);
        }
    }
}
