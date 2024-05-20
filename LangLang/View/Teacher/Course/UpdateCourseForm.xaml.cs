using LangLang.Controller;
using LangLang.DTO;
using LangLang.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace LangLang.View.Teacher
{
    public partial class UpdateCourseForm : Window
    {
        public Language[] languageValues => (Language[])Enum.GetValues(typeof(Language));
        public LanguageLevel[] languageLevelValues => (LanguageLevel[])Enum.GetValues(typeof(LanguageLevel));
        public CourseDTO Course { get; set; }

        private readonly TeacherController teacherController;
        private readonly CourseController courseController;
        public TeacherDTO Teacher { get; set; }

        public UpdateCourseForm(int courseId, int teacherId, MainController mainController)
        {
            teacherController = mainController.GetTeacherController();
            courseController = mainController.GetCourseController();
            Course = new CourseDTO(courseController, teacherController.GetCourseById(courseId), mainController.GetDirectorController().GetTeacherById(teacherId));
            Teacher = new TeacherDTO(mainController.GetDirectorController().GetTeacherById(teacherId));
            DataContext = Course;

            Course.StartTime = Course.StartDate.ToString("HH:mm");

            InitializeComponent();

            SetPlaceholders();
        }
        private void SetPlaceholders()
        {
            List<string> levelLanguageStr = new List<string>();

            for (int i = 0; i < Teacher.LevelOfLanguages.Count; i++)
            {
                levelLanguageStr.Add($"{Teacher.Languages[i]} {Teacher.LevelOfLanguages[i]}");
            }
            languageComboBox.ItemsSource = levelLanguageStr;

            string selectedLanguageAndLevel = $"{Course.Language} {Course.Level}";

            languageComboBox.SelectedItem = selectedLanguageAndLevel;

            durationTextBox.Text = Course.Duration.ToString();

            startDatePicker.SelectedDate = Course.StartDate;
            startTimeTextBox.Text = Course.StartTime;

            List<string> dayOfWeek = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

            dayListBox.ItemsSource = dayOfWeek;

            for (int i = 0; i < Course.WorkDays.Count; i++)
            {
                dayListBox.SelectedItems.Add($"{Course.WorkDays[i]}");
            }

            maxEnrolledTextBox.Text = Course.MaxEnrolledStudents.ToString();
        }

        private void PickLanguageAndLevel()
        {
            if (languageComboBox.SelectedItem != null)
            {
                string selectedLanguageAndLevel = (string)languageComboBox.SelectedItem;

                string[] parts = selectedLanguageAndLevel.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2 &&
                    Enum.TryParse(parts[0], out Language language) &&
                    Enum.TryParse(parts[1], out LanguageLevel level))
                {
                    Course.Language = language;
                    Course.Level = level;
                }
                else
                    MessageBox.Show("Invalid input format.");
            }
        }
        private void PickDataFromDatePicker()
        {
            if (startDatePicker.SelectedDate.HasValue && !string.IsNullOrWhiteSpace(startTimeTextBox.Text))
            {
                DateTime startDate = startDatePicker.SelectedDate.Value.Date;
                DateTime startTime;

                if (DateTime.TryParseExact(startTimeTextBox.Text, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
                    Course.StartDate = startDate.Add(startTime.TimeOfDay);

                else
                    MessageBox.Show("Please enter a valid start time (HH:mm).");
            }
            else
                MessageBox.Show("Please select a valid start date and time.");
        }
        private void PickDataFromListBox()
        {
            Course.WorkDays = new List<DayOfWeek>();
            foreach (var selectedItem in dayListBox.SelectedItems)
                if (Enum.TryParse(selectedItem.ToString(), out DayOfWeek day))
                    Course.WorkDays.Add(day);
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            PickDataFromDatePicker();
            PickLanguageAndLevel();
            PickDataFromListBox();
            if (Course.IsValid)
            {
                courseController.UpdateCourse(Course.ToCourse());
                Close();
            }
            else
                MessageBox.Show("Course can not be updated. Not all fields are valid.");
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
