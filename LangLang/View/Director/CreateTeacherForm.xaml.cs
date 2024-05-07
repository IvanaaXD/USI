using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using LangLang.Model.Enums;
using LangLang.Controller;
using LangLang.DTO;
using System.Collections.Generic;
using LangLang.Model;
using System.Windows.Input;

namespace LangLang.View.Director
{
    public partial class CreateTeacherFrom : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Gender[] GenderValues => (Gender[])Enum.GetValues(typeof(Gender));

        private TeacherDTO? _teacher;

        public TeacherDTO Teacher
        {
            get { return _teacher; }
            set
            {
                _teacher = value;
                OnPropertyChanged(nameof(Teacher));
            }
        }

        private readonly DirectorController? directorController;

        public CreateTeacherFrom(DirectorController directorController)
        {
            InitializeComponent();
            Teacher = new TeacherDTO();
            DataContext = this;
            Teacher.Password = passwordBox.Password;
            Teacher.DateOfBirth = new DateTime(DateTime.Today.AddYears(-64).Year, 1, 1);
            Teacher.StartedWork = new DateTime(DateTime.Today.AddYears(-64+18).Year, 1, 1);
            this.directorController = directorController;

            SetPlaceholders();
        }

        private void SetPlaceholders()
        {
            Teacher.FirstName = "Name";
            Teacher.LastName = "Surname";
            Teacher.Email = "example@gmail.com";
            Teacher.PhoneNumber = "0123456789";
            Teacher.Password = "password";
            passwordBox.Password = "password";

            firstNameTextBox.GotFocus += FirstNameTextBox_GotFocus;
            lastNameTextBox.GotFocus += LastNameTextBox_GotFocus;
            emailTextBox.GotFocus += EmailTextBox_GotFocus;
            phoneNumberTextBox.GotFocus += PhoneNumberTextBox_GotFocus;
            passwordBox.GotFocus += PasswordBox_GotFocus;
        }

        private void FirstNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            firstNameTextBox.Text = string.Empty;
        }

        private void LastNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            lastNameTextBox.Text = string.Empty;
        }

        private void EmailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            emailTextBox.Text = string.Empty;
        }

        private void PhoneNumberTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            phoneNumberTextBox.Text = string.Empty;
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            passwordBox.Password = "";
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
                Teacher.Password = passwordBox.Password;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            PickDataFromListBox();
            PickDataFromDatePicker();
            PickDataFromComboBox();

            if (Teacher.IsValid)
            {
                directorController?.Add(Teacher.ToTeacher());
                Close();
            }
            else
                MessageBox.Show("Teacher cannot be created. Not all fields are valid.");
        }

        private void PickDataFromListBox()
        {
            Teacher.Languages = new List<Language>();
            Teacher.LevelOfLanguages = new List<LanguageLevel>();

            foreach (var selectedItem in languagesListBox.SelectedItems)
            {
                string[] parts = selectedItem.ToString().Split(' ');
                if (parts.Length == 2)
                {
                    if (Enum.TryParse(parts[0], out Language lan))
                        Teacher.Languages.Add(lan);

                    if (Enum.TryParse(parts[1], out LanguageLevel level))
                        Teacher.LevelOfLanguages.Add(level);
                }
            }
        }

        private void PickDataFromDatePicker()
        {
            if (dateOfBirthDatePicker.SelectedDate.HasValue)
                Teacher.DateOfBirth = dateOfBirthDatePicker.SelectedDate.Value;
            else
                MessageBox.Show("Please select a valid date of birth.");

            if (startedWorkDatePicker.SelectedDate.HasValue)
                Teacher.StartedWork = startedWorkDatePicker.SelectedDate.Value;
            else
                MessageBox.Show("Please select a valid start date of work.");
        }

        private void PickDataFromComboBox()
        {
            if (genderComboBox.SelectedItem != null)
            {
                if (genderComboBox.SelectedItem is Gender selectedGender)
                    Teacher.Gender = selectedGender;
            }
        }

        private void LanguagesListBox_SelectionChanged(object sender, RoutedEventArgs e) {}

    }
}