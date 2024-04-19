using LangLang.Controller;
using System;
using System.Windows;

namespace LangLang.View.Teacher
{
    public partial class TeacherPage : Window
    {
        int teacherId;
        DirectorController directorController;
        TeacherController teacherController;
        public TeacherPage(int teacherId, TeacherController teacherController, DirectorController directorController)
        {
            InitializeComponent();
            this.teacherId = teacherId;
            this.teacherController = teacherController;
            this.directorController = directorController;
        }

        private void Courses_Click(object sender, RoutedEventArgs e)
        {
            CoursesTable coursesTable = new CoursesTable(teacherId, teacherController,directorController);
            coursesTable.Show();
        }
        private void ExamTerms_Click(object sender, RoutedEventArgs e)
        {
            ExamTermsTable examTermsTable = new ExamTermsTable(teacherId, directorController);
            examTermsTable.Show();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Close();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

    }
}
