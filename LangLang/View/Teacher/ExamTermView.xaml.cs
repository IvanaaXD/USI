using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model;
using LangLang.Observer;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace LangLang.View.Teacher
{
    public partial class ExamTermView : Window, IObserver
    {
        public ObservableCollection<MailDTO> ReceivedMails { get; set; }
        public ObservableCollection<MailDTO> SentMails { get; set; }
        public ObservableCollection<StudentDTO> Students { get; set; }
        public class ViewModel
        {
            public ObservableCollection<MailDTO> ReceivedMails { get; set; }
            public ObservableCollection<MailDTO> SentMails { get; set; }
            public ObservableCollection<StudentDTO> Students { get; set; }

            public ViewModel()
            {
                SentMails = new ObservableCollection<MailDTO>();
                ReceivedMails = new ObservableCollection<MailDTO>();
                Students = new ObservableCollection<StudentDTO>();
            }
        }

        public ViewModel SentMailsTableViewModel { get; set; }
        public ViewModel ReceivedMailsTableViewModel { get; set; }
        public ViewModel StudentsTableViewModel { get; set; }
        public StudentDTO SelectedStudent { get; set; }
        public MailDTO SelectedSentMail { get; set; }
        public MailDTO SelectedReceivedMail { get; set; }

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

            SentMailsTableViewModel = new ViewModel();
            ReceivedMailsTableViewModel = new ViewModel();
            StudentsTableViewModel = new ViewModel();

            DataContext = this;
            teacherController.Subscribe(this);

            if (DateTime.Now.AddDays(+7) <= examTerm.ExamTime.Date || examTerm.Confirmed)
            {
                Confirm.Visibility = Visibility.Collapsed;
            }

            if (!HasExamTermStarted())
            {
                Suspend.Visibility = Visibility.Collapsed;
            } 

            if (!HasExamTermFinished())
            {
                Mark.Visibility = Visibility.Collapsed;
            }

            AddExamTermInfo();
            Update();
        }

        public void Update()
        {
            try
            {
                SentMailsTableViewModel.SentMails.Clear();
                ReceivedMailsTableViewModel.ReceivedMails.Clear();

                var allMails = teacherController.GetAllMails();

                if (allMails != null)
                {
                    foreach (Mail mail in allMails)
                    {
                        if (mail.Recevier == this.teacher)
                        {
                            ReceivedMails.Add(new MailDTO(mail));
                        }
                        else if (mail.Sender == this.teacher)
                        {
                            SentMails.Add(new MailDTO(mail));
                        }
                    }
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

        private void ViewExamTerms_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddExamTermInfo()
        {
            Course course = teacherController.GetCourseById(examTerm.CourseID);

            examTermLanguageTextBlock.Text = $"{course.Language}";
            examTermLevelTextBlock.Text = $"{course.Level}";
            examTermStartDateTextBlock.Text = examTerm.ExamTime.ToString("yyyy-MM-dd HH:mm");
            examTermMaxStudentsTextBlock.Text = examTerm.MaxStudents.ToString();
            examTermCurrentlyAttendingTextBlock.Text = examTerm.CurrentlyAttending.ToString();

            string examTermStatusCheck;

            if (HasExamTermStarted())
            {
                examTermStatusCheck = "ExamTerm has started";
            }
            else if (HasExamTermFinished())
            {
                examTermStatusCheck = "ExamTerm has finished. It needs to be graded";
            }
            else if(examTerm.Confirmed)
            {
                examTermStatusCheck = "ExamTerm has been confirmed";
            } 
            else 
            {
                examTermStatusCheck = "ExamTerm hasn't started";
            }

            examTermStatus.Text = examTermStatusCheck;
        }

        private bool HasExamTermStarted()
        {
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            TimeSpan examStartTime = TimeSpan.Parse(examTerm.ExamTime.ToString().Split()[1]);
            TimeSpan examEndTime = examStartTime.Add(new TimeSpan(4, 0, 0));

            if (DateTime.Today.Date.ToString("yyyy-MM-dd").Equals(examTerm.ExamTime.Date.ToString("yyyy-MM-dd"))) 
            {
                if (currentTime >= examStartTime && currentTime <= examEndTime)
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasExamTermFinished()
        {
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            TimeSpan examStartTime = TimeSpan.Parse(examTerm.ExamTime.ToString().Split()[1]);
            TimeSpan examEndTime = examStartTime.Add(new TimeSpan(4, 0, 0));

            if (DateTime.Today.Date > examTerm.ExamTime.Date)
            {
                return true;
            }
            else if (DateTime.Today.Date == examTerm.ExamTime.Date)
            {
                if (currentTime > examEndTime)
                {
                    return true;
                }
            }
            return false;
        }

        private void ConfirmExamTerm_Click(object sender, RoutedEventArgs e)
        {
            teacherController.ConfirmExamTerm(this.examTerm.ExamID);
            MessageBox.Show("ExamTerm confirmed.");
            Confirm.Visibility = Visibility.Collapsed;
        }

        private void SuspendStudent_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent == null)
            {
                MessageBox.Show("Please choose a student to delete!");
            }
            else
            {
                studentController.Delete(SelectedStudent.id);
            }
        }

        private void GradeStudent_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent == null)
            {
                MessageBox.Show("Please choose a student to grade!");
            }
            else
            {
                Model.Student student = studentController.GetStudentById(SelectedStudent.id);
                GradeStudentForm gradeStudentForm = new GradeStudentForm(examTerm, teacher, student, teacherController, studentController);
                gradeStudentForm.Show();
            }
        }

        private void ReadMail_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AnswerMail_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
