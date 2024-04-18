using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using LangLang.View.Director;
using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model;
using LangLang.Observer;
using LangLang.View.Teacher;
using LangLang.View.Student;
using RegistrationForm = LangLang.View.Student.RegistrationForm;
using LangLang.Model.DAO;

namespace LangLang
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IObserver
    {
        public ObservableCollection<StudentDTO> Students { get; set; }
        public ObservableCollection<TeacherDTO> Teachers { get; set; }
        public ObservableCollection<ExamTermDTO> ExamTerms { get; set; }
        public ObservableCollection<CourseDTO> Courses { get; set; }
        public TeacherDTO SelectedTeacher { get; set; }
        public StudentDTO SelectedStuent { get; set; }
        private StudentsController studentController { get; set; }
        private TeacherController teacherController { get; set; }
        private DirectorController directorController { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Students = new ObservableCollection<StudentDTO>();
            Teachers = new ObservableCollection<TeacherDTO>();
            ExamTerms = new ObservableCollection<ExamTermDTO>();
            Courses = new ObservableCollection<CourseDTO>();
            studentController = new StudentsController();
            teacherController = new TeacherController();
            directorController = new DirectorController();
            studentController.Subscribe(this);
            teacherController.Subscribe(this);
            directorController.Subscribe(this);
            Update();
        }

        public void Update()
        {
            Students.Clear();
            foreach (Student student in studentController.GetAllStudents())
                Students.Add(new StudentDTO(student));

            Teachers.Clear();
            foreach (Teacher teacher in directorController.GetAllTeachers())
                Teachers.Add(new TeacherDTO(teacher));

            ExamTerms.Clear();
            foreach (ExamTerm examTerm in teacherController.GetAllExamTerms())
                ExamTerms.Add(new ExamTermDTO(examTerm));

            Courses.Clear();
            foreach (Course course in teacherController.GetAllCourses())
                Courses.Add(new CourseDTO(course));

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = Email.Text; 
            string password = Password.Password; 

            foreach (Teacher teacher in directorController.GetAllTeachers())
            {
                if (teacher.Email == email && teacher.Password == password)
                {
                    CoursesTable coursesTable = new CoursesTable(teacher.Id, directorController);
                    coursesTable.Show();
                    this.Close();
                    return;
                }
            }

            foreach (Student student in studentController.GetAllStudents())
            {
                if (student.Email == email && student.Password == password)
                {
                    LangLang.View.Student.WelcomePage welcomePage = new LangLang.View.Student.WelcomePage(student.Id, studentController);
                    welcomePage.Show();
                    this.Close();
                    return;
                }
            }

            Director director = directorController.GetDirector();

            if (director.Email == email && director.Password == password)
            {
                TeachersTable table = new TeachersTable();
                table.Show();
                this.Close();

                return;
            }
            
            
             MessageBox.Show("User does not exist.");


            } 

   
        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            RegistrationForm registrationForm = new RegistrationForm(studentController);
            registrationForm.Show();
        }
    }
}

