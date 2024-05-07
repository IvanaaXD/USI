using System.Windows;
using System.Windows.Input;
using LangLang.View.Director;
using LangLang.Controller;
using LangLang.Model;
using LangLang.View.Teacher;
using LangLang.View.Student;

namespace LangLang
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StudentsController studentController { get; set; }
        private DirectorController directorController { get; set; }
        private MainController mainController { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            mainController = new MainController();
            directorController = mainController.GetDirectorController();
            studentController = mainController.GetStudentController();

            SetPlaceholders();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = Email.Text; 
            string password = Password.Password;

            if (HasStudentLoggedIn(email, password) || HasTeacherLoggedIn(email, password) || HasDirectorLoggedIn(email, password))
            {
                this.Close();
                return;
            }
            
             MessageBox.Show("User does not exist.");
        } 

        private bool HasStudentLoggedIn(string email, string password)
        {
            foreach (Student student in studentController.GetAllStudents())
            {
                if (student.Email == email && student.Password == password)
                {
                    if (student.ActiveCourseId != -10)
                    {
                        StudentForm welcomePage = new StudentForm(student.Id, studentController);
                        welcomePage.Show();
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Your account has been deactivated.");
                        return false;
                    }
                }
            }
            return false;
        }

        private bool HasTeacherLoggedIn(string email, string password)
        {
            foreach (Teacher teacher in directorController.GetAllTeachers())
            {
                if (teacher.Email == email && teacher.Password == password)
                {
                    TeacherPage teacherPage = new TeacherPage(teacher.Id, mainController);
                    teacherPage.Show();
                    return true;
                }
            }
            return false;
        }

        private bool HasDirectorLoggedIn(string email, string password)
        {
            Director director = directorController.GetDirector();

            if (director.Email == email && director.Password == password)
            {
                DirectorPage directorPage = new DirectorPage(director.Id, directorController);
                directorPage.Show();
                return true;
            }
            return false;
        }

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            RegistrationForm registrationForm = new RegistrationForm(studentController);
            registrationForm.Show();
        }

        private void SetPlaceholders()
        {
            EmailPlaceholder.Visibility = Visibility.Visible;
            PasswordPlaceholder.Visibility = Visibility.Visible;

            Email.GotFocus += EmailTextBox_GotFocus;
            Password.GotFocus += PasswordBox_GotFocus;

            Email.LostFocus += EmailTextBox_LostFocus;
            Password.LostFocus += PasswordBox_LostFocus;

            EmailPlaceholder.MouseDown += Placeholder_MouseDown;
            PasswordPlaceholder.MouseDown += Placeholder_MouseDown;
        }

        private void EmailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            EmailPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void EmailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Email.Text))
                EmailPlaceholder.Visibility = Visibility.Visible;
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Password.Password))
                PasswordPlaceholder.Visibility = Visibility.Visible;
        }

        private void Placeholder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == EmailPlaceholder)
            {
                EmailPlaceholder.Visibility = Visibility.Collapsed;
                Email.Focus();
            }
            else if (sender == PasswordPlaceholder)
            {
                PasswordPlaceholder.Visibility = Visibility.Collapsed;
                Password.Focus();
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordPlaceholder != null)
                PasswordPlaceholder.Visibility = string.IsNullOrEmpty(Password.Password) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}

