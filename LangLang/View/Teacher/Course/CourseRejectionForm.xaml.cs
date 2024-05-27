using LangLang.Controller;
using LangLang.DTO;
using System;
using System.ComponentModel;
using System.Windows;
using LangLang.Domain.Model;
using LangLang.Domain.Model.Enums;

namespace LangLang.View.Teacher
{
    public partial class CourseRejectionForm : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Course course;
        private Domain.Model.Teacher teacher;
        private Domain.Model.Student student;

        private StudentsController studentController;
        private MailController mailController;

        public string RejectReason { get; set; }

        public CourseRejectionForm(Course course, Domain.Model.Teacher teacher, Domain.Model.Student student)
        {
            InitializeComponent();

            this.course = course;
            this.teacher = teacher;
            this.student = student;

            studentController = Injector.CreateInstance<StudentsController>();
            mailController = Injector.CreateInstance<MailController>();

            DataContext = this;

            SetFormInfo();
        }
        public void SetFormInfo()
        {
            firstNameTextBlock.Text = student.FirstName;
            lastNameTextBlock.Text = student.LastName;
            RejectReason = " ";
        }

        public void SendRejection_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(mailBodyTextBlock.Text))
            {
                string messageBody = "You have been rejected from course " + course.Language.ToString() + " " + course.Level.ToString() + ". Reason: " + RejectReason;
                studentController.RejectStudentApplication(student, course);
                mailController.ConstructMail(teacher, student, course, TypeOfMessage.DenyEnterCourseRequestMessage, messageBody);

                Close();
            }
            else
            {
                MessageBox.Show("Please name the reason for rejecting the student from the course.");
            }

        }
    }
}
