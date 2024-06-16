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
using LangLang.Domain.IUtility;
using LangLang.Domain.Utility;

namespace LangLang.View.Teacher
{
    public partial class TeacherPage : Window, IObserver
    {
        public ObservableCollection<CourseDTO> Courses { get; set; }
        public ObservableCollection<ExamTermDTO> ExamTerms { get; set; }
        public class ViewModel
        {
            public ObservableCollection<CourseDTO> Courses { get; set; }
            public ObservableCollection<ExamTermDTO> ExamTerms { get; set; }

            public ViewModel()
            {
                Courses = new ObservableCollection<CourseDTO>();
                ExamTerms = new ObservableCollection<ExamTermDTO>();
            }
        }
        readonly int teacherId;
        public ViewModel TableViewModel { get; set; }
        public CourseDTO SelectedCourse { get; set; }
        public ExamTermDTO SelectedExamTerm { get; set; }
        public StudentsController studentController { get; set; }
        public TeacherController teacherController { get; set; }
        public DirectorController directorController { get; set; }
        public ExamTermController examTermController { get; set; }
        public CourseController courseController { get; set; }

        private bool isSearchCourseClicked = false;
        private bool isSearchExamClicked = false;
        private int currentExamPage = 1;
        private int currentCoursePage = 1;
        private string sortCriteria;
        private string courseSortCriteria;

        private ISortStrategy currentSortStrategy = new SortByDatetime();
        private ISortStrategy courseSortStrategy = new SortByDatetime();

        public TeacherPage(int teacherId)
        {
            InitializeComponent();
            this.teacherId = teacherId;
            studentController = Injector.CreateInstance<StudentsController>();
            teacherController = Injector.CreateInstance<TeacherController>();
            directorController = Injector.CreateInstance<DirectorController>();
            examTermController = Injector.CreateInstance<ExamTermController>();
            courseController = Injector.CreateInstance<CourseController>();

            Courses = Courses;
            ExamTerms = ExamTerms;

            TableViewModel = new ViewModel();
            teacherController.Subscribe(this);
            directorController.Subscribe(this);

            Domain.Model.Teacher teacher = directorController.GetById(teacherId);
            firstAndLastName.Text = teacher.FirstName + " " + teacher.LastName;

            studentRating.Text = directorController.GetAverageTeacherGrade(teacherId).ToString();

            courseLanguageComboBox.ItemsSource = Enum.GetValues(typeof(Language));
            courseLevelComboBox.ItemsSource = Enum.GetValues(typeof(LanguageLevel));


            List<Language> languages = new List<Language>();
            List<LanguageLevel> levels = new List<LanguageLevel>();

            var courses = GetFilteredCourses();

            foreach (Course course in courses)
            {
                if (!languages.Contains(course.Language))
                    languages.Add(course.Language);
                if (!levels.Contains(course.Level))
                    levels.Add(course.Level);

            }
            examLanguageComboBox.ItemsSource = languages;
            examLevelComboBox.ItemsSource = levels;

            DataContext = this;

            Update();
            UpdateExamPagination();
            UpdateCoursePagination();
        }

        public void Update()
        {
            try
            {
                UpdateExamPagination();
                UpdateCoursePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        private void UpdateCourses()
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

        private void CreateCourse_Click(object sender, RoutedEventArgs e)
        {
            CreateCourseForm courseTable = new CreateCourseForm(teacherId);
            courseTable.Show();
        }

        private void SearchCourse_Click(object sender, EventArgs e)
        {
            isSearchCourseClicked = true;
            UpdateCoursePagination();
        }

        private void ResetCourse_Click(object sender, EventArgs e)
        {
            isSearchCourseClicked = false;
            ResetCourse_Click();
            UpdateCoursePagination();
        }

        private void UpdateCourse_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCourse == null)
            {
                MessageBox.Show("Please choose a course to update!");
            }
            else
            {
                if (DateTime.Now.AddDays(7) > SelectedCourse.StartDate)
                    MessageBox.Show("Cannot update a course that starts in less than a week.");
                else
                {
                    UpdateCourseForm updateForm = new UpdateCourseForm(SelectedCourse.Id, teacherId);
                    updateForm.Show();
                }
            }
        }

        private void DeleteCourse_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCourse == null)
            {
                MessageBox.Show("Please choose a course to delete!");
            }
            else
            {
                if (DateTime.Now.AddDays(7) > SelectedCourse.StartDate)
                    MessageBox.Show("Cannot delete a course that starts in less than a week.");
                else
                {
                    int courseId = SelectedCourse.Id;
                    courseController.Delete(SelectedCourse.ToCourse());
                    directorController.RemoveCourseFromList(teacherId, courseId);
                    directorController.RemoveCourseFromDirector(courseId);
                }
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.Show();
            this.Close();
        }

        private void ResetCourse_Click()
        {
            courseLanguageComboBox.SelectedItem = null;
            courseLevelComboBox.SelectedItem = null;
            courseStartDateDatePicker.SelectedDate = null;
            courseDurationTextBox.Text = string.Empty;
            courseOnlineCheckBox.IsChecked = false;
        }

        private List<Course>? GetFilteredCourses()
        {
            Language? selectedLanguage = (Language?)courseLanguageComboBox.SelectedItem;
            LanguageLevel? selectedLevel = (LanguageLevel?)courseLevelComboBox.SelectedItem;
            DateTime? selectedStartDate = courseStartDateDatePicker.SelectedDate;
            int selectedDuration = 0;
            if (!string.IsNullOrEmpty(courseDurationTextBox.Text))
            {
                if (int.TryParse(courseDurationTextBox.Text, out int duration))
                {
                    selectedDuration = duration;
                }
            }
            bool isOnline = courseOnlineCheckBox.IsChecked ?? false;

            Domain.Model.Teacher teacher = directorController.GetById(teacherId);

            List<Course> availableCourses = courseController.GetAvailableCourses(teacher);

            return courseController.GetCoursesForDisplay(isSearchCourseClicked, availableCourses, selectedLanguage, selectedLevel, selectedStartDate, selectedDuration, isOnline);
        }

        private void CreateExam_Click(object sender, RoutedEventArgs e)
        {
            CreateExamForm examTable = new CreateExamForm(teacherId);
            examTable.Show();
        }
        private void UpdateExam_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedExamTerm == null)
            {
                MessageBox.Show("Please choose exam to update!");
            }
            else
            {
                UpdateExamForm modifyDataForm = new UpdateExamForm(teacherId, SelectedExamTerm.ExamID);
                modifyDataForm.Show();
                modifyDataForm.Activate();
            }
        }
        private void ResetExam_Click(object sender, RoutedEventArgs e)
        {
            isSearchExamClicked = false;
            ResetExam_Click();
            UpdateExamPagination();
        }
        private void SearchExam_Click(object sender, RoutedEventArgs e)
        {
            UpdateExamPagination();
            isSearchExamClicked = true;
        }
        private void ResetExam_Click()
        {
            examLanguageComboBox.SelectedItem = null;
            examLevelComboBox.SelectedItem = null;
            examDatePicker.SelectedDate = null;
        }
        private void DeleteExam_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedExamTerm == null)
            {
                MessageBox.Show("Please choose an exam term to cancel!");
            }
            else
            {
                if (DateTime.Now.AddDays(14) > SelectedExamTerm.ExamDate)
                    MessageBox.Show("Cannot cancel an exam that starts in less than a 2 week.");
                else
                    teacherController.RemoveExamTerm(SelectedExamTerm.ToExamTerm().ExamID);
            }
        }

        private void ViewCourse_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCourse == null)
                MessageBox.Show("Please choose a course to view!");
            else
            {
                Course course = teacherController.GetCourseById(SelectedCourse.Id);
                CourseView courseView = new CourseView(course, directorController.GetById(this.teacherId));
                courseView.Show();
            }
        }

        private void ViewExam_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedExamTerm == null)
                MessageBox.Show("Please choose an exam term to view!");
            else
            {
                ExamTerm? examTerm = teacherController.GetExamTermById(SelectedExamTerm.ExamID);
                Domain.Model.Teacher? teacher = directorController.GetById(this.teacherId);
                ExamTermView examTermView = new ExamTermView(examTerm, teacher, this);
                examTermView.Owner = this;
                this.Visibility = Visibility.Collapsed;
                examTermView.Show();
            }
        }

        public void UpdateExam()
        {
            try
            {
                TableViewModel.ExamTerms.Clear();

                var examTerms = GetFilteredExamTerms();
                if (examTerms != null)
                {
                    foreach (ExamTerm examTerm in examTerms)
                        TableViewModel.ExamTerms.Add(new ExamTermDTO(examTerm));
                }
                else
                {
                    MessageBox.Show("No exam terms found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        private List<ExamTerm> GetFilteredExamTerms()
        {
            Language? selectedLanguage = (Language?)examLanguageComboBox.SelectedItem;
            LanguageLevel? selectedLevel = (LanguageLevel?)examLevelComboBox.SelectedItem;
            DateTime? selectedStartDate = examDatePicker.SelectedDate;

            Domain.Model.Teacher teacher = directorController.GetById(teacherId);

            List<ExamTerm> availableExams = teacherController.GetAvailableExamTerms(teacher);
            
            return examTermController.GetExamsForDisplay(isSearchExamClicked, availableExams, selectedLanguage, selectedLevel, selectedStartDate);
        }
    
        private void NextExamPage_Click(object sender, RoutedEventArgs e)
        {

            currentExamPage++;
            PreviousButton.IsEnabled = true;
            UpdateExamPagination();

        }

        private void PreviousExamPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentExamPage > 1)
            {
                currentExamPage--;
                NextButton.IsEnabled = true;
                UpdateExamPagination();
            }
            else if (currentExamPage == 1)
            {
                PreviousButton.IsEnabled = false;
            }
        }
        private void UpdateExamPagination()
        {
            if (currentExamPage == 1)
            {
                PreviousButton.IsEnabled = false;
            }
            PageNumberTextBlock.Text = $"{currentExamPage}";

            try
            {
                TableViewModel.ExamTerms.Clear();
                var examTerms = GetFilteredExamTerms();
                //List<ExamTerm> exams = examTermController.GetAllExamTerms(currentExamPage, 4, sortCriteria, examTerms);
                //List<ExamTerm> newExams = examTermController.GetAllExamTerms(currentExamPage + 1, 4, sortCriteria, examTerms);
                
                List<ExamTerm> exams = examTermController.GetAllExamTerms(currentExamPage, 4, currentSortStrategy, examTerms);
                List<ExamTerm> newExams = examTermController.GetAllExamTerms(currentExamPage + 1, 4, currentSortStrategy, examTerms);
                
                if (newExams.Count == 0)
                    NextButton.IsEnabled = false;
                else NextButton.IsEnabled = true;   
                if (examTerms != null)
                {
                    foreach (ExamTerm examTerm in exams)
                        TableViewModel.ExamTerms.Add(new ExamTermDTO(examTerm));
                }
                else
                {
                    MessageBox.Show("No exam terms found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        private void SortCriteriaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sortCriteriaComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedContent = selectedItem.Content.ToString();

                switch (selectedContent)
                {
                    case "Language":
                        sortCriteria = "Language";
                        currentSortStrategy = new SortByLanguage(); 
                        break;
                    case "Level":
                        sortCriteria = "Level";
                        currentSortStrategy = new SortByLevel();
                        break;
                    case "Datetime":
                        sortCriteria = "Datetime";
                        currentSortStrategy = new SortByDatetime();
                        break;
                }
                UpdateExamPagination();
            }
        }
        //--------------------------- COURSE PAGINATION --------------------------------
        private void CourseNextPage_Click(object sender, RoutedEventArgs e)
        {

            currentCoursePage++;
            CoursePreviousButton.IsEnabled = true;
            UpdateCoursePagination();

        }

        private void CoursePreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentCoursePage > 1)
            {
                currentCoursePage--;
                CourseNextButton.IsEnabled = true;
                UpdateCoursePagination();
            }
            else if (currentCoursePage == 1)
            {
                CoursePreviousButton.IsEnabled = false;
            }
        }
        private void UpdateCoursePagination()
        {
            if (currentCoursePage == 1)
            {
                CoursePreviousButton.IsEnabled = false;
            }
            CoursePageNumberTextBlock.Text = $"{currentCoursePage}";

            try
            {
                TableViewModel.Courses.Clear();
                var filteredCourses = GetFilteredCourses();
                List<Course> courses = courseController.GetAllCourses(currentCoursePage, 4, courseSortCriteria, filteredCourses);
                List<Course> newCourses = courseController.GetAllCourses(currentCoursePage + 1, 4, courseSortCriteria, filteredCourses);
                
                //List<Course> courses = courseController.GetAllCourses(currentCoursePage, 4, courseSortStrategy, filteredCourses);
                //List<Course> newCourses = courseController.GetAllCourses(currentCoursePage + 1, 4, courseSortStrategy, filteredCourses);
                
                if (newCourses.Count == 0)
                    CourseNextButton.IsEnabled = false;
                else
                    CourseNextButton.IsEnabled = true;
                if (filteredCourses != null)
                {
                    foreach (Course course in courses)
                        TableViewModel.Courses.Add(new CourseDTO(course));
                }
                else
                {
                    MessageBox.Show("No exam terms found.");
                }
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
                        courseSortStrategy = new SortByLanguage();
                        break;
                    case "Level":
                        courseSortCriteria = "Level";
                        courseSortStrategy = new SortByLevel();
                        break;
                    case "StartDate":
                        courseSortCriteria = "StartDate";
                        courseSortStrategy = new SortByDatetime();
                        break;
                }
                UpdateCoursePagination();
            }
        }
    }
}
