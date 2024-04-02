using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace LangLang.View.Teacher
{
    public partial class ModifyCourseDataForm : Window
    {
        public Language[] languageValues => (Language[])Enum.GetValues(typeof(Language));
        public LanguageLevel[] languageLevelValues => (LanguageLevel[])Enum.GetValues(typeof(LanguageLevel));
        public CourseDTO Course { get; set; }

        private readonly TeacherController teacherController;
        private int teacherId;

        public ModifyCourseDataForm(int courseId, TeacherController teacherController)
        {
            Course = new CourseDTO(teacherController,teacherController.GetCourseById(courseId));
            Course.StartTime = Course.StartDate.ToString("HH:mm");
            DataContext = Course;
            InitializeComponent();
            this.teacherController = teacherController;

            string selectedLanguageAndLevel = $"{Course.Language} {Course.Level}";

            languageComboBox.SelectedItem = selectedLanguageAndLevel;

            durationInWeeks.Text = Course.Duration.ToString();

            startDatePicker.SelectedDate = Course.StartDate;
            startTimeTextBox.Text = Course.StartTime;

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
        private void btnSaveData_Click(object sender, RoutedEventArgs e)
        {
            PickDataFromDatePicker();
            PickLanguageAndLevel();
            PickDataFromListBox();
            if (Course.IsValid)
            {
                    teacherController.UpdateCourse(Course.ToCourse());
                    Close();
            }
            else
            {
                MessageBox.Show("Course can not be updated. Not all fields are valid.");
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
