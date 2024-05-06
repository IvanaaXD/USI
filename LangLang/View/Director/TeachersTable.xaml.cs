using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using LangLang.Observer;
using LangLang.DTO;
using LangLang.Controller;
using LangLang.Model.Enums;

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

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            CreateTeacherFrom createTeacherFrom = new CreateTeacherFrom(directorController);
            createTeacherFrom.Show();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTeacher == null)
            {
                MessageBox.Show("Please choose a teacher to update!");
            }
            else
            {
                UpdateTeacherForm updateTeacherForm = new UpdateTeacherForm(SelectedTeacher.Id, directorController);
                updateTeacherForm.Show();
                updateTeacherForm.Activate();
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
            LanguageLevel selectedLevel = LanguageLevel.NULL;
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

            return GetDisplayTeachers(selectedLanguage, selectedLevel, selectedStartDate);
        }

        private List<Model.Teacher> GetDisplayTeachers(Language selectedLanguage, LanguageLevel selectedLevel, DateTime selectedStartDate)
        {
            List<Model.Teacher> finalTeachers = new List<Model.Teacher>();

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
