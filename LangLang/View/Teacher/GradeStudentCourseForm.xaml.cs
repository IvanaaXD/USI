using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model;
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

        private StudentDTO _student;
        public StudentDTO Student
        {
            get { return _student; }
            set
            {
                _student = value;
                OnPropertyChanged(nameof(Student));
            }
        }

        private CourseGradeDTO _grade;
        public CourseGradeDTO CourseGrade
        {
            get { return _grade; }
            set
            {
                _grade = value;
                OnPropertyChanged(nameof(CourseGrade));
            }
        }

        private Course course;
        private Model.Teacher teacher;
        private Model.Student student;
        private TeacherController teacherController;
        private StudentsController studentController;

        public GradeStudentCourseForm(Course course, Model.Teacher teacher, Model.Student student, TeacherController teacherController, StudentsController studentController)
        {
            InitializeComponent();
            DataContext = this;
            CourseGrade = new CourseGradeDTO();

            Student = new StudentDTO(student);

            this.course = course;
            this.teacherController = teacherController;
            this.studentController = studentController;
            this.teacher = teacher;
            this.student = student;

            firstNameTextBlock.Text = student.FirstName;
            lastNameTextBlock.Text = student.LastName;
            emailTextBlock.Text = student.Email;

            CourseGrade.Value = 1;
        }

        public void GradeStudent_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(gradeValueTextBox.Text))
            {
                CourseGrade.TeacherId = teacher.Id;
                CourseGrade.CourseId = course.Id;
                CourseGrade.StudentId = student.Id;
                teacherController.GradeStudentCourse(CourseGrade.ToCourseGrade());
            }
            Close();
        }
    }
}
