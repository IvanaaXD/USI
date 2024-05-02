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

            SentMailsTableViewModel = new ViewModel();
            ReceivedMailsTableViewModel = new ViewModel();
            StudentsTableViewModel = new ViewModel();

            DataContext = this;

            teacherController.Subscribe(this);

            AddCourseInfo();
            AddCourseStatus();
            CheckButtons();
            CheckStudentsGrades();

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
                var students = studentController.GetAllStudentsRequestingCourse(course.Id);

                if (HasCourseStarted() && !HasCourseFinished())
                {
                    students = studentController.GetAllStudentsEnrolledCourse(course.Id);
                }
                else if (HasCourseFinished())
                {
                    students = studentController.GetAllStudentsForCourseGrading(course.Id);
                }

                if (students != null)
                {
                    foreach (Model.Student student in students)
                        StudentsTableViewModel.Students.Add(new StudentDTO(student));

                }
                else
                {
                    MessageBox.Show("No students found.");
                }
                CheckButtons();
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
        }

        private void AddCourseStatus()
        {
            string courseStatusCheck;

            if (HasStudentAcceptingPeriodStarted() && !HasCourseStarted())
                courseStatusCheck = "Request Accepting Period";
            else if (HasCourseStarted() && !HasCourseFinished())
                courseStatusCheck = "Course Active";
            else if (HasCourseFinished() && !HasCourseBeenGraded())
                courseStatusCheck = "Course Finished. Student Grading Period";
            else if (HasCourseBeenGraded())
                courseStatusCheck = "Course Finished And Students Graded";
            else
                courseStatusCheck = "Requests Open For Students";

            courseStatus.Text = courseStatusCheck;
        }
        private void CheckButtons()
        {
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
            else if (HasCourseFinished() && !HasCourseBeenGraded())
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

        public bool HasCourseBeenGraded()
        {
            var grades = teacherController.GetCourseGradesByTeacherCourse(teacher.Id, course.Id);
            if (grades.Count == 0)
            {
                return false;
            }

            foreach (CourseGrade grade in grades)
            {
                if (!teacherController.IsStudentGradedCourse(grade.StudentId))
                {
                    return false;
                }
            }
            return true;
        }

        public void CheckStudentsGrades()
        {
            if (HasCourseFinished())
            {
                var courseStudents = studentController.GetAllStudentsForCourseGrading(course.Id);

                foreach (Model.Student student in courseStudents)
                {
                    var grade = teacherController.GetCourseGradesByStudentTeacherCourse(student.Id, teacher.Id, course.Id);
                    SelectedGrade = new CourseGradeDTO();

                    if (grade != null)
                    {
                        SelectedGrade.Value = grade.Value;
                        student.ActiveCourseId = -1;
                        studentController.Update(student);
                        Update();
                    }
                    else
                    {
                        SelectedGrade.Value = 0;
                    }
                }
            }
        }

        private void ConfirmRequest_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent == null)
            {
                MessageBox.Show("Please choose a student to accept to the course!");
            }
            else
            {
                Model.Student student = studentController.GetStudentById(SelectedStudent.id);
                if (student.ActiveCourseId != -1)
                {
                    MessageBox.Show("Student has been added to the course already.");
                }
                else
                {
                    student.ActiveCourseId = course.Id;
                    teacherController.IncrementCourseCurrentlyEnrolled(course.Id);
                    student.RegisteredCoursesIds.Remove(course.Id);
                    studentController.Update(student);
                    Update();
                }
            }
        }

        private void RejectRequest_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent == null)
            {
                MessageBox.Show("Please choose a student to reject from a course!");
            }
            else
            {
                Model.Student student = studentController.GetStudentById(SelectedStudent.id);
                if (student.ActiveCourseId != -1)
                {
                    MessageBox.Show("Student has been added to the course already.");
                }
                else
                {
                    // page for showing the reason for being rejected
                    student.RegisteredCoursesIds.Remove(course.Id);
                    studentController.Update(student);
                    StudentsTableViewModel.Students.Remove(SelectedStudent);
                    Update();
                }
            }
        }

        private void PenaltyPoint_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent == null)
            {
                MessageBox.Show("Please choose a student to give a penalty point to!");
            }
            else
            {
                Model.Student student = studentController.GetStudentById(SelectedStudent.id);
                // open window for reason
                studentController.GivePenaltyPoint(student.Id);
                if (student.PenaltyPoints >= 3)
                {
                    studentController.Delete(student.Id);
                }
                studentController.Update(student);
                Update();
            }
        }

        private void GradeStudent_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent == null)
            {
                MessageBox.Show("Please choose a student to grade!");
            }
            else if (teacherController.IsStudentGradedCourse(SelectedStudent.id))
            {
                MessageBox.Show("This student is already graded!");
            }
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
            AddCourseStatus();
            CheckStudentsGrades();
            CheckButtons();
            Update();
        }

        private void ReadMail_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AnswerMail_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
