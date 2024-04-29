using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using LangLang.Model.Enums;
using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model.DAO;

namespace LangLang.View.Student
{
    public partial class RegistrationForm : Window
    {
        public Gender[] genderValues => (Gender[])Enum.GetValues(typeof(Gender));
        public EducationLevel[] educationLevelValues => (EducationLevel[])Enum.GetValues(typeof(EducationLevel));

        public StudentDTO Student { get; set; }

        private StudentsController studentsController;

        public RegistrationForm(StudentsController studentsController)
        {
            InitializeComponent();
            Student = new StudentDTO();
            Student.Password = passwordBox.Password;
            this.studentsController = studentsController;
            DataContext = this;

        }

        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            if (Student.IsValid)
            {
                if (studentsController.IsEmailUnique(Student.Email))
                {
                    studentsController.Add(Student.ToStudent());
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
