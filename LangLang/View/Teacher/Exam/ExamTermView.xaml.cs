using LangLang.Controller;
using LangLang.DTO;
using LangLang.Domain.Model;
using LangLang.Observer;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

namespace LangLang.View.Teacher
{
    public partial class ExamTermView : Window, IObserver
    {
        public ObservableCollection<MailDTO> SentMails { get; set; }
        public ObservableCollection<StudentDTO>? Students { get; set; }
        public class ViewModel
        {
            public ObservableCollection<MailDTO> SentMails { get; set; }
            public ObservableCollection<StudentDTO>? Students { get; set; }
            public ViewModel()
            {
                Students = new ObservableCollection<StudentDTO>();
            }
        }
        public MailDTO SelectedSentMail { get; set; }
        public ViewModel StudentsTableViewModel { get; set; }
        public StudentDTO? SelectedStudent { get; set; }

        private ExamTermGradeDTO? _selectedGrade;
        public ExamTermGradeDTO? SelectedGrade
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly ExamTerm examTerm;
        private readonly Domain.Model.Teacher teacher;
        private readonly TeacherController _teacherController;
        private readonly StudentsController _studentController;
        private readonly DirectorController _directorController;
        private readonly ExamTermController _examTermController;
        private readonly ExamTermGradeController _examTermGradeController;
        private readonly MailController _mailController;

        private readonly Window window;

        public ExamTermView(ExamTerm examTerm, Domain.Model.Teacher teacher, Window window)
        {
            InitializeComponent();
            _teacherController = Injector.CreateInstance<TeacherController>();
            _studentController = Injector.CreateInstance<StudentsController>();
            _examTermController = Injector.CreateInstance<ExamTermController>();
            _directorController = Injector.CreateInstance<DirectorController>();
            _examTermGradeController = Injector.CreateInstance<ExamTermGradeController>();
            _mailController = Injector.CreateInstance<MailController>();

            this.teacher = teacher;
            this.examTerm = examTerm;
            this.window = window;

            StudentsTableViewModel = new ViewModel();

            DataContext = this;
            _teacherController.Subscribe(this);

            Update();

            Closing += ExamTermView_Closing;
        }

        private void ExamTermView_Closing(object? sender, CancelEventArgs e)
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
                StudentsTableViewModel.Students?.Clear();
                var students = _studentController.GetAllStudentsForExamTerm(examTerm.ExamID);

                if (students != null)
                {
                    foreach (Domain.Model.Student student in students)
                    {
                        StudentDTO studentDTO = new StudentDTO(student);
                        studentDTO = CheckStudentsGrades(studentDTO);

                        StudentsTableViewModel.Students?.Add(studentDTO);
                    }
                }
                else
                {
                    MessageBox.Show("No teachers found.");
                }

                AddExamTermInfo();
                AddExamTermStatus();
                CheckButtons();
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
            Course? course = _teacherController.GetCourseById(examTerm.CourseID);

            examTermLanguageTextBlock.Text = $"{course?.Language}";
            examTermLevelTextBlock.Text = $"{course?.Level}";
            examTermStartDateTextBlock.Text = examTerm.ExamTime.ToString("yyyy-MM-dd HH:mm");
            examTermMaxStudentsTextBlock.Text = examTerm.MaxStudents.ToString();
            examTermCurrentlyAttendingTextBlock.Text = examTerm.CurrentlyAttending.ToString();
        }

        private void AddExamTermStatus()
        {
            string examTermStatusCheck;

            if (HasExamTermStarted())
                examTermStatusCheck = "ExamTerm has started";
            else if (HasExamTermFinished())
            {
                if (!HasExamTermBeenGraded())
                    examTermStatusCheck = "ExamTerm has finished. It needs to be graded";
                else if (examTerm.Informed)
                    examTermStatusCheck = "ExamTerm grades have been sent to students";
                else if (HasExamTermBeenGraded())
                    examTermStatusCheck = "ExamTerm has been graded";
                else
                    examTermStatusCheck = "ExamTerm has finished";
            }
            else if (examTerm.Confirmed)
                examTermStatusCheck = "ExamTerm has been confirmed";
            else
                examTermStatusCheck = "ExamTerm hasn't started";

            examTermStatus.Text = examTermStatusCheck;
        }

        private void CheckButtons()
        {
            if (DateTime.Now.AddDays(+7) <= examTerm.ExamTime.Date || examTerm.Confirmed)
                Confirm.Visibility = Visibility.Collapsed;

            if (!HasExamTermStarted())
                Suspend.Visibility = Visibility.Collapsed;

            if (!HasExamTermFinished())
                Mark.Visibility = Visibility.Collapsed;

            if (HasExamTermBeenGraded())
                Mark.Visibility = Visibility.Collapsed;

            if (!IsDirectorPage() || examTerm.Informed == true)
                Email.Visibility = Visibility.Collapsed;
        }

        private bool HasExamTermStarted()
        {
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            TimeSpan examStartTime = TimeSpan.Parse(examTerm.ExamTime.ToString().Split()[1]);
            TimeSpan examEndTime = examStartTime.Add(new TimeSpan(4, 0, 0));

            if (DateTime.Today.Date.ToString("yyyy-MM-dd").Equals(examTerm.ExamTime.Date.ToString("yyyy-MM-dd"))) 
                if (currentTime >= examStartTime && currentTime <= examEndTime)
                    return true;
            return false;
        }

        private bool HasExamTermFinished()
        {
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            TimeSpan examStartTime = TimeSpan.Parse(examTerm.ExamTime.ToString().Split()[1]);
            TimeSpan examEndTime = examStartTime.Add(new TimeSpan(4, 0, 0));

            if (DateTime.Today.Date > examTerm.ExamTime.Date)
                return true;
            else if (DateTime.Today.Date == examTerm.ExamTime.Date)
                if (currentTime > examEndTime)
                    return true;
            return false;
        }

        public bool HasExamTermBeenGraded()
        {
            var grades = _examTermGradeController.GetExamTermGradesByTeacherExam(teacher.Id, examTerm.ExamID);
            var examTermStudents = _studentController.GetAllStudentsForExamTerm(examTerm.ExamID);

            if (grades.Count==0)
                return false;

            if (examTermStudents.Count != grades.Count)
                return false;

            return true;
        }

        private List<Domain.Model.Student> GetAllStudentsForExamTerm(int examTermID)
        {
            return _studentController.GetAllStudentsForExamTerm(examTerm.ExamID);
        }

        public StudentDTO CheckStudentsGrades(StudentDTO selectedStudent)
        {
            var examTermStudents = GetAllStudentsForExamTerm(examTerm.ExamID);
           
            foreach(Domain.Model.Student student in examTermStudents)
            {
                if (selectedStudent.id == student.Id)
                {
                    var grade = _examTermGradeController.GetExamTermGradeByStudentTeacherExam(student.Id, teacher.Id, examTerm.ExamID);

                    if (grade != null)
                        selectedStudent.ExamTermGrade = grade.Value;
                    else
                        selectedStudent.ExamTermGrade = 0;
                }
            }
            return selectedStudent;
        }

        private string GetMailMessage(Domain.Model.Student student)
        {
            string message;
            var examTermGrade = _examTermGradeController.GetExamTermGradeByStudentExam(student.Id, examTerm.ExamID);

            if (examTermGrade.Value > 5)
                message = "You have passed exam " + examTerm.Language.ToString() + " " + examTerm.Level.ToString() + " with grade " + examTermGrade.ToString();
            else
                message = "You have failed exam " + examTerm.Language.ToString() + " " + examTerm.Level.ToString();

            return message;
        }

        private void ConfirmExamTerm_Click(object sender, RoutedEventArgs e)
        {
            _examTermController.ConfirmExamTerm(examTerm.ExamID);
            MessageBox.Show("ExamTerm confirmed.");
            Update();
        }

        private void SuspendStudent_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent == null)
                MessageBox.Show("Please choose a student to delete!");
            else
            {
                _studentController.DeactivateStudentAccount(SelectedStudent.ToStudent());
                Update();
            }
        }

        private bool IsDirectorPage()
        {
            return window is LangLang.View.Director.DirectorPage;
        }

        private void GradeStudent_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent == null)
                MessageBox.Show("Please choose a student to grade!");
            else if (_examTermGradeController.IsStudentGraded(SelectedStudent.id, examTerm.ExamID))
                MessageBox.Show("This student is already graded!");
            else
            {
                Domain.Model.Student? student = _studentController.GetStudentById(SelectedStudent.id);
                GradeStudentForm gradeStudentForm = new GradeStudentForm(examTerm, teacher, student);

                gradeStudentForm.Closed += RefreshPage;

                gradeStudentForm.Show();
                gradeStudentForm.Activate();
            }
        }

        private void MailStudent_Click(object sender, RoutedEventArgs e)
        {
            examTerm.Informed = true;
            _examTermController.UpdateExamTerm(examTerm);

            var examTermStudents = GetAllStudentsForExamTerm(examTerm.ExamID);
            var director = _directorController.GetDirector();

            foreach (Domain.Model.Student student in examTermStudents)
            {
                MailToSend = new MailDTO();
                MailToSend.Sender = director.Email;
                MailToSend.Receiver = student.Email;
                MailToSend.TypeOfMessage = Domain.Model.Enums.TypeOfMessage.StudentGradeMessage;
                MailToSend.DateOfMessage = DateTime.Now;
                MailToSend.CourseId = examTerm.CourseID;
                MailToSend.Message = GetMailMessage(student);
                MailToSend.Answered = false;

                _mailController.Send(MailToSend.ToMail());
            }

            Update();
        }

        private void RefreshPage(object? sender, EventArgs e)
        {
            Update();
        }
    }
}
