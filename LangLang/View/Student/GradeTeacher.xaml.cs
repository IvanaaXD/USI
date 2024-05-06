using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model;
using System.Windows;

namespace LangLang.View.Student
{
    /// <summary>
    /// Interaction logic for GradeTeacher.xaml
    /// </summary>
    public partial class GradeTeacher : Window
    {
        private TeacherDTO teacher { get; set; }
        private StudentGradeDTO teacherGrade { get; set; }

        private int studentId, courseId;

        private TeacherController teacherController;
        private StudentsController studentController;
        private DirectorController directorController;

        public GradeTeacher(int studentId, int courseId)
        {
            InitializeComponent();
            DataContext = this;

            teacherController = new TeacherController();
            studentController = new StudentsController();
            directorController = new DirectorController();

            this.studentId = studentId;
            this.courseId = courseId;

            InitializeDTO();
            SetGUIElements();

        }

        private void SetGUIElements()
        {
            completedCourseName.Text = GetCourseName(courseId);
            firstNameTextBlock.Text = teacher.FirstName;
            lastNameTextBlock.Text = teacher.LastName;
            emailTextBlock.Text = teacher.Email;
        }
        private void InitializeDTO()
        {
            Model.Teacher teacher = directorController.GetTeacherByCourse(courseId);
            this.teacher = new TeacherDTO(teacher);
            teacherGrade = new StudentGradeDTO();
        }

        public void GradeStudent_Click(object sender, RoutedEventArgs e)
        {
            if (teacherGrade.IsValid)
            {
                teacherGrade.TeacherId = teacher.Id;
                teacherGrade.CourseId = courseId;
                teacherGrade.StudentId = studentId;
                studentController.GradeStudentCourse(teacherGrade.ToCourseGrade());
            }
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private string GetCourseName(int courseId)
        {
            Course course = teacherController.GetCourseById(courseId);
            return course.Language.ToString() + " " + course.Level.ToString();
        }
    }
}
