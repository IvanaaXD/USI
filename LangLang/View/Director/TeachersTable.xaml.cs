using LangLang.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using LangLang.View;
using LangLang.Observer;
using LangLang.DTO;
using LangLang.Controller;

namespace LangLang.View.Director
{
    public partial class TeachersTable : Window, IObserver
    {
        public class ViewModel
        {
            public ObservableCollection<TeacherDTO> Teachers { get; set; }

            public ViewModel()
            {
                Teachers = new ObservableCollection<TeacherDTO>();
            }
        }

        public ViewModel TableViewModel { get; set; }
        public TeacherDTO SelectedTeacher { get; set; }
        public DirectorController directorController { get; set; }

        public TeachersTable()
        {
            InitializeComponent();
            TableViewModel = new ViewModel();
            directorController = new DirectorController();
            DataContext = this;
            directorController.Subscribe(this);
            Update();
        }

        public void Update()
        {
            try
            {
                TableViewModel.Teachers.Clear();
                var teachers = directorController.GetAllTeachers();
                if (teachers != null)
                {
                    foreach (Model.Teacher teacher in teachers)
                        TableViewModel.Teachers.Add(new TeacherDTO(teacher));
                }
                else
                {
                    MessageBox.Show("No teachers found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            RegistrationForm regForm = new RegistrationForm(directorController);

            regForm.Show();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTeacher == null)
            {
                MessageBox.Show("Please choose a teacher to update!");
            }
            else
            {
                directorController.Delete(SelectedTeacher.Id);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTeacher == null)
            {
                MessageBox.Show("Please choose a teacher to delete!");
            }
            else
            {
                directorController.Delete(SelectedTeacher.Id);
            }
        }
    }
}
