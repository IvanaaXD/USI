﻿using LangLang.Controller;
using LangLang.DTO;
using LangLang.Domain.Model.Enums;
using LangLang.Observer;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Windows;
namespace LangLang.View.Director
{
    public partial class DirectorPage : Window, IObserver
    {
        public ObservableCollection<TeacherDTO>? Teachers { get; set; }

        public class ViewModel
        {
            public ObservableCollection<TeacherDTO> Teachers { get; set; }

            public ViewModel()
            {
                Teachers = new ObservableCollection<TeacherDTO>();
            }
        }
        readonly int directorId;
        readonly DirectorService directorController;
        readonly MainController mainController;

        public TeacherDTO? SelectedTeacher { get; set; }

        public ViewModel TableViewModel { get; set; }

        private bool isSearchButtonClicked = false;

        public DirectorPage(int directorId, MainController mainController)
        {
            InitializeComponent();
            this.directorId = directorId;
            this.mainController = mainController;
            this.directorController = mainController.GetDirectorController();

            TableViewModel = new ViewModel();
            DataContext = this;
            directorController.Subscribe(this);

            Domain.Model.Director director = directorController.GetDirector();
            firstAndLastName.Text = director.FirstName + " " + director.LastName;

            languageComboBox.ItemsSource = Enum.GetValues(typeof(Language));
            levelComboBox.ItemsSource = Enum.GetValues(typeof(LanguageLevel));

            Update();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.Show();
            this.Close();
        }

        public void Update()
        {
            try
            {
                TableViewModel.Teachers.Clear();
                var teachers = directorController.GetAllTeachers();

                if (teachers != null)
                {
                    foreach (Domain.Model.Teacher teacher in teachers)
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
                List<Domain.Model.Teacher> teachers = GetFilteredTeachers();

                if (teachers != null)
                {
                    foreach (Domain.Model.Teacher teacher in teachers)
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

        private void CreateTeacher_Click(object sender, RoutedEventArgs e)
        {
            CreateTeacherFrom createTeacherFrom = new CreateTeacherFrom(directorController);
            createTeacherFrom.Show();
        }

        private void UpdateTeacher_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTeacher == null)
                MessageBox.Show("Please choose a teacher to update!");
            else
            {
                UpdateTeacherForm updateTeacherForm = new UpdateTeacherForm(SelectedTeacher.Id, directorController);
                updateTeacherForm.Show();
                updateTeacherForm.Activate();
            }
        }

        private void DeleteTeacher_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTeacher == null)
                MessageBox.Show("Please choose a teacher to delete!");
            else
                directorController.Delete(SelectedTeacher.Id);
        }

        private void SearchTeachers_Click(object sender, RoutedEventArgs e)
        {
            UpdateSearch();
            isSearchButtonClicked = true;
        }

        private void ResetTeachers_Click(object sender, EventArgs e)
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

        private List<Domain.Model.Teacher> GetFilteredTeachers()
        {
            Language selectedLanguage = Domain.Model.Enums.Language.NULL;
            LanguageLevel selectedLevel = LanguageLevel.NULL;
            DateTime selectedStartDate = DateTime.MinValue;

            if (languageComboBox.SelectedItem != null)
                selectedLanguage = (Language)languageComboBox.SelectedItem;

            if (levelComboBox.SelectedItem != null)
                selectedLevel = (LanguageLevel)levelComboBox.SelectedItem;

            if (startedWorkDatePicker.SelectedDate.HasValue)
                selectedStartDate = (DateTime)startedWorkDatePicker.SelectedDate;

            return GetDisplayTeachers(selectedLanguage, selectedLevel, selectedStartDate);
        }

        private List<Domain.Model.Teacher> GetDisplayTeachers(Language selectedLanguage, LanguageLevel selectedLevel, DateTime selectedStartDate)
        {
            List<Domain.Model.Teacher> finalTeachers = new();

            if (isSearchButtonClicked)
            {
                List<Domain.Model.Teacher> allFilteredTeachers = directorController.FindTeachersByCriteria(selectedLanguage, selectedLevel, selectedStartDate);

                foreach (Domain.Model.Teacher teacher in allFilteredTeachers)
                {

                    finalTeachers.Add(teacher);
                }
            }
            else
            {
                foreach (Domain.Model.Teacher teacher in directorController.GetAllTeachers())
                {
                    finalTeachers.Add(teacher);
                }
            }
            return finalTeachers;
        }
    }
}
