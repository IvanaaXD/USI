using LangLang.Controller;
using LangLang.DTO;
using LangLang.Domain.Model;
using LangLang.Observer;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

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
        private readonly Domain.Model.Teacher teacher;
        private readonly TeacherController teacherController;
        private readonly StudentsController studentController;
        private readonly CourseController courseController;
        private readonly CourseGradeController courseGradeController;
        private readonly MainController mainController;
        private readonly MailController mailController;

        public CourseView(Course course, Domain.Model.Teacher teacher, MainController mainController)
        {
            InitializeComponent();
            this.course = course;
            teacherController = Injector.CreateInstance<TeacherController>();
            studentController = Injector.CreateInstance<StudentsController>();
            courseController = Injector.CreateInstance<CourseController>();
            courseGradeController = Injector.CreateInstance<CourseGradeController>();
            mailController = Injector.CreateInstance<MailController>();

            this.mainController = mainController;
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

            var students = courseController.GetCourseStudents(studentController, course);

            if (students != null)
                foreach (Domain.Model.Student student in students)
                {
                    StudentDTO dtoStudent = new StudentDTO(student);
                    dtoStudent.ActivityGrade = 0;
                    dtoStudent.KnowledgeGrade = 0;
                    if (!courseController.HasCourseStarted(course))
                    {
                        if (mailController.IsStudentAccepted(student, course.Id))
                            dtoStudent.AddedToCourse = true;
                    }
                    if (courseController.HasCourseFinished(course, courseController.GetStudentCount(studentController, course)))
                    {
                        CourseGrade grade = courseGradeController.GetCourseGradeByStudentTeacher(student.Id, teacher.Id, course.Id);
                        if (grade == null)
                        {
                            dtoStudent.ActivityGrade = 0;
                            dtoStudent.KnowledgeGrade = 0;
                        }
                        else
                        {
                            dtoStudent.ActivityGrade = grade.StudentActivityValue;
                            dtoStudent.KnowledgeGrade = grade.StudentKnowledgeValue;
                        }
                    }
                    dtoStudent.PenaltyPoints = studentController.GetPenaltyPointCount(student.Id);

                    StudentsTableViewModel.Students.Add(dtoStudent);
                }

            else
                MessageBox.Show("No students found.");
        }

        private void RefreshMails()
        {
            SentMailsTableViewModel.SentMails.Clear();
            ReceivedMailsTableViewModel.ReceivedMails.Clear();

            var receivedMails = mailController.GetReceivedCourseMails(teacher, course.Id);
            var sentMails = mailController.GetSentCourseMail(teacher, course.Id);

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

            if (courseController.HasStudentAcceptingPeriodEnded(course) && !courseController.HasCourseStarted(course))
                courseStatusCheck = "Final Student Enrollments";
            else if (courseController.HasStudentAcceptingPeriodEnded(course) && !courseController.HasGradingPeriodStarted(course))
                courseStatusCheck = "Course Active";
            else if (courseController.HasStudentAcceptingPeriodEnded(course) && !courseController.HasCourseFinished(course, courseController.GetStudentCount(studentController, course)))
                courseStatusCheck = "Student Grading Period";
            else if (courseController.HasCourseFinished(course, courseController.GetStudentCount(studentController, course)))
                courseStatusCheck = "Course Finished And Students Graded";
            else
                courseStatusCheck = "Requests Open For Students";

            courseStatus.Text = courseStatusCheck;
        }
        private void CheckButtons()
        {
            ConfirmRequest.Visibility = Visibility.Collapsed;
            RejectRequest.Visibility = Visibility.Collapsed;
            PenaltyPoint.Visibility = Visibility.Collapsed;
            Mark.Visibility = Visibility.Collapsed;

            if (!courseController.HasStudentAcceptingPeriodEnded(course))
            {
                ConfirmRequest.Visibility = Visibility.Visible;
                RejectRequest.Visibility = Visibility.Visible;
            }

            else if (courseController.HasStudentAcceptingPeriodEnded(course) && !courseController.HasGradingPeriodStarted(course))
                PenaltyPoint.Visibility = Visibility.Visible;

            else if (courseController.HasStudentAcceptingPeriodEnded(course) && !courseController.HasCourseFinished(course, courseController.GetStudentCount(studentController, course)))
                Mark.Visibility = Visibility.Visible;
        }

        private void ConfirmRequest_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent == null)
                MessageBox.Show("Please choose a student to accept to the course!");
            else
            {
                StudentDTO selected = SelectedStudent;
                Domain.Model.Student student = studentController.GetStudentById(selected.id);

                if (SelectedStudent.AddedToCourse == true)
                    MessageBox.Show("Student has been added to the course already.");

                else
                {
                    courseController.IncrementCourseCurrentlyEnrolled(course.Id);

                    MailToSend.Sender = teacher.Email;
                    MailToSend.Receiver = student.Email;
                    MailToSend.TypeOfMessage = Domain.Model.Enums.TypeOfMessage.AcceptEnterCourseRequestMessage;
                    MailToSend.DateOfMessage = DateTime.Now;
                    MailToSend.CourseId = course.Id;
                    MailToSend.Message = "You have been accepted to course " + course.Language.ToString() + " " + course.Level.ToString();
                    MailToSend.Answered = false;

                    mailController.Send(MailToSend.ToMail());
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
                Domain.Model.Student student = studentController.GetStudentById(SelectedStudent.id);

                if (SelectedStudent.AddedToCourse == true)
                    MessageBox.Show("Student has been added to the course already.");
                else
                {
                    CourseRejectionForm rejectionForm = new CourseRejectionForm(course, teacher, student, mainController);

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
                Domain.Model.Student student = studentController.GetStudentById(SelectedStudent.id);
                CoursePenaltyPointForm penaltyPointForm = new CoursePenaltyPointForm(course, teacher, student, mainController);
                penaltyPointForm.Closed += RefreshPage;

                penaltyPointForm.Show();
                penaltyPointForm.Activate();
            }
        }

        private void GradeStudent_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent == null)
                MessageBox.Show("Please choose a student to grade!");

            else if (courseGradeController.IsStudentGraded(SelectedStudent.id, course.Id))
                MessageBox.Show("This student is already graded!");

            else
            {
                Domain.Model.Student student = studentController.GetStudentById(SelectedStudent.id);
                GradeStudentCourseForm gradeStudentForm = new GradeStudentCourseForm(course, teacher, student, mainController);

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

        private void KickStudentOut(Domain.Model.Student student)
        {
            student.ActiveCourseId = -1;
            studentController.Update(student);
            courseController.DecrementCourseCurrentlyEnrolled(course.Id);
        }

        private void ApproveDroppingOut_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedReceivedMail != null)
            {
                MailDTO mail = SelectedReceivedMail;
                mailController.SetMailToAnswered(mail.ToMail());
                Domain.Model.Student studentSender = studentController.GetStudentByEmail(mail.Sender);

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
                mailController.SetMailToAnswered(mail.ToMail());
                Domain.Model.Student studentSender = studentController.GetStudentByEmail(mail.Sender);
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

                if (SelectedReceivedMail.TypeOfMessage == Domain.Model.Enums.TypeOfMessage.QuitCourseRequest && SelectedReceivedMail.Answered == false)
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
