using LangLang.Controller;
using LangLang.Observer;
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

using LangLang.Model;
using LangLang.View.Converters;

namespace LangLang.View.Student
{
    /// <summary>
    /// Interaction logic for StudentForm.xaml
    /// </summary>
    public partial class StudentForm : Window
    {
        int studentId;
        StudentsController studentController;
        public StudentForm(int studentId, StudentsController studentController)
        {
            InitializeComponent();
            this.studentId = studentId;
            this.studentController = studentController;
        }

        private void AvailableCourses_Click(object sender, RoutedEventArgs e)
        {
            AvailableCoursesTable availableCoursesTable = new AvailableCoursesTable(studentId);
            availableCoursesTable.Show();
        }
        private void RegisteredCourses_Click(object sender, RoutedEventArgs e)
        {
            RegisteredCoursesTable registeredCoursesTable = new RegisteredCoursesTable(studentId);
            registeredCoursesTable.Show();
        }
        private void AvailableExamTerms_Click(object sender, RoutedEventArgs e)
        {
            AvailableExamTermsTable availableExamTermsForm = new AvailableExamTermsTable(studentId);
            availableExamTermsForm.Show();
        }

        private void UpdateAccount_Click(object sender, RoutedEventArgs e)
        {
            LangLang.Model.Student student = studentController.GetStudentById(studentId);
            if (student.ActiveCourseId != -1 && student.RegisteredExamsIds != null)
            {
                MessageBox.Show("The student attends the course and cannot change the data.");
            }
            else
            {
                UpdateForm updateDataForm = new UpdateForm(studentId, studentController);
                updateDataForm.Show();
                updateDataForm.Activate();
            }
        }
        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            studentController.Delete(studentId);
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Close();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
