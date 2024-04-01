using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model.Enums;
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

namespace LangLang.View.Student
{
    /// <summary>
    /// Interaction logic for ModifyDataForm.xaml
    /// </summary>
    public partial class ModifyDataForm : Window
    {
        public Gender[] genderValues => (Gender[])Enum.GetValues(typeof(Gender));
        public EducationLevel[] educationLevelValues => (EducationLevel[])Enum.GetValues(typeof(EducationLevel));

        public StudentDTO Student { get; set; }

        private readonly StudentsController studentsController;

        public ModifyDataForm(int studentId, StudentsController studentsController)
        {
            InitializeComponent();
            this.studentsController = studentsController;
            Student = new StudentDTO(studentsController.GetStudentById(studentId));
            DataContext = Student;

            passwordBox.Password = Student.Password;
        }
        private void btnSaveData_Click(object sender, RoutedEventArgs e)
        {
            if (Student.IsValid)
            {
                studentsController.Update(Student.ToStudent());
                Close();
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
