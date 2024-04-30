using LangLang.Model;
using LangLang.Controller;
using LangLang.DTO;
using LangLang.Observer;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;

namespace LangLang.View.Teacher
{
    public partial class CourseView : Window, IObserver
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

        private readonly Course course;
        private readonly Model.Teacher teacher;
        private readonly TeacherController teacherController;
        private readonly StudentsController studentController;

        public CourseView(Course course, Model.Teacher teacher, TeacherController teacherController, StudentsController studentController)
        {
            InitializeComponent();
            this.course = course;
            this.teacherController = teacherController;
            this.studentController = studentController;
            this.teacher = teacher;

            SentMailsTableViewModel = new ViewModel();
            ReceivedMailsTableViewModel = new ViewModel();
            StudentsTableViewModel = new ViewModel();

            DataContext = this;

            teacherController.Subscribe(this);

            if (!HasStudentAcceptingPeriodStarted())
            {
                ConfirmRequest.Visibility = Visibility.Collapsed;
                RejectRequest.Visibility = Visibility.Collapsed;
                PenaltyPoint.Visibility = Visibility.Collapsed;
                Mark.Visibility = Visibility.Collapsed;
            }
            else if (HasStudentAcceptingPeriodStarted() && !HasCourseStarted())
            {
                PenaltyPoint.Visibility = Visibility.Collapsed;
                Mark.Visibility = Visibility.Collapsed;
            }
            else if (HasCourseStarted() && !HasCourseFinished())
            {
                ConfirmRequest.Visibility = Visibility.Collapsed;
                RejectRequest.Visibility = Visibility.Collapsed;
                Mark.Visibility = Visibility.Collapsed;
            }
            else
            {
                ConfirmRequest.Visibility = Visibility.Collapsed;
                RejectRequest.Visibility = Visibility.Collapsed;
                PenaltyPoint.Visibility = Visibility.Collapsed;
            }

            AddCourseInfo();

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
                var students = studentController.GetAllStudentsForCourse(course.Id);

                if (students != null)
                {
                    foreach (Model.Student student in students)
                        StudentsTableViewModel.Students.Add(new StudentDTO(student));
                }
                else
                {
                    MessageBox.Show("No courses found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void ViewCourses_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddCourseInfo()
        {
            courseLanguageTextBlock.Text = $"{course.Language}";
            courseLevelTextBlock.Text = $"{course.Level}";
            courseStartDateTextBlock.Text = course.StartDate.ToString("yyyy-MM-dd HH:mm");
            courseDurationTextBlock.Text = course.Duration.ToString();
            courseCurrentyEnrolledTextBlock.Text = course.CurrentlyEnrolled.ToString();

            string courseStatusCheck;

            if (HasStudentAcceptingPeriodStarted() && !HasCourseStarted())
                courseStatusCheck = "Request Accepting Period";
            else if (HasCourseStarted() && !HasCourseFinished())
                courseStatusCheck = "Course Active";
            else if (HasCourseFinished())
                courseStatusCheck = "Course Ended. Students need to be graded.";
            else
                courseStatusCheck = "Requests Open For Students";

            courseStatus.Text = courseStatusCheck;
        }

        private bool HasStudentAcceptingPeriodStarted()
        {
            return (course.StartDate <= DateTime.Now.AddDays(7));
        }

        private bool HasCourseStarted()
        {
            return (course.StartDate <= DateTime.Now);
        }

        private bool HasCourseFinished()
        {
            return (course.StartDate.AddDays(7 * course.Duration) <= DateTime.Now);
        }

        private void ConfirmRequest_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RejectRequest_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PenaltyPoint_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GradeStudent_Click(object sender, RoutedEventArgs e)
        {

        }


        private void ReadMail_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AnswerMail_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
