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
using LangLang.Model.Enums;
using System.Windows.Documents;
using LangLang.Model.DAO;

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

        private bool isSearchButtonClicked = false;


        public TeachersTable()
        {
            InitializeComponent();
            TableViewModel = new ViewModel();
            directorController = new DirectorController();
            DataContext = this;
            directorController.Subscribe(this);
            languageComboBox.ItemsSource = Enum.GetValues(typeof(Language));
            levelComboBox.ItemsSource = Enum.GetValues(typeof(LanguageLevel));
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

        public void UpdateSearch()
        {
            try
            {
                TableViewModel.Teachers.Clear();
                List<Model.Teacher> teachers = GetFilteredTeachers();

                if (teachers != null)
                {
                    foreach (Model.Teacher teacher in teachers)
                        TableViewModel.Teachers.Add(new TeacherDTO(teacher));
                }
                else
                {
                    MessageBox.Show("No courses found.");
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
                ModifyDataForm modifyDataForm = new ModifyDataForm(SelectedTeacher.Id, directorController);
                modifyDataForm.Show();
                modifyDataForm.Activate();
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

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            UpdateSearch();
            isSearchButtonClicked = true;
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            isSearchButtonClicked = false;
            Update();
            ResetSearchElements();
        }

        private void ResetSearchElements()
        {
            languageComboBox.SelectedItem = null;
            levelComboBox.SelectedItem = null;
            startedWorkDatePicker.SelectedDate = null;
        }

        private List<Model.Teacher> GetFilteredTeachers()
        {
            Language selectedLanguage = Model.Enums.Language.NULL;
            LanguageLevel selectedLevel = Model.Enums.LanguageLevel.NULL;
            DateTime selectedStartDate = DateTime.MinValue;

            if (languageComboBox.SelectedItem != null)
            {
                selectedLanguage = (Language)languageComboBox.SelectedItem; 
            }

            if (levelComboBox.SelectedItem != null)
            {
                selectedLevel = (LanguageLevel)levelComboBox.SelectedItem;
            }

            if (startedWorkDatePicker.SelectedDate.HasValue)
            {
                selectedStartDate = (DateTime)startedWorkDatePicker.SelectedDate;
            }

            List<Model.Teacher> finalTeachers= new List<Model.Teacher>();

            if (isSearchButtonClicked)
            {

                List<Model.Teacher> allFilteredTeachers = directorController.FindTeachersByCriteria(selectedLanguage, selectedLevel, selectedStartDate);

                foreach (Model.Teacher teacher in allFilteredTeachers)
                {

                    finalTeachers.Add(teacher);
                }
            }
            else
            {
                foreach (Model.Teacher teacher in directorController.GetAllTeachers())
                {
                    finalTeachers.Add(teacher);
                }
            }
            return finalTeachers;
        }
    }
}
