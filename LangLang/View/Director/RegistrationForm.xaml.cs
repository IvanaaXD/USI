using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using LangLang.Model.Enums;
using LangLang.Controller;
using LangLang.DTO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LangLang.View.Director
{
    public partial class RegistrationForm : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Gender[] genderValues => (Gender[])Enum.GetValues(typeof(Gender));

        private TeacherDTO _teacher;

        public TeacherDTO Teacher
        {
            get { return _teacher; }
            set
            {
                _teacher = value;
                OnPropertyChanged(nameof(Teacher));
            }
        }

        private DirectorController directorController;

        public RegistrationForm(DirectorController directorController)
        {
            InitializeComponent();
            DataContext = this;
            Teacher = new TeacherDTO();
            this.directorController = directorController;
            Teacher.DateOfBirth = new DateTime(1900, 1, 1);
            Teacher.StartedWork = new DateTime(1900, 1, 1);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private Regex _passwordRegex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{5,}$");

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;

            string password = passwordBox.Password;

            if (!_passwordRegex.IsMatch(password))
            {
                passwordBox.ToolTip = "Invalid password format";
            }
            else
            {
                passwordBox.ToolTip = null;
            }
        }


        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            PickDataFromListBox();
            PickDataFromDatePicker();
            PickDataFromComboBox();

            if (Teacher.IsValid)
            {
                directorController.Add(Teacher.ToTeacher());
                Close();
            }
            else
            {

                MessageBox.Show("Teacher cannot be created. Not all fields are valid.");
            }
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
                    {
                        Teacher.Languages.Add(lan);
                    }

                    if (Enum.TryParse(parts[1], out LanguageLevel level))
                    {
                        Teacher.LevelOfLanguages.Add(level);
                    }
                }
            }
        }

        private void PickDataFromDatePicker()
        {
            if (dateOfBirthDatePicker.SelectedDate.HasValue)
            {
                Teacher.DateOfBirth = dateOfBirthDatePicker.SelectedDate.Value;
            }
            else
            {
                MessageBox.Show("Please select a valid date of birth.");
            }

            if (startedWorkDatePicker.SelectedDate.HasValue)
            {
                Teacher.StartedWork = startedWorkDatePicker.SelectedDate.Value;
            }
            else
            {
                MessageBox.Show("Please select a valid start date of work.");
            }
        }

        private void PickDataFromComboBox()
        {
            if (genderComboBox.SelectedItem != null)
            {
                if (genderComboBox.SelectedItem is Gender selectedGender)
                {
                    Teacher.Gender = selectedGender;
                }
            }
        }


        private void languagesListBox_SelectionChanged(object sender, RoutedEventArgs e) { }

    }
}
