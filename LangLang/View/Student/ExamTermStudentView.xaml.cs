using LangLang.Controller;
using LangLang.Domain.Model;
using System.Windows;

namespace LangLang.View.Student
{
    /// <summary>
    /// Interaction logic for ExamTermStudentView.xaml
    /// </summary>
    public partial class ExamTermStudentView : Window
    {

        private readonly ExamTerm examTerm;
        private readonly Domain.Model.Student student;
        private readonly TeacherController teacherController;

        public ExamTermStudentView(ExamTerm examTerm, Domain.Model.Student student, TeacherController teacherController, StudentsController studentController)
        {
            InitializeComponent();
            this.examTerm = examTerm;
            this.teacherController = teacherController;
            this.student = student;

            DataContext = this;

            AddExamTermInfo();

        }
        private void AddExamTermInfo()
        {
            Course course = teacherController.GetCourseById(examTerm.CourseID);
            ExamTermGrade grade = teacherController.GetExamTermGradeByStudentExam(student.Id, examTerm.ExamID);

            examTermLanguageTextBlock.Text = $"{course.Language}";
            examTermLevelTextBlock.Text = $"{course.Level}";
            if (grade != null)
            {
                examTermReadingPointsTextBlock.Text = $"{grade.ReadingPoints}";
                examTermSpeakingPointsTextBlock.Text = $"{grade.SpeakingPoints}";
                examTermWritingPointsTextBlock.Text = $"{grade.WritingPoints}";
                examTermListeningPointsTextBlock.Text = $"{grade.ListeningPoints}";
                examTermGradeTextBlock.Text = $"{grade.Value}";
            }
            else
            {
                examTermReadingPointsTextBlock.Text = "/";
                examTermSpeakingPointsTextBlock.Text = "/";
                examTermWritingPointsTextBlock.Text = "/";
                examTermListeningPointsTextBlock.Text = "/";
                examTermGradeTextBlock.Text = "not graded yet";
            }

            
        }
        private void resultClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}