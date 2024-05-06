using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model;
using LangLang.View.Teacher;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LangLang.View.Student
{
    /// <summary>
    /// Interaction logic for ExamTermStudentView.xaml
    /// </summary>
    public partial class ExamTermStudentView : Window
    {
      
        private readonly ExamTerm examTerm;
        private readonly Model.Student student;
        private readonly TeacherController teacherController;

        public ExamTermStudentView(ExamTerm examTerm,Model.Student student, TeacherController teacherController, StudentsController studentController)
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
            examTermReadingPointsTextBlock.Text = $"{grade.ReadingPoints}";
            examTermSpeakingPointsTextBlock.Text = $"{grade.SpeakingPoints}";
            examTermWritingPointsTextBlock.Text = $"{grade.WritingPoints}";
            examTermListeningPointsTextBlock.Text = $"{grade.ListeningPoints}";
            examTermGradeTextBlock.Text = $"{grade.Value}";
        }
 
        private void resultClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
