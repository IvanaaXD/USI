using LangLang.Controller;
using LangLang.Domain.Model;
using LangLang.DTO;
using System;
using System.ComponentModel;
using System.Windows;

namespace LangLang.View.Teacher
{
    public partial class GradeStudentCourseForm : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private CourseGradeDTO _grade;
        public CourseGradeDTO StudentCourseGrade
        {
            get { return _grade; }
            set
            {
                _grade = value;
                OnPropertyChanged(nameof(StudentCourseGrade));
            }
        }

        private Domain.Model.Course course;
        private Domain.Model.Teacher teacher;
        private Domain.Model.Student student;
        private TeacherController teacherController;
        private StudentsController studentController;
        private CourseGradeController courseGradeController;
        private MailController mailController;

        public GradeStudentCourseForm(Domain.Model.Course course, Domain.Model.Teacher teacher, Domain.Model.Student student)
        {
            InitializeComponent();
            DataContext = this;
            StudentCourseGrade = new CourseGradeDTO();

            this.course = course;
            teacherController = Injector.CreateInstance<TeacherController>();
            studentController = Injector.CreateInstance<StudentsController>();
            mailController = Injector.CreateInstance<MailController>();
            courseGradeController = Injector.CreateInstance<CourseGradeController>();

            this.teacher = teacher;
            this.student = student;

            SetFormInfo();
        }
        public void SetFormInfo()
        {
            firstNameTextBlock.Text = student.FirstName;
            lastNameTextBlock.Text = student.LastName;
            emailTextBlock.Text = student.Email;

            StudentCourseGrade.StudentActivityValue = 1;
            StudentCourseGrade.StudentKnowledgeValue = 1;
            StudentCourseGrade.TeacherId = teacher.Id;
            StudentCourseGrade.StudentId = student.Id;
            StudentCourseGrade.CourseId = course.Id;
        }

        public void GradeStudent_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(activityValueTextBox.Text) && !string.IsNullOrWhiteSpace(knowledgeValueTextBox.Text))
            {
                string messageBody = "Your final grade from course " + course.Language.ToString() + " " + course.Level.ToString() + " is " + StudentCourseGrade.StudentActivityValue.ToString() +
                " for your activity on course, and " + StudentCourseGrade.StudentKnowledgeValue.ToString() + " for knowledge shown during course.";

                mailController.ConstructMail(teacher, student, course, Domain.Model.Enums.TypeOfMessage.TeacherGradeStudentMessage, messageBody);
                courseGradeController.AddGrade(StudentCourseGrade.ToCourseGrade());
                studentController.CompleteCourse(student, course);

                Close();
            }
        }
    }
}
