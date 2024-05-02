using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model;
using LangLang.Observer;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
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

        private ExamTermGradeDTO _selectedGrade;
        public ExamTermGradeDTO SelectedGrade
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

            AddExamTermInfo();
            AddExamTermStatus();
            CheckStudentsGrades();
            CheckButtons();
            Update();

            Closing += ExamTermView_Closing;
        }

        private void ExamTermView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (Window window in Application.Current.Windows.OfType<Window>().Where(w => w != this))
            {
                window.Close();
            }
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
            if (Owner != null)
            {
                Owner.Visibility = Visibility.Visible;
                this.Visibility = Visibility.Collapsed;
            }
        }

        private void AddExamTermInfo()
        {
            Course course = teacherController.GetCourseById(examTerm.CourseID);

            examTermLanguageTextBlock.Text = $"{course.Language}";
            examTermLevelTextBlock.Text = $"{course.Level}";
            examTermStartDateTextBlock.Text = examTerm.ExamTime.ToString("yyyy-MM-dd HH:mm");
            examTermMaxStudentsTextBlock.Text = examTerm.MaxStudents.ToString();
            examTermCurrentlyAttendingTextBlock.Text = examTerm.CurrentlyAttending.ToString();
        }

        private void AddExamTermStatus()
        {
            string examTermStatusCheck;

            if (HasExamTermStarted())
            {
                examTermStatusCheck = "ExamTerm has started";
            }
            else if (HasExamTermFinished())
            {
                if (!HasExamTermBeenGraded())
                {
                    examTermStatusCheck = "ExamTerm has finished. It needs to be graded";
                }
                else
                {
                    examTermStatusCheck = "ExamTerm has been graded";
                }

            }
            else if (examTerm.Confirmed)
            {
                examTermStatusCheck = "ExamTerm has been confirmed";
            }
            else
            {
                examTermStatusCheck = "ExamTerm hasn't started";
            }

            examTermStatus.Text = examTermStatusCheck;
        }

        private void CheckButtons()
        {
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

            if (HasExamTermBeenGraded())
            {
                Mark.Visibility = Visibility.Collapsed;
            }
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

        public bool HasExamTermBeenGraded()
        {
            var grades = teacherController.GetExamTermGradesByTeacherExam(teacher.Id, examTerm.ExamID);
            if (grades.Count==0)
            {
                return false;
            }

            foreach (ExamTermGrade grade in grades)
            {
                if (!teacherController.IsStudentGradedExamTerm(grade.StudentId))
                {
                    return false;
                }
            }
            return true;
        }

        public void CheckStudentsGrades()
        {
            var examTermStudents = studentController.GetAllStudentsForExamTerm(examTerm.ExamID);
           
            foreach(Model.Student student in examTermStudents)
            {
                var grade = teacherController.GetExamTermGradeByStudentTeacherExam(student.Id, teacher.Id, examTerm.ExamID);
                SelectedGrade = new ExamTermGradeDTO();

                if (grade != null)
                {
                    SelectedGrade.Value = grade.Value;
                }
                else
                {
                    SelectedGrade.Value = 0;
                }
            }
        }

        private void ConfirmExamTerm_Click(object sender, RoutedEventArgs e)
        {
            teacherController.ConfirmExamTerm(this.examTerm.ExamID);
            MessageBox.Show("ExamTerm confirmed.");
            AddExamTermStatus();
            CheckButtons();
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
                Update();
            }
        }
        private void GradeStudent_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent == null)
            {
                MessageBox.Show("Please choose a student to grade!");
            }
            else if (teacherController.IsStudentGradedExamTerm(SelectedStudent.id))
            {
                MessageBox.Show("This student is already graded!");
            }
            else
            {
                Model.Student student = studentController.GetStudentById(SelectedStudent.id);
                GradeStudentForm gradeStudentForm = new GradeStudentForm(examTerm, teacher, student, teacherController, studentController);

                gradeStudentForm.Closed += RefreshPage;

                gradeStudentForm.Show();
                gradeStudentForm.Activate();
            }
        }

        private void RefreshPage(object sender, EventArgs e)
        {
            AddExamTermStatus();
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
