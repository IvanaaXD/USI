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
        public CourseGradeDTO StudentCourseGrade
        {
            get { return _grade; }
            set
            {
                _grade = value;
                OnPropertyChanged(nameof(StudentCourseGrade));
            }
        }

        private Model.Course course;
        private Model.Teacher teacher;
        private Model.Student student;
        private TeacherController teacherController;
        private StudentsController studentController;

        public GradeStudentCourseForm(Model.Course course, Model.Teacher teacher, Model.Student student, TeacherController teacherController, StudentsController studentController)
        {
            InitializeComponent();
            DataContext = this;
            StudentCourseGrade = new CourseGradeDTO();

            Student = new StudentDTO(student);

            this.course = course;
            this.teacherController = teacherController;
            this.studentController = studentController;
            this.teacher = teacher;
            this.student = student;

            firstNameTextBlock.Text = student.FirstName;
            lastNameTextBlock.Text = student.LastName;
            emailTextBlock.Text = student.Email;

            StudentCourseGrade.StudentActivityValue = 1;
            StudentCourseGrade.StudentKnowledgeValue = 1;
        }
        public void SendGradeMail()
        {
            Model.Mail mail = new Model.Mail();
            mail.Sender = teacher.Email;
            mail.Receiver = student.Email;
            mail.TypeOfMessage = Model.Enums.TypeOfMessage.TeacherGradeStudentMessage;
            mail.DateOfMessage = DateTime.Now;
            mail.CourseId = course.Id;
            mail.Message = "Your final grade from course " + course.Language.ToString() + " " + course.Level.ToString() + " is " + activityValueTextBox.Text +
                " for your activity on course, and " + knowledgeValueTextBox.Text + " for knowledge shown during course.";
            mail.Answered = false;

            teacherController.SendMail(mail);

            student.ActiveCourseId = -1;
            student.CompletedCoursesIds.Add(course.Id);

            studentController.Update(student);
        }

        public void GradeStudent_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(activityValueTextBox.Text) && !string.IsNullOrWhiteSpace(knowledgeValueTextBox.Text))
            {
                StudentCourseGrade.TeacherId = teacher.Id;
                StudentCourseGrade.CourseId = course.Id;
                StudentCourseGrade.StudentId = student.Id;
                // BUG - to be fixed
                //teacherController.GradeStudentCourse(StudentCourseGrade.ToCourseGrade());

                SendGradeMail();
            }
            Close();
        }
    }
}
