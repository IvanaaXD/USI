using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model;
using LangLang.Observer;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace LangLang.View.Teacher
{
    public partial class ExamTermView : Window, IObserver
    {
        public ObservableCollection<MailDTO> Mails { get; set; }
        public ObservableCollection<StudentDTO> Students { get; set; }
        public class ViewModel
        {
            public ObservableCollection<MailDTO> Mails { get; set; }
            public ObservableCollection<StudentDTO> Students { get; set; }

            public ViewModel()
            {
                Mails = new ObservableCollection<MailDTO>();
                Students = new ObservableCollection<StudentDTO>();
            }
        }

        public ViewModel MailsTableViewModel { get; set; }
        public ViewModel StudentsTableViewModel { get; set; }
        public MailDTO SelectedMail { get; set; }

        private readonly ExamTerm examTerm;
        private readonly Model.Teacher teacher;
        private readonly TeacherController teacherController;
        private readonly StudentsController studentController;

        public ExamTermView(ExamTerm examTerm, Model.Teacher teacher, TeacherController teacherController, StudentsController studentController)
        {
            InitializeComponent();
            this.examTerm = examTerm;
            this.teacherController = teacherController;
            this.studentController = studentController;
            this.teacher = teacher;

            MailsTableViewModel = new ViewModel();
            StudentsTableViewModel = new ViewModel();

            DataContext = this;

            teacherController.Subscribe(this);  

            if (examTerm.ExamTime.Date <= DateTime.Now.AddDays(-7) || examTerm.Confirmed)
            {
                Confirm.Visibility = Visibility.Collapsed;
            }

            Update();
        }
        public void Update()
        {
            try
            {
                MailsTableViewModel.Mails.Clear();
                var mails = teacherController.GetAllMails();

                if (mails != null)
                {
                    foreach (Mail mail in mails)
                        MailsTableViewModel.Mails.Add(new MailDTO(mail));
                }
                else
                {
                    MessageBox.Show("No teachers found.");
                }

                StudentsTableViewModel.Students.Clear();
                var students = studentController.GetAllStudentsForExamTerm(examTerm.ExamID);

                if (students != null)
                {
                    foreach (Model.Student student in students)
                        StudentsTableViewModel.Students.Add(new StudentDTO(student));
                }
                else
                {
                    MessageBox.Show("No teachers found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void ConfirmExamTerm_Click(object sender, RoutedEventArgs e)
        {
            teacherController.ConfirmExamTerm(this.examTerm.ExamID);
            MessageBox.Show("ExamTerm confirmed.");
            Confirm.Visibility = Visibility.Collapsed;
        }

        private void ReadMail_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AnswerMail_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
