
namespace LangLang.Domain.Model
{
    public class Grade
    {
        protected int id;
        protected int studentId;
        protected int teacherId;
        protected int courseId;

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

        public Grade()
        {
        }

        public Grade(int studentId, int teacherId, int courseId)
        {
            this.studentId = studentId;
            this.teacherId = teacherId;
            this.courseId = courseId;
        }
    }
}
