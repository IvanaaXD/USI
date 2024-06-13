using LangLang.Controller;
using LangLang.DTO;
using System;
using System.ComponentModel;
using System.Windows;
using LangLang.Domain.Model;
using LangLang.Domain.Model.Enums;

namespace LangLang.View.Teacher
{
    public partial class CoursePenaltyPointForm : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private Course course;
        private Domain.Model.Teacher teacher;
        private Domain.Model.Student student;

        private MailController mailController;
        private StudentsController studentController;
        string messageBody;

        private bool _isFirstOptionSelected;
        private bool _isSecondOptionSelected;
        private bool _isThirdOptionSelected;

        public CoursePenaltyPointForm(Course course, Domain.Model.Teacher teacher, Domain.Model.Student student)
        {
            InitializeComponent();

            this.course = course;
            this.teacher = teacher;
            this.student = student;

            mailController = Injector.CreateInstance<MailController>();
            studentController = Injector.CreateInstance<StudentsController>();

            DataContext = this;

            SetFormInfo();
        }

        public void SetFormInfo()
        {
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
                    messageBody = "You have gotten a penalty point from course " + course.Language.ToString() + " " + course.Level.ToString() + ". Reason: Student didn't attend a course class.";
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
                    messageBody = "You have gotten a penalty point from course " + course.Language.ToString() + " " + course.Level.ToString() + ". Reason: Student is bothering other students during class.";
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
                    messageBody = "You have gotten a penalty point from course " + course.Language.ToString() + " " + course.Level.ToString() + ". Reason: Student didn't do homework.";
            }
        }

        public void GivePenaltyPoint_Click(object sender, RoutedEventArgs e)
        {
            if (IsFirstOptionSelected || IsSecondOptionSelected || IsThirdOptionSelected)
            {
                var examTerm = new ExamTerm();
                examTerm.ExamID = -1;

                mailController.ConstructMail(teacher, student, course, examTerm, TypeOfMessage.PenaltyPointMessage, messageBody);
                studentController.GivePenaltyPoint(student.Id);

                Close();
            }
            else
            {
                MessageBox.Show("Please name the reason for giving student a penalty point.");
            }
        }
    }
}
