using System;
using System.Collections.Generic;
using System.Windows;
using LangLang.Domain.Model.Enums;
using LangLang.Controller;
using LangLang.DTO;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

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
        public TeacherDTO Teacher { get; set; }

        public CourseDTO Course
        {
            get { return _course; }
            set
            {
                _course = value;
                OnPropertyChanged(nameof(Course));
            }
        }

        private readonly TeacherController teacherController;
        private readonly CourseController courseController;
        private readonly DirectorController directorController;
        private int teacherId;

        public CreateCourseForm(int teacherId, MainController mainController)
        {
            InitializeComponent();

            Course = new CourseDTO(teacherController, mainController.GetDirectorController().GetTeacherById(teacherId));
            Teacher = new TeacherDTO(mainController.GetDirectorController().GetTeacherById(teacherId));

            this.directorController = mainController.GetDirectorController();
            this.teacherController = mainController.GetTeacherController();
            this.courseController = mainController.GetCourseController();

            this.teacherId = teacherId;
            DataContext = Course;

            SetPlaceholders();
        }

        private void SetPlaceholders()
        {
            List<string> levelLanguageStr = new List<string>();

            for (int i = 0; i < Teacher.LevelOfLanguages.Count; i++)
                levelLanguageStr.Add($"{Teacher.Languages[i]} {Teacher.LevelOfLanguages[i]}");

            languageComboBox.ItemsSource = levelLanguageStr;

            Course.StartDate = DateTime.Today;
            Course.StartTime = "00:00";
            Course.Duration = "1";
            Course.MaxEnrolledStudents = "50";

            durationTextBox.GotFocus += DurationTextBox_GotFocus;
            startTimeTextBox.GotFocus += StartTimeTextBox_GotFocus;
            maxEnrolledTextBox.GotFocus += MaxEnrolledTextBox_GotFocus;
        }
        private void DurationTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            durationTextBox.Text = string.Empty;
        }
        private void StartTimeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            startTimeTextBox.Text = string.Empty;
        }
        private void MaxEnrolledTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            maxEnrolledTextBox.Text = string.Empty;
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

        private void PickDataFromCheckBox()
        {
            Course.IsOnline = isOnlineCheckBox.IsChecked ?? false;
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
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            PickDataFromCheckBox();
            PickDataFromDatePicker();
            PickLanguageAndLevel();
            PickDataFromListBox();

            if (Course.IsValid)
            {
                int courseId = teacherController.GetAllCourses().Last().Id;
                Domain.Model.Teacher teacher = directorController.GetTeacherById(teacherId);
                teacher.CoursesId.Add(courseId + 1);
                directorController.Update(teacher);
                courseController.AddCourse(Course.ToCourse());

                Close();
            }
            else
                MessageBox.Show("Course cannot be created. Not all fields are valid.");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
