using System;
using System.Collections.Generic;
using System.Windows;
using LangLang.Domain.Model.Enums;
using LangLang.Controller;
using LangLang.DTO;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using LangLang.Domain.Model;
using System.Windows.Input;

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

        public CourseDTO CreatedCourse
        {
            get { return _course; }
            set
            {
                _course = value;
                OnPropertyChanged(nameof(CreatedCourse));
            }
        }

        private readonly TeacherController teacherController;
        private readonly CourseController courseController;
        private readonly DirectorController directorController;
        private int teacherId;

        public CreateCourseForm(int teacherId)
        {
            InitializeComponent();

            directorController = Injector.CreateInstance<DirectorController>();
            teacherController = Injector.CreateInstance<TeacherController>();
            courseController = Injector.CreateInstance<CourseController>();

            if (teacherId != -1)
            {
                CreatedCourse = new CourseDTO(directorController.GetById(teacherId));
                Teacher = new TeacherDTO(directorController.GetById(teacherId));
            }
            else
            {
                CreatedCourse = new CourseDTO();
            }
            this.teacherId = teacherId;
            DataContext = CreatedCourse;

            SetPlaceholders();
        }

        private void SetPlaceholders()
        {
            List<string> levelLanguageStr = new List<string>();
            if (teacherId == -1)
            {
                List<Domain.Model.Teacher> allTeachers = directorController.GetAllTeachers();
                foreach (Domain.Model.Teacher teacher in allTeachers)
                {
                    for (int i = 0; i < teacher.LevelOfLanguages.Count; i++)
                    {
                        string levelLanguage = $"{teacher.Languages[i]} {teacher.LevelOfLanguages[i]}";
                        if (!levelLanguageStr.Contains(levelLanguage))
                            levelLanguageStr.Add(levelLanguage);

                    }
                }
            }
            else
            {
                for (int i = 0; i < Teacher.LevelOfLanguages.Count; i++)
                    levelLanguageStr.Add($"{Teacher.Languages[i]} {Teacher.LevelOfLanguages[i]}");

            }

            languageComboBox.ItemsSource = levelLanguageStr;

            CreatedCourse.StartDate = DateTime.Today;
            CreatedCourse.StartTime = "00:00";
            CreatedCourse.Duration = "1";
            CreatedCourse.MaxEnrolledStudents = "50";

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
                    CreatedCourse.Language = language;
                    CreatedCourse.Level = level;
                }
                else
                    MessageBox.Show("Invalid input format.");
            }
        }

        private void PickDataFromCheckBox()
        {
            CreatedCourse.IsOnline = isOnlineCheckBox.IsChecked ?? false;
        }


        private void PickDataFromDatePicker()
        {
            if (startDatePicker.SelectedDate.HasValue && !string.IsNullOrWhiteSpace(startTimeTextBox.Text))
            {
                DateTime startDate = startDatePicker.SelectedDate.Value.Date;
                DateTime startTime;

                if (DateTime.TryParseExact(startTimeTextBox.Text, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
                    CreatedCourse.StartDate = startDate.Add(startTime.TimeOfDay);

                else
                    MessageBox.Show("Please enter a valid start time (HH:mm).");
            }
            else
                MessageBox.Show("Please select a valid start date and time.");
        }

        private void PickDataFromListBox()
        {
            CreatedCourse.WorkDays = new List<DayOfWeek>();
            foreach (var selectedItem in dayListBox.SelectedItems)

                if (Enum.TryParse(selectedItem.ToString(), out DayOfWeek day))
                    CreatedCourse.WorkDays.Add(day);
        }
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            PickDataFromCheckBox();
            PickDataFromDatePicker();
            PickLanguageAndLevel();
            PickDataFromListBox();

            if (CreatedCourse.IsValid && teacherId != -1)
            {
                courseController.Add(CreatedCourse.ToCourse());
                Domain.Model.Teacher teacher = directorController.GetById(teacherId);
                teacher.CoursesId.Add(CreatedCourse.ToCourse().Id);
                directorController.Update(teacher);

                Close();
            }
            else if (teacherId == -1)
            {
                Domain.Model.Course course = CreatedCourse.ToCourse();
                int teacherId = directorController.FindMostAppropriateTeacher(course);
                if (teacherId != -1)
                {
                    Domain.Model.Teacher teacher = directorController.GetById(teacherId);
                    CreatedCourse.SetTeacher(teacher, course);
                }
                if (CreatedCourse.IsValid)
                {
                    int courseId = teacherController.GetAllCourses().Last().Id;
                    Domain.Model.Director director = directorController.GetDirector();
                    if (director.CoursesId == null)
                    {
                        director.CoursesId = new List<int>();
                    }
                    director.CoursesId.Add(courseId + 1);
                    directorController.Update(director);
                    courseController.Add(CreatedCourse.ToCourse());
                    Domain.Model.Teacher teacher = directorController.GetById(teacherId);
                    teacher.CoursesId.Add(CreatedCourse.ToCourse().Id);
                    directorController.Update(teacher);

                    Close();
                }
                else
                    MessageBox.Show("Course cannot be created. Not all fields are valid.");
            }

           
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
