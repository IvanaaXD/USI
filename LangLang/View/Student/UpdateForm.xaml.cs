using LangLang.Controller;
using LangLang.DTO;
using LangLang.Domain.Model.Enums;
using System;
using System.Windows;
using System.Windows.Controls;
using LangLang.Domain.Model;

namespace LangLang.View.Student
{
    /// <summary>
    /// Interaction logic for UpdateForm.xaml
    /// </summary>
    public partial class UpdateForm : Window
    {

        public StudentDTO Student { get; set; }

        private readonly StudentsController studentsController;
        private string studentEmail;

        public UpdateForm(int studentId, StudentsController studentsController)
        {
            InitializeComponent();
            this.studentsController = Injector.CreateInstance<StudentsController>();
            Student = new StudentDTO(studentsController.GetStudentById(studentId));
            DataContext = Student;
            studentEmail = Student.Email;

            genderComboBox.ItemsSource = Enum.GetValues(typeof(Gender));
            educationLevelComboBox.ItemsSource = Enum.GetValues(typeof(EducationLevel));
            passwordBox.Password = Student.Password;
        }
        private void btnSaveData_Click(object sender, RoutedEventArgs e)
        {
            if (Student.IsValid)
            {
                if (studentsController.IsEmailUnique(Student.Email) || 
                   (!studentsController.IsEmailUnique(Student.Email) && Student.Email == studentEmail))
                {
                    studentsController.Update(Student.ToStudent());
                    Close();
                }
                else
                {
                    MessageBox.Show("Email already exists.");
                }
            }
            else
            {
                MessageBox.Show("Student can not be created. Not all fields are valid.");
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                Student.Password = passwordBox.Password;
            }
        }
    }
}
