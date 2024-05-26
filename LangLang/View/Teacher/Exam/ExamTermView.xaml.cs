using LangLang.Controller;
using LangLang.DTO;
using LangLang.Domain.Model;
using LangLang.Observer;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace LangLang.View.Teacher
{
    public partial class ExamTermView : Window, IObserver
    {
        public ObservableCollection<StudentDTO>? Students { get; set; }
        public class ViewModel
        {
            public ObservableCollection<StudentDTO>? Students { get; set; }
            public ViewModel()
            {
                Students = new ObservableCollection<StudentDTO>();
            }
        }
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly ExamTerm examTerm;
        private readonly Domain.Model.Teacher teacher;
        private readonly TeacherController teacherController;
        private readonly StudentsController studentController;
        private readonly ExamTermController examTermController;
        private readonly ExamTermGradeController examTermGradeController;
        private readonly MainController mainController;

        public ExamTermView(ExamTerm examTerm, Domain.Model.Teacher teacher, MainController mainController)
        {
            InitializeComponent();
            this.teacherController = Injector.CreateInstance<TeacherController>();
            this.studentController = Injector.CreateInstance<StudentsController>();
            this.examTermController = Injector.CreateInstance<ExamTermController>();
            this.examTermGradeController = Injector.CreateInstance<ExamTermGradeController>();
            this.mainController = mainController;
            this.teacher = teacher;
            this.examTerm = examTerm;

            StudentsTableViewModel = new ViewModel();

            DataContext = this;
            teacherController.Subscribe(this);

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
                var students = studentController.GetAllStudentsForExamTerm(examTerm.ExamID);

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
            Course? course = teacherController.GetCourseById(examTerm.CourseID);

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
            var grades = examTermGradeController.GetExamTermGradesByTeacherExam(teacher.Id, examTerm.ExamID);
            var examTermStudents = studentController.GetAllStudentsForExamTerm(examTerm.ExamID);

            if (grades.Count==0)
                return false;

            if (examTermStudents.Count != grades.Count)
                return false;

            return true;
        }

        public StudentDTO CheckStudentsGrades(StudentDTO selectedStudent)
        {
            var examTermStudents = studentController.GetAllStudentsForExamTerm(examTerm.ExamID);
           
            foreach(Domain.Model.Student student in examTermStudents)
            {
                if (selectedStudent.id == student.Id)
                {
                    var grade = examTermGradeController.GetExamTermGradeByStudentTeacherExam(student.Id, teacher.Id, examTerm.ExamID);

                    if (grade != null)
                        selectedStudent.ExamTermGrade = grade.Value;
                    else
                        selectedStudent.ExamTermGrade = 0;
                }
            }
            return selectedStudent;
        }

        private void ConfirmExamTerm_Click(object sender, RoutedEventArgs e)
        {
            examTermController.ConfirmExamTerm(examTerm.ExamID);
            MessageBox.Show("ExamTerm confirmed.");
            Update();
        }

        private void SuspendStudent_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent == null)
                MessageBox.Show("Please choose a student to delete!");
            else
            {
                studentController.DeactivateStudentAccount(SelectedStudent.ToStudent());
                Update();
            }
        }
        private void GradeStudent_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent == null)
                MessageBox.Show("Please choose a student to grade!");
            else if (examTermGradeController.IsStudentGraded(SelectedStudent.id, examTerm.ExamID))
                MessageBox.Show("This student is already graded!");
            else
            {
                Domain.Model.Student? student = studentController.GetStudentById(SelectedStudent.id);
                GradeStudentForm gradeStudentForm = new GradeStudentForm(examTerm, teacher, student, mainController);

                gradeStudentForm.Closed += RefreshPage;

                gradeStudentForm.Show();
                gradeStudentForm.Activate();
            }
        }

        private void RefreshPage(object? sender, EventArgs e)
        {
            Update();
        }
    }
}
