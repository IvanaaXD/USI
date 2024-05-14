using LangLang.Model;
using LangLang.Controller;
using LangLang.DTO;
using LangLang.Observer;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Linq;

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

        private MailDTO _mail;

        public MailDTO MailToSend
        {
            get { return _mail; }
            set
            {
                _mail = value;
                OnPropertyChanged(nameof(Course));
            }
        }

        private CourseGradeDTO _selectedGrade;
        public CourseGradeDTO SelectedGrade
        {
            get { return _selectedGrade; }
            set
            {
                if (_selectedGrade != value)
                {
                    _selectedGrade = value;
                    OnPropertyChanged(nameof(SelectedGrade));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
            MailToSend = new MailDTO();

            SentMailsTableViewModel = new ViewModel();
            ReceivedMailsTableViewModel = new ViewModel();
            StudentsTableViewModel = new ViewModel();

            DataContext = this;

            teacherController.Subscribe(this);

            Update();
        }

        public void Update()
        {
            try
            {
                AddCourseInfo();
                AddCourseStatus();
                CheckButtons();
                RefreshMails();
                RefreshStudents();


            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void RefreshStudents()
        {
            StudentsTableViewModel.Students.Clear();
            var students = studentController.GetAllStudentsRequestingCourse(course.Id);

            if (HasCourseStarted() && !HasCourseFinished())
            {
                students = studentController.GetAllStudentsEnrolledCourse(course.Id);
            }
            else if (HasCourseFinished())
            {
                students = studentController.GetAllStudentsCompletedCourse(course.Id);
            }

            if (students != null)
                foreach (Model.Student student in students)
                {
                    StudentDTO dtoStudent = new StudentDTO(student);
                    dtoStudent.Grade = 0;
                    if (!HasCourseStarted())
                    {
                        if (teacherController.IsStudentAccepted(student, course.Id))
                            dtoStudent.AddedToCourse = true;
                    }
                    if (HasCourseFinished())
                    {
                        dtoStudent.Grade = teacherController.GetCourseGradesByStudentTeacherCourse(student.Id, teacher.Id, course.Id).Value;
                    }

                    StudentsTableViewModel.Students.Add(dtoStudent);
                }

            else
                MessageBox.Show("No students found.");
        }

        private void RefreshMails()
        {
            SentMailsTableViewModel.SentMails.Clear();
            ReceivedMailsTableViewModel.ReceivedMails.Clear();

            var receivedMails = teacherController.GetReceivedCourseMails(teacher, course.Id);
            var sentMails = teacherController.GetSentCourseMails(teacher, course.Id);

            if (receivedMails != null)
                foreach (Mail mail in receivedMails)
                {
                    ReceivedMailsTableViewModel.ReceivedMails.Add(new MailDTO(mail));
                }

            if (sentMails != null)
                foreach (Mail mail in sentMails)
                {
                    SentMailsTableViewModel.SentMails.Add(new MailDTO(mail));
                }

            else
                MessageBox.Show("No teachers found.");
        }

        private void ViewCourses_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddCourseInfo()
        {
            courseLanguageTextBlock.Text = $"{course.Language}";
            courseLevelTextBlock.Text = $"{course.Level}";
            courseStartDateTextBlock.Text = course.StartDate.ToString("dd-MM-yyyy HH:mm");
            courseDurationTextBlock.Text = course.Duration.ToString();
            courseCurrentyEnrolledTextBlock.Text = course.CurrentlyEnrolled.ToString();
        }

        private void AddCourseStatus()
        {
            string courseStatusCheck;

            if (HasStudentAcceptingPeriodEnded() && !HasCourseStarted())
                courseStatusCheck = "Final Student Enrollments";
            else if (HasCourseStarted() && !HasGradingPeriodStarted())
                courseStatusCheck = "Course Active";
            else if (HasGradingPeriodStarted() && !HasCourseFinished())
                courseStatusCheck = "Student Grading Period";
            else if (HasCourseFinished())
                courseStatusCheck = "Course Finished And Students Graded";
            else
                courseStatusCheck = "Requests Open For Students";

            courseStatus.Text = courseStatusCheck;
        }
        private void CheckButtons()
        {
            if (!HasStudentAcceptingPeriodEnded())
            {
                PenaltyPoint.Visibility = Visibility.Collapsed;
                Mark.Visibility = Visibility.Collapsed;
            }
            else if (HasStudentAcceptingPeriodEnded() && !HasCourseStarted())
            {
                ConfirmRequest.Visibility = Visibility.Collapsed;
                RejectRequest.Visibility = Visibility.Collapsed;
                PenaltyPoint.Visibility = Visibility.Collapsed;
                Mark.Visibility = Visibility.Collapsed;
            }
            else if (HasCourseStarted() && !HasGradingPeriodStarted())
            {
                ConfirmRequest.Visibility = Visibility.Collapsed;
                RejectRequest.Visibility = Visibility.Collapsed;
                Mark.Visibility = Visibility.Collapsed;
            }
            else if (HasGradingPeriodStarted() && !HasCourseFinished())
            {
                ConfirmRequest.Visibility = Visibility.Collapsed;
                RejectRequest.Visibility = Visibility.Collapsed;
                PenaltyPoint.Visibility = Visibility.Collapsed;
            }
            else
            {
                ConfirmRequest.Visibility = Visibility.Collapsed;
                RejectRequest.Visibility = Visibility.Collapsed;
                PenaltyPoint.Visibility = Visibility.Collapsed;
                Mark.Visibility = Visibility.Collapsed;
            }
        }

        private bool HasStudentAcceptingPeriodEnded()
        {
            return (course.StartDate <= DateTime.Now.AddDays(7));
        }

        private bool HasCourseStarted()
        {
            return (course.StartDate <= DateTime.Now);
        }

        private bool HasGradingPeriodStarted()
        {
            return (course.StartDate.AddDays(7 * course.Duration) <= DateTime.Now);
        }

        public bool HasCourseFinished()
        {
            if (course.StartDate.AddDays(course.Duration * 7) >= DateTime.Now)
                return false;

            var students = studentController.GetAllStudentsEnrolledCourse(course.Id);

            if (students.Count == 0)
                return true;

            return false;
        }

        private void ConfirmRequest_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent == null)
                MessageBox.Show("Please choose a student to accept to the course!");
            else
            {
                StudentDTO selected = SelectedStudent;
                Model.Student student = studentController.GetStudentById(selected.id);
                if (SelectedStudent.AddedToCourse == true)
                {
                    MessageBox.Show("Student has been added to the course already.");
                }
                else
                {
                    teacherController.IncrementCourseCurrentlyEnrolled(course.Id);

                    MailToSend.Sender = teacher.Email;
                    MailToSend.Receiver = student.Email;
                    MailToSend.TypeOfMessage = Model.Enums.TypeOfMessage.AcceptEnterCourseRequestMessage;
                    MailToSend.DateOfMessage = DateTime.Now;
                    MailToSend.CourseId = course.Id;
                    MailToSend.Message = "You have been accepted to course " + course.Language.ToString() + " " + course.Level.ToString();
                    MailToSend.Answered = false;

                    teacherController.SendMail(MailToSend.ToMail());
                    selected.AddedToCourse = true;

                    AddCourseInfo();
                    Update();
                }
            }
        }

        private void RejectRequest_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent == null)
                MessageBox.Show("Please choose a student to reject from a course!");
            else
            {
                Model.Student student = studentController.GetStudentById(SelectedStudent.id);

                if (SelectedStudent.AddedToCourse == true)
                    MessageBox.Show("Student has been added to the course already.");
                else
                {
                    CourseRejectionForm rejectionForm = new CourseRejectionForm(course, teacher, student, teacherController, studentController);

                    rejectionForm.Closed += RefreshPage;

                    rejectionForm.Show();
                    rejectionForm.Activate();
                }
            }
        }

        private void PenaltyPoint_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent == null)
                MessageBox.Show("Please choose a student to give a penalty point to!");

            else
            {
                Model.Student student = studentController.GetStudentById(SelectedStudent.id);
                CoursePenaltyPointForm penaltyPointForm = new CoursePenaltyPointForm(course, teacher, student, teacherController, studentController);
                penaltyPointForm.Closed += RefreshPage;

                penaltyPointForm.Show();
                penaltyPointForm.Activate();
            }
        }

        private void GradeStudent_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent == null)
                MessageBox.Show("Please choose a student to grade!");

            else if (teacherController.IsStudentGradedCourse(SelectedStudent.id, course.Id))
                MessageBox.Show("This student is already graded!");

            else
            {
                Model.Student student = studentController.GetStudentById(SelectedStudent.id);
                GradeStudentCourseForm gradeStudentForm = new GradeStudentCourseForm(course, teacher, student, teacherController, studentController);

                gradeStudentForm.Closed += RefreshPage;

                gradeStudentForm.Show();
                gradeStudentForm.Activate();
            }
        }
        private void RefreshPage(object sender, EventArgs e)
        {
            AddCourseInfo();
            AddCourseStatus();
            CheckButtons();
            Update();
        }

        private void KickStudentOut(Model.Student student)
        {
            student.ActiveCourseId = -1;
            studentController.Update(student);
            teacherController.DecrementCourseCurrentlyEnrolled(course.Id);
        }

        private void ApproveDroppingOut_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedReceivedMail != null)
            {
                MailDTO mail = SelectedReceivedMail;
                teacherController.AnswerMail(mail.Id);
                Model.Student studentSender = studentController.GetStudentByEmail(mail.Sender);

                KickStudentOut(studentSender);

                approveDropOut.Visibility = Visibility.Collapsed;
                rejectDropOut.Visibility = Visibility.Collapsed;

                AddCourseInfo();
                Update();
            }
            else
                MessageBox.Show("Please select mail you want to view!");
        }

        private void RejectDroppingOut_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedReceivedMail != null)
            {
                MailDTO mail = SelectedReceivedMail;
                teacherController.AnswerMail(mail.Id);
                Model.Student studentSender = studentController.GetStudentByEmail(mail.Sender);
                studentController.GivePenaltyPoint(studentSender.Id);
                studentSender = studentController.GetStudentByEmail(mail.Sender);

                KickStudentOut(studentSender);

                approveDropOut.Visibility = Visibility.Collapsed;
                rejectDropOut.Visibility = Visibility.Collapsed;

                AddCourseInfo();
                Update();
            }
            else
                MessageBox.Show("Please select mail you want to view!");
        }
        private void ReceivedMailDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedReceivedMail != null)
            {
                receivedMailSenderTextBlock.Text = SelectedReceivedMail.Sender;
                receivedMailDateTextBlock.Text = SelectedReceivedMail.DateOfMessage.ToString("dd-MM-yyyy HH:mm");
                receivedMailTypeTextBlock.Text = SelectedReceivedMail.TypeOfMessage.ToString();
                receivedMailMessageTextBlock.Text = SelectedReceivedMail.Message;

                if (SelectedReceivedMail.TypeOfMessage == Model.Enums.TypeOfMessage.QuitCourseRequest && SelectedReceivedMail.Answered == false)
                {
                    approveDropOut.Visibility = Visibility.Visible;
                    rejectDropOut.Visibility = Visibility.Visible;
                }
                else
                {
                    approveDropOut.Visibility = Visibility.Collapsed;
                    rejectDropOut.Visibility = Visibility.Collapsed;
                }
            }
        }
        private void SentMailDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedSentMail != null)
            {
                sentMailSenderTextBlock.Text = SelectedSentMail.Receiver;
                sentMailDateTextBlock.Text = SelectedSentMail.DateOfMessage.ToString("dd-MM-yyyy HH:mm");
                sentMailTypeTextBlock.Text = SelectedSentMail.TypeOfMessage.ToString();
                sentMailMessageTextBlock.Text = SelectedSentMail.Message;

            }
        }
    }
}
