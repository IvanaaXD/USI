using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model.Enums;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LangLang.View.Director
{
    /// <summary>
    /// Interaction logic for ModifyDataForm.xaml
    /// </summary>
    public partial class UpdateTeacherForm : Window
    {

        public TeacherDTO Teacher { get; set; }

        private DirectorController directorController;
        private string teacherEmail;

        public UpdateTeacherForm(int teacherId, DirectorController directorController)
        {
            InitializeComponent();
            this.directorController = directorController;
            Teacher = new TeacherDTO(directorController.GetTeacherById(teacherId));
            DataContext = Teacher;
            teacherEmail = Teacher.Email;

            genderComboBox.ItemsSource = Enum.GetValues(typeof(Gender));
            genderComboBox.SelectedItem = Teacher.Gender;
            passwordBox.Password = Teacher.Password;
            
            languagesListBox.ItemsSource = Teacher.LevelAndLanguages;

            for (int i = 0; i < Teacher.LevelOfLanguages.Count; i++)
            {
                for (int j = 0; j < Teacher.Languages.Count; j++)
                {
                    if (i == j)
                    {
                        languagesListBox.SelectedItems.Add($"{Teacher.Languages[j]} {Teacher.LevelOfLanguages[i]}");
                    }
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Teacher.IsValid)
            {
                directorController.Update(Teacher.ToTeacher());
                Close();
            }
            else
            {
                MessageBox.Show("Teacher can not be updated. Not all fields are valid.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                Teacher.Password = passwordBox.Password;
            }
        }

        private void LanguagesListBox_SelectionChanged(object sender, RoutedEventArgs e) { }
    }
}
