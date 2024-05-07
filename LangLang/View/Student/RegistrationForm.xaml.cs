using System;
using System.Windows;
using System.Windows.Controls;
using LangLang.Model.Enums;
using LangLang.Controller;
using LangLang.DTO;

namespace LangLang.View.Student
{
    public partial class RegistrationForm : Window
    {
        public Gender[] genderValues => (Gender[])Enum.GetValues(typeof(Gender));
        public EducationLevel[] educationLevelValues => (EducationLevel[])Enum.GetValues(typeof(EducationLevel));

        public StudentDTO student { get; set; }

        private StudentsController studentsController;

        public RegistrationForm(StudentsController studentsController)
        {
            InitializeComponent();
            student = new StudentDTO();
            student.Password = passwordBox.Password;
            this.studentsController = studentsController;
            DataContext = this;

            SetPlaceholders();
        }

        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            if (student.IsValid)
            {
                if (studentsController.IsEmailUnique(student.Email))
                {
                    studentsController.Add(student.ToStudent());
                    Close();
                }
                else
                {
                    MessageBox.Show("Email already exists.");
                }
            }
            else
            {
                MessageBox.Show("student can not be created. Not all fields are valid.");
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
                student.Password = passwordBox.Password;
            }
        }
        private void SetPlaceholders()
        {
            student.FirstName = "Name";
            student.LastName = "Surname";
            student.Email = "example@gmail.com";
            student.PhoneNumber = "0123456789";
            student.Password = "password12";
            passwordBox.Password = student.Password;

            firstNameTextBox.GotFocus += FirstNameTextBox_GotFocus;
            lastNameTextBox.GotFocus += LastNameTextBox_GotFocus;
            emailTextBox.GotFocus += EmailTextBox_GotFocus;
            phoneNumberTextBox.GotFocus += PhoneNumberTextBox_GotFocus;
            passwordBox.GotFocus += PasswordBox_GotFocus;
        }

        private void FirstNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            firstNameTextBox.Text = string.Empty;
        }

        private void LastNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            lastNameTextBox.Text = string.Empty;
        }

        private void EmailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            emailTextBox.Text = string.Empty;
        }

        private void PhoneNumberTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            phoneNumberTextBox.Text = string.Empty;
        }
        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            passwordBox.Password = "";
        }
    }
}
