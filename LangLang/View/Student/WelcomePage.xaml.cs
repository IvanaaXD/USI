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
    /// Interaction logic for WelcomePage.xaml
    /// </summary>
    public partial class WelcomePage : Window
    {
        int studentId;
        StudentsController studentController;
        public WelcomePage(int studentId, StudentsController studentController)
        {
            InitializeComponent();
            this.studentId = studentId;
            this.studentController = studentController;
        }

        private void AvailableCourses_Click(object sender, RoutedEventArgs e)
        {
            AvailableCoursesForm availableCoursesForm = new AvailableCoursesForm(studentId);
            availableCoursesForm.Show();
        }
       
        private void ModifyAccount_Click(object sender, RoutedEventArgs e)
        {
            LangLang.Model.Student student = studentController.GetStudentById(studentId);
            if (student.ActiveCourseId != -1)
            {
                MessageBox.Show("The student attends the course and cannot change the data.");
            }
            else
            {
                ModifyDataForm modifyDataForm = new ModifyDataForm(studentId, studentController);
                modifyDataForm.Show();
                modifyDataForm.Activate();
            }
        }
        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            studentController.Delete(studentId);
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
