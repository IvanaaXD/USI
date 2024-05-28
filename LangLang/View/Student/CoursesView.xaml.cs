using LangLang.Controller;
using LangLang.DTO;
using LangLang.Domain.Model;
using LangLang.Domain.Model.Enums;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace LangLang.View.Student
{
    /// <summary>
    /// Interaction logic for CoursesView.xaml
    /// </summary>
    public partial class CoursesView : Window, IObserver
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
        private CourseController courseController { get; set; }
        private int studentId { get; set; }
        private bool isSearchButtonClicked = false;
        private int selectedTabIndex = 0;
        private int currentCoursePage = 1;
        private string courseSortCriteria;

        private CoursesTable studentCoursesTable;
        private Button CourseNextButton;
        private Button CoursePreviousButton;
        private TextBlock CoursePageNumberTextBlock;
        private ComboBox courseSortCriteriaComboBox;

        public CoursesView(int studentId, int tabIndex)
        {
            InitializeComponent();
            TableViewModel = new ViewModel();
            studentsController = Injector.CreateInstance<StudentsController>();
            teacherController = Injector.CreateInstance<TeacherController>();
            courseController = Injector.CreateInstance<CourseController>();

            this.studentId = studentId;
            studentCoursesTable = (CoursesTable) FindName("StudentCoursesTable" + selectedTabIndex);
            selectedTabIndex = tabIndex;
            myTabControl.SelectedIndex = tabIndex;

            DataContext = this;
            studentsController.Subscribe(this);
            UpdatePagination();
        }
        public void Update()
        {
            try
            {
                SetCancelRequestButtonAvailability();
                TableViewModel.Courses.Clear();
                var courses = GetFilteredCourses();

                if (courses != null)
                    foreach (Course course in courses)
                        TableViewModel.Courses.Add(new CourseDTO(course));
                else
                    MessageBox.Show("No courses found.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                TabControl tabControl = sender as TabControl;
                selectedTabIndex = tabControl.SelectedIndex;

                CourseNextButton = (Button)FindName("CourseNextButton" + selectedTabIndex);
                CoursePreviousButton = (Button)FindName("CoursePreviousButton" + selectedTabIndex);
                CoursePageNumberTextBlock = (TextBlock)FindName("CoursePageNumberTextBlock" + selectedTabIndex);
                courseSortCriteriaComboBox = (ComboBox)FindName("courseSortCriteriaComboBox" + selectedTabIndex);

                UpdatePagination();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void SearchButton_Click(object sender, EventArgs e)
        {
            UpdatePagination();
            isSearchButtonClicked = true;
        }
        private void ResetButton_Click(object sender, EventArgs e)
        {
            isSearchButtonClicked = false;
            UpdatePagination();
            ResetSearchElements();
        }

        private void ResetSearchElements()
        {
            studentCoursesTable = (CoursesTable) FindName("StudentCoursesTable" + selectedTabIndex);

            studentCoursesTable.languageComboBox.SelectedItem = null;
            studentCoursesTable.levelComboBox.SelectedItem = null;
            studentCoursesTable.startDateDatePicker.SelectedDate = null;
            studentCoursesTable.durationTextBox.Text = string.Empty;
            studentCoursesTable.onlineCheckBox.IsChecked = false;
        }
        private List<Course> GetFilteredCourses()
        {
            studentCoursesTable = (CoursesTable) FindName("StudentCoursesTable" + selectedTabIndex);

            Language? selectedLanguage = (Language?) studentCoursesTable.languageComboBox.SelectedItem;
            LanguageLevel? selectedLevel = (LanguageLevel?) studentCoursesTable.levelComboBox.SelectedItem;
            DateTime? selectedStartDate = studentCoursesTable.startDateDatePicker.SelectedDate;
            int selectedDuration = 0;

            if (!string.IsNullOrEmpty(studentCoursesTable.durationTextBox.Text))
                if (int.TryParse(studentCoursesTable.durationTextBox.Text, out int duration))
                    selectedDuration = duration;

           return DoFilter(selectedLanguage,selectedLevel,selectedStartDate,selectedDuration);
        }

        private List<Course> DoFilter(Language? language, LanguageLevel? languageLevel, DateTime? startDate, int duration)
        {
            List<Course> courses = GetSelectedTabCourses();
 
            if (!isSearchButtonClicked)
                return courses;

            List<Course> filteredCourses = new List<Course>();
            bool isOnline = studentCoursesTable.onlineCheckBox.IsChecked ?? false;
            List<Course> allFilteredCourses = courseController.FindCoursesByCriteria(language, languageLevel, startDate, duration, isOnline);

            foreach (Course course in allFilteredCourses)
                foreach (Course studentCourse in courses)
                    if (studentCourse.Id == course.Id && !filteredCourses.Contains(course))
                        filteredCourses.Add(course);
            return filteredCourses;
        }

        private List<Course>? GetSelectedTabCourses()
        {
            switch(selectedTabIndex)
            {
                case 0: return studentsController.GetAvailableCourses(studentId);
                case 1: return studentsController.GetRegisteredCourses(studentId);
                case 2: return studentsController.GetCompletedCourses(studentId);
                case 3: return studentsController.GetPassedCourses(studentId);
            }
            return null;
        }

        private void SetCancelRequestButtonAvailability()
        {
            if (selectedTabIndex == 1)
            {
                if (studentsController.IsStudentAttendingCourse(studentId))
                    CancelRequestButton.IsEnabled = false;
                else
                    CancelRequestButton.IsEnabled = true;
            }
        }

        private void SignUpButton_Click(object sender, EventArgs e)
        {
            if (SelectedCourse == null)
                MessageBox.Show("Please choose a course to register!");
            else
            {
                bool isRegisteredForCourse = studentsController.RegisterForCourse(studentId, SelectedCourse.Id);
                if (isRegisteredForCourse)
                {
                    MessageBox.Show("You have sent a request to register for the course");
                    UpdatePagination();
                }
                else
                    MessageBox.Show("You are already taking a course.");
            }
        }
        private void CancelRequestButton_Click(object sender, EventArgs e)
        {
            if (SelectedCourse == null)
                MessageBox.Show("Please choose a course to cancel course request!");
            else
            {
                bool isRequestCanceled = studentsController.CancelCourseRegistration(studentId, SelectedCourse.Id);
                if (isRequestCanceled)
                {
                    MessageBox.Show("You have canceled your request to register for the course");
                    UpdatePagination();
                }
                else
                    MessageBox.Show("You cannot cancel your request less than 7 days before the start of the course.");
            }
        }

        //--------------------------- COURSE PAGINATION --------------------------------
        private void CourseNextPage_Click(object sender, RoutedEventArgs e)
        {
            currentCoursePage++;
            CoursePreviousButton.IsEnabled = true;
            UpdatePagination();

        }

        private void CoursePreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentCoursePage > 1)
            {
                currentCoursePage--;
                CourseNextButton.IsEnabled = true;
                UpdatePagination();
            }
            else if (currentCoursePage == 1)
            {
                CoursePreviousButton.IsEnabled = false;
            }
        }
        private void UpdatePagination()
        {
            if (currentCoursePage == 1)
            {
                CoursePreviousButton = (Button)FindName("CoursePreviousButton" + selectedTabIndex);
                CoursePreviousButton.IsEnabled = false;
            }
            CoursePageNumberTextBlock.Text = $"{currentCoursePage}";

            try
            {
                SetCancelRequestButtonAvailability();
                TableViewModel.Courses.Clear();

                var filteredCourses = GetFilteredCourses();
                List<Course> courses = courseController.GetAllCourses(currentCoursePage, 1, courseSortCriteria, filteredCourses);
                List<Course> newCourses = courseController.GetAllCourses(currentCoursePage + 1, 1, courseSortCriteria, filteredCourses);

                if (newCourses.Count == 0)
                    CourseNextButton.IsEnabled = false;
                else
                    CourseNextButton.IsEnabled = true;

                if (filteredCourses != null)
                    foreach (Course course in courses)
                        TableViewModel.Courses.Add(new CourseDTO(course));
                else
                    MessageBox.Show("No exam terms found.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        private void CourseSortCriteriaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (courseSortCriteriaComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedContent = selectedItem.Content.ToString();
                switch (selectedContent)
                {
                    case "Language":
                        courseSortCriteria = "Language";
                        break;
                    case "Level":
                        courseSortCriteria = "Level";
                        break;
                    case "StartDate":
                        courseSortCriteria = "StartDate";
                        break;
                }
                UpdatePagination();
            }
        }
    }
}
