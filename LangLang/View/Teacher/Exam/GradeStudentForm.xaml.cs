using LangLang.Controller;
using LangLang.DTO;
using LangLang.Domain.Model;
using System.ComponentModel;
using System.Windows;

namespace LangLang.View.Teacher
{
    public partial class GradeStudentForm : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private StudentDTO? _student;
        public StudentDTO? Student
        {
            get { return _student; }
            set
            {
                _student = value;
                OnPropertyChanged(nameof(Student));
            }
        }

        private ExamTermGradeDTO? _grade;
        public ExamTermGradeDTO? Grade
        {
            get { return _grade; }
            set
            {
                _grade = value;
                OnPropertyChanged(nameof(Grade));
            }
        }

        private ExamTerm examTerm;
        private Domain.Model.Teacher? teacher;
        private Domain.Model.Student student;
        private TeacherController teacherController;
        private StudentsController studentController;

        public GradeStudentForm(ExamTerm examTerm, Domain.Model.Teacher? teacher, Domain.Model.Student? student, TeacherController teacherController, StudentsController studentController)
        {
            InitializeComponent();
            DataContext = this;
            Grade = new ExamTermGradeDTO();

            Student = new StudentDTO(student);

            this.examTerm = examTerm;
            this.teacherController = teacherController;
            this.studentController = studentController;
            this.teacher = teacher;
            this.student = student;

            firstNameTextBlock.Text = student.FirstName;
            lastNameTextBlock.Text = student.LastName;
            emailTextBlock.Text = student.Email;
        }

        public void GradeStudent_Click(object sender, RoutedEventArgs e)
        {
            if (Grade != null)
            {
                Grade.Value = 5;

                if (HasPassedExamTerm())
                {
                    int sum = Grade.ListeningPoints + Grade.SpeakingPoints + Grade.WritingPoints + Grade.ReadingPoints;

                    switch (sum)
                    {
                        case int p when p >= 200:
                            Grade.Value = 10;
                            break;
                        case int p when p >= 190:
                            Grade.Value = 9;
                            break;
                        case int p when p >= 180:
                            Grade.Value = 8;
                            break;
                        case int p when p >= 170:
                            Grade.Value = 7;
                            break;
                        default:
                            Grade.Value = 6;
                            break;
                    }
                }
            }
        }

        public void Done_Click(object sender, RoutedEventArgs e)
        {
            if (teacher != null && Grade != null && !valueTextBlock.Equals('-'))
            {
                Grade.TeacherId = teacher.Id;
                Grade.ExamId = examTerm.ExamID;
                Grade.StudentId = student.Id;
                teacherController.GradeStudent(Grade.ToGrade());
            }
            Close();
        }

        private bool HasPassedExamTerm()
        {
            bool passedListening = Grade?.ListeningPoints >= 0.5 * 40;
            bool passedSpeaking = Grade?.SpeakingPoints >= 0.5 * 50;
            bool passedWriting = Grade?.WritingPoints >= 0.5 * 60;
            bool passedReading = Grade?.ReadingPoints >= 0.5 * 60;
            bool sum = Grade?.ListeningPoints + Grade?.SpeakingPoints + Grade?.WritingPoints + Grade?.ReadingPoints >= 160;

            if (passedListening && passedSpeaking && passedWriting && passedReading && sum)
            {
                return true;
            }
            return false;
        }
    }
}
