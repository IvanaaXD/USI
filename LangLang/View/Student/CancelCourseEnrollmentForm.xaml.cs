using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model;
using System;
using System.Windows;

namespace LangLang.View.Student
{
    /// <summary>
    /// Interaction logic for CancelCourseEnrollmentForm.xaml
    /// </summary>
    public partial class CancelCourseEnrollmentForm : Window
    {
        int studentId, courseId;

        StudentsController studentController;
        TeacherController teacherController;
        DirectorController directorController;
        MailController mailController;

        MailDTO mailDTO { get; set; }

        // Event handler to turn off drop out button functionality in student form when this window closes.
        public event EventHandler WindowClosed;

        public CancelCourseEnrollmentForm(int studentId, int courseId)
        {
            InitializeComponent();
            this.studentId = studentId;
            this.courseId = courseId;
            studentController = new StudentsController();
            teacherController = new TeacherController();
            directorController = new DirectorController();
            mailController = new MailController();

            CreateMailDTO();

            courseTextBox.Text = courseTextBox.Text + GetCourseName(courseId);

            DataContext = mailDTO;
        }

        private void SendExplanationButton_Click(object sender, RoutedEventArgs e)
        {
            if (mailDTO.IsValid)
            {
                mailController.Send(mailDTO.ToMail());

                Close();
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CreateMailDTO()
        {
            Model.Student student = studentController.GetStudentById(studentId);
            Model.Teacher teacher = directorController.GetTeacherByCourse(courseId);
            
            mailDTO = new MailDTO(mailController.PrepareQuitCourseMail(student.Email,teacher.Email,courseId));
        }
        private string GetCourseName(int courseId)
        {
            Course course = teacherController.GetCourseById(courseId);
            return course.Language.ToString() + " " + course.Level.ToString();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            WindowClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}
