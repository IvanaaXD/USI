using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model;
using System;
using System.ComponentModel;
using System.Windows;
using LangLang.Model.Enums;
using System.Security.RightsManagement;
using System.Drawing;

namespace LangLang.View.Teacher
{
    public partial class CoursePenaltyPointForm : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private MailDTO _mail;
        public MailDTO Mail
        {
            get { return _mail; }
            set
            {
                _mail = value;
                OnPropertyChanged(nameof(Mail));
            }
        }

        private Course course;
        private Model.Teacher teacher;
        private Model.Student student;
        private TeacherController teacherController;
        private StudentsController studentController;

        private bool _isFirstOptionSelected;
        private bool _isSecondOptionSelected;
        private bool _isThirdOptionSelected;

        public CoursePenaltyPointForm(Course course, Model.Teacher teacher, Model.Student student, TeacherController teacherController, StudentsController studentController)
        {
            InitializeComponent();
            DataContext = this;
            Mail = new MailDTO();

            this.course = course;
            this.teacherController = teacherController;
            this.studentController = studentController;
            this.teacher = teacher;
            this.student = student;

            firstNameTextBlock.Text = student.FirstName;
            lastNameTextBlock.Text = student.LastName;
        }

        public bool IsFirstOptionSelected
        {
            get { return _isFirstOptionSelected; }
            set
            {
                _isFirstOptionSelected = value;
                OnPropertyChanged(nameof(IsFirstOptionSelected));
                if (value)
                    Mail.Message = "You have gotten a penalty point from course"+course.Language.ToString()+" "+course.Level.ToString()+"Reason: Student didn't attend a course class.";
            }
        }
        public bool IsSecondOptionSelected
        {
            get { return _isSecondOptionSelected; }
            set
            {
                _isSecondOptionSelected = value;
                OnPropertyChanged(nameof(IsSecondOptionSelected));
                if (value)
                    Mail.Message = "You have gotten a penalty point from course"+course.Language.ToString()+" "+course.Level.ToString()+"Reason: Student is bothering other students during class.";
            }
        }
        public bool IsThirdOptionSelected
        {
            get { return _isThirdOptionSelected; }
            set
            {
                _isThirdOptionSelected = value;
                OnPropertyChanged(nameof(IsThirdOptionSelected));
                if (value)
                    Mail.Message = "You have gotten a penalty point from course"+course.Language.ToString()+" "+course.Level.ToString()+"Reason: Student didn't do homework.";
            }
        }

        public void GivePenaltyPoint_Click(object sender, RoutedEventArgs e)
        {
            if (IsFirstOptionSelected || IsSecondOptionSelected || IsThirdOptionSelected)
            {
                Mail.Sender = teacher.Email;
                Mail.Receiver = student.Email;
                Mail.TypeOfMessage = TypeOfMessage.PenaltyPointMessage;
                Mail.DateOfMessage = DateTime.Now;
                Mail.CourseId = course.Id;
                Mail.Answered = false;

                teacherController.SendMail(Mail.ToMail());

                studentController.GivePenaltyPoint(student.Id);
                studentController.Update(student);

                Close();
            }
            else
            {
                MessageBox.Show("Please name the reason for giving student a penalty point.");
            }
        }
    }
}
