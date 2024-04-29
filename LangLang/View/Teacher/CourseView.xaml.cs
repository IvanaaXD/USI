using LangLang.Model;
using LangLang.Controller;
using LangLang.DTO;
using LangLang.Observer;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;

namespace LangLang.View.Teacher
{
    public partial class CourseView : Window, IObserver
    {
        public ObservableCollection<MailDTO> Mails { get; set; }
        public ObservableCollection<StudentDTO> Students { get; set; }
        public class ViewModel
        {
            public ObservableCollection<MailDTO> Mails { get; set; }
            public ObservableCollection<StudentDTO> Students { get; set; }

            public ViewModel()
            {
                Mails = new ObservableCollection<MailDTO>();
                Students = new ObservableCollection<StudentDTO>();
            }
        }
        
        public ViewModel MailsTableViewModel { get; set; }
        public ViewModel StudentsTableViewModel { get; set; }
        public MailDTO SelectedMail { get; set; }

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

            MailsTableViewModel = new ViewModel();
            StudentsTableViewModel = new ViewModel();

            DataContext = this;

            teacherController.Subscribe(this);

            if (course.StartDate.Date <= DateTime.Now.AddDays(-7))
            {
                PenaltyPoint.Visibility = Visibility.Collapsed;
            }

            Update();
        }

        public void Update()
        {
            try
            {
                MailsTableViewModel.Mails.Clear();
                var mails = teacherController.GetAllMails();

                if (mails != null)
                {
                    foreach (Mail mail in mails)
                        MailsTableViewModel.Mails.Add(new MailDTO(mail));
                }
                else
                {
                    MessageBox.Show("No teachers found.");
                }

                StudentsTableViewModel.Students.Clear();
                var students = studentController.GetAllStudentsForCourse(course.Id);

                if (students != null)
                {
                    foreach (Model.Student student in students)
                        StudentsTableViewModel.Students.Add(new StudentDTO(student));
                }
                else
                {
                    MessageBox.Show("No courses found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void PenaltyPoint_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReadMail_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AnswerMail_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
