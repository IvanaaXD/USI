using LangLang.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using LangLang.Observer;
using LangLang.DTO;
using LangLang.Controller;
using LangLang.Model.Enums;

namespace LangLang.View.Teacher
{
    public partial class CoursesTable : Window, IObserver
    {
        public class ViewModel
        {
            public ObservableCollection<CourseDTO> Courses { get; set; }

            public ViewModel()
            {
                Courses = new ObservableCollection<CourseDTO>();
            }
        }

        public ViewModel TableViewModel { get; set; }
        public CourseDTO SelectedCourse { get; set; }
        public TeacherController teacherController { get; set; }

        public DirectorController directorController { get; set; }
        public int teacherId { get; set; }
        private bool isSearchButtonClicked = false;

        public CoursesTable(int teacherId, TeacherController teacherController, DirectorController directorController)
        {
            InitializeComponent();
            TableViewModel = new ViewModel();
            this.directorController = directorController;
            this.teacherController = teacherController;
            this.teacherId = teacherId;

            languageComboBox.ItemsSource = Enum.GetValues(typeof(Language));
            levelComboBox.ItemsSource = Enum.GetValues(typeof(LanguageLevel));
            DataContext = this;

            teacherController.Subscribe(this);
            Update();
        }

        public void Update()
        {
            try
            {
                TableViewModel.Courses.Clear();
                var courses = GetFilteredCourses();

                if (courses != null)
                {
                    foreach (Course course in courses)
                        TableViewModel.Courses.Add(new CourseDTO(course));
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
            CreateCourseForm courseTable = new CreateCourseForm(teacherController, directorController, teacherId);
            courseTable.Show();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Search_Click(object sender, EventArgs e)
        {
            Update();
            isSearchButtonClicked = true;
        }
        private void Reset_Click(object sender, EventArgs e)
        {
            isSearchButtonClicked = false;
            Update();
            ResetSearchElements();
        }

        private void ExamTermTable_Click(object sender, EventArgs e)
        {
            ExamTermsTable examTable = new ExamTermsTable(teacherId, directorController);
            examTable.Show();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCourse == null)
            {
                MessageBox.Show("Please choose a course to cancel!");
            }
            else
            {
                if (DateTime.Now.AddDays(7) > SelectedCourse.StartDate)
                    MessageBox.Show("Cannot update a course that starts in less than a week.");
                else
                {
                    UpdateCourseForm updateForm = new UpdateCourseForm(SelectedCourse.Id, teacherId, teacherController, directorController);
                    updateForm.Show();
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCourse == null)
            {
                MessageBox.Show("Please choose a course to cancel!");
            }
            else
            {
                if (DateTime.Now.AddDays(7) > SelectedCourse.StartDate)
                    MessageBox.Show("Cannot cancel a course that starts in less than a week.");
                else
                    teacherController.DeleteCourse(SelectedCourse.Id);
            }
        }
        private void ResetSearchElements()
        {
            languageComboBox.SelectedItem = null;
            levelComboBox.SelectedItem = null;
            startDateDatePicker.SelectedDate = null;
            durationTextBox.Text = string.Empty;
            onlineCheckBox.IsChecked = false;
        }

        private List<Course> GetFinalDisplayCourses(List<Course> availableCourses, Language? selectedLanguage, LanguageLevel? selectedLevel, DateTime? selectedStartDate, int selectedDuration)
        {
            List<Course> finalCourses = new List<Course>();

            if (isSearchButtonClicked)
            {
                bool isOnline = onlineCheckBox.IsChecked ?? false;
                List<Course> allFilteredCourses = teacherController.FindCoursesByCriteria(selectedLanguage, selectedLevel, selectedStartDate, selectedDuration, isOnline);
                foreach (Course course in allFilteredCourses)
                {
                    foreach (Course teacherCourse in availableCourses)
                    {
                        if (teacherCourse.Id == course.Id)
                        {
                            finalCourses.Add(course);
                        }
                    }
                }
            }
            else
            {
                foreach (Course course in availableCourses)
                {
                    finalCourses.Add(course);
                }
            }
            return finalCourses;
        }

        private List<Course> GetFilteredCourses()
        {
            Language? selectedLanguage = (Language?)languageComboBox.SelectedItem;
            LanguageLevel? selectedLevel = (LanguageLevel?)levelComboBox.SelectedItem;
            DateTime? selectedStartDate = startDateDatePicker.SelectedDate;
            int selectedDuration = 0;
            if (!string.IsNullOrEmpty(durationTextBox.Text))
            {
                if (int.TryParse(durationTextBox.Text, out int duration))
                {
                    selectedDuration = duration;
                }
            }

            LangLang.Model.Teacher teacher = directorController.GetTeacherById(teacherId);

            List<Course> availableCourses = teacherController.GetAvailableCourses(teacher);

            return GetFinalDisplayCourses(availableCourses, selectedLanguage,selectedLevel,selectedStartDate,selectedDuration);
        }

    }
}
