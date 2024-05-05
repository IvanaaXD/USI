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
    }
}
