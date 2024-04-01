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
using LangLang.Model.Enums;
using LangLang.Controller;
using LangLang.DTO;
using System.ComponentModel;
using System.Globalization;
using System.Reflection.Emit;
using System.Xml.Serialization;

namespace LangLang.View.Teacher
{
    public partial class CreateCourseForm : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Language[] languageValues => (Language[])Enum.GetValues(typeof(Language));
        public LanguageLevel[] languageLevelValues => (LanguageLevel[])Enum.GetValues(typeof(LanguageLevel));

        private CourseDTO _course;

        public CourseDTO Course
        {
            get { return _course; }
            set
            {
                _course = value;
                OnPropertyChanged(nameof(Course));
            }
        }

        private TeacherController teacherController;
        private int teacherId;

        public CreateCourseForm(TeacherController teacherController, int teacherId)
        {
            InitializeComponent();

            DataContext = this;
            Course = new CourseDTO();
            this.teacherController = teacherController;
            this.teacherId = teacherId;
            Course.StartDate = DateTime.Now;
            Course.StartTime = "00:00";
        }

        private void PickLanguageAndLevel()
        {
            if (languageComboBox.SelectedItem != null)
            {
                string selectedLanguageAndLevel = (string)languageComboBox.SelectedItem;

                string[] parts = selectedLanguageAndLevel.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2)
                {
                    if (Enum.TryParse(parts[0], out Language language))
                    {
                        Course.Language = language;
                    }
                    else
                    {
                        MessageBox.Show($"Invalid language: {parts[0]}");
                    }

                    if (Enum.TryParse(parts[1], out LanguageLevel level))
                    {
                        Course.Level = level;
                    }
                    else
                    {
                        MessageBox.Show($"Invalid level: {parts[1]}");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid language and level format.");
                }
            }
        }


        private void PickDataFromComboBox()
        {
            if (isOnlineComboBox.SelectedItem != null)
            {
                string selectedOption = ((ComboBoxItem)isOnlineComboBox.SelectedItem).Content.ToString();

                Course.IsOnline = selectedOption == "Online";
            }
        }

        private void PickDataFromDatePicker()
        {
            if (startDatePicker.SelectedDate.HasValue && !string.IsNullOrWhiteSpace(startTimeTextBox.Text))
            {
                DateTime startDate = startDatePicker.SelectedDate.Value.Date;
                DateTime startTime;
                if (DateTime.TryParseExact(startTimeTextBox.Text, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
                {
                    Course.StartDate = startDate.Add(startTime.TimeOfDay);
                }
                else
                {
                    MessageBox.Show("Please enter a valid start time (HH:mm).");
                }
            }
            else
            {
                MessageBox.Show("Please select a valid start date and time.");
            }
        }

        private void PickDataFromListBox()
        {
            Course.WorkDays = new List<DayOfWeek>();
            foreach (var selectedItem in dayListBox.SelectedItems)
            {
                if (Enum.TryParse(selectedItem.ToString(), out DayOfWeek day))
                {
                    Course.WorkDays.Add(day);
                }
            }
        }
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            PickDataFromComboBox();
            PickDataFromDatePicker();
            PickLanguageAndLevel();
            PickDataFromListBox();

            if (Course.IsValid)
            {
                teacherController.AddCourse(Course.ToCourse());
                Close();
            }
            else
            {
                MessageBox.Show("Course cannot be created. Not all fields are valid.");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
