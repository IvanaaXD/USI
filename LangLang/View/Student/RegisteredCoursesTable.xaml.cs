using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model.Enums;
using LangLang.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using LangLang.Observer;

namespace LangLang.View.Student
{
    /// <summary>
    /// Interaction logic for RegisteredCoursesTable.xaml
    /// </summary>
    public partial class RegisteredCoursesTable : Window, IObserver
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
        private StudentsController studentsController { get; set; }
        private TeacherController teacherController { get; set; }

        private int studentId { get; set; }
        private bool isSearchButtonClicked = false;


        public RegisteredCoursesTable(int studentId)
        {
            InitializeComponent();
            TableViewModel = new ViewModel();
            studentsController = new StudentsController();
            teacherController = new TeacherController();
            this.studentId = studentId;

            languageComboBox.ItemsSource = Enum.GetValues(typeof(Language));
            levelComboBox.ItemsSource = Enum.GetValues(typeof(LanguageLevel));

            SetCancelRequestButtonAvailability();

            DataContext = this;
            studentsController.Subscribe(this);
            Update();
        }

        private void SetCancelRequestButtonAvailability()
        {
            if (studentsController.IsStudentAttendingCourse(studentId))
                CancelRequestButton.IsEnabled = false;
            else
                CancelRequestButton.IsEnabled = true;
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

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void CancelRequestButton_Click(object sender, EventArgs e)
        {
            if (SelectedCourse == null)
            {
                MessageBox.Show("Please choose a course to cancel course request!");
            }
            else
            {
                bool isRequestCanceled = studentsController.CancelCourseRegistration(studentId, SelectedCourse.Id);
                if (isRequestCanceled)
                {
                    /* MessageBox.Show("You have canceled your request to register for the course: " +
                                     $"{SelectedCourse.Language} {SelectedCourse.Level}");*/
                    MessageBox.Show("You have canceled your request to register for the course: ");
                    Update();
                }
                else
                {
                    MessageBox.Show("You cannot cancel your request less than 7 days before the start of the course.");
                }
            }
        }
        private void SearchButton_Click(object sender, EventArgs e)
        {
            Update();
            isSearchButtonClicked = true;
        }
        private void ResetButton_Click(object sender, EventArgs e)
        {
            isSearchButtonClicked = false;
            Update();
            ResetSearchElements();
        }

        private void ResetSearchElements()
        {
            languageComboBox.SelectedItem = null;
            levelComboBox.SelectedItem = null;
            startDateDatePicker.SelectedDate = null;
            durationTextBox.Text = string.Empty;
            onlineCheckBox.IsChecked = false;
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

            List<Course> studentsAvailableCourses = studentsController.GetRegisteredCourses(studentId);
            List<Course> finalCourses = new List<Course>();

            if (isSearchButtonClicked)
            {
                bool isOnline = onlineCheckBox.IsChecked ?? false;
                List<Course> allFilteredCourses = teacherController.FindCoursesByCriteria(selectedLanguage, selectedLevel, selectedStartDate, selectedDuration, isOnline);

                foreach (Course course in allFilteredCourses)
                {
                    foreach (Course studentCourse in studentsAvailableCourses)
                    {
                        if (studentCourse.Id == course.Id && !finalCourses.Contains(course))
                        {
                            finalCourses.Add(course);
                        }
                    }
                }
            }
            else
            {
                foreach (Course studentCourse in studentsAvailableCourses)
                {
                    finalCourses.Add(studentCourse);
                }
            }
            return finalCourses;
        }
    }
}
