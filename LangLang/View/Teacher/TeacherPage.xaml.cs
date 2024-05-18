using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model;
using LangLang.Model.DAO;
using LangLang.Model.Enums;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;

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
        public DirectorService directorController { get; set; }
        public MainController mainController { get; set; }

        private bool isSearchCourseClicked = false;
        private bool isSearchExamClicked = false;
        public TeacherPage(int teacherId, MainController mainController)
        {
            InitializeComponent();
            this.teacherId = teacherId;
            this.mainController = mainController;
            this.studentController = mainController.GetStudentController();
            this.teacherController = mainController.GetTeacherController();
            this.directorController = mainController.GetDirectorController();

            this.Courses = Courses;
            this.ExamTerms = ExamTerms;

            TableViewModel = new ViewModel();
            teacherController.Subscribe(this);

            Model.Teacher teacher = directorController.GetTeacherById(teacherId);
            firstAndLastName.Text = teacher.FirstName + " " + teacher.LastName;

            courseLanguageComboBox.ItemsSource = Enum.GetValues(typeof(Language));
            courseLevelComboBox.ItemsSource = Enum.GetValues(typeof(LanguageLevel));


            List<Language> languages = new List<Language>();
            List<LanguageLevel> levels = new List<LanguageLevel>();

            var courses = GetFilteredCourses();

            foreach (Course course in courses)
            {
                if(!languages.Contains(course.Language)) 
                    languages.Add(course.Language);
                if (!levels.Contains(course.Level))  
                    levels.Add(course.Level);

            }
            examLanguageComboBox.ItemsSource = languages;
            examLevelComboBox.ItemsSource = levels;

            DataContext = this;

            Update();
            UpdateExam();
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
                UpdateExam();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void CreateCourse_Click(object sender, RoutedEventArgs e)
        {
            CreateCourseForm courseTable = new CreateCourseForm(teacherController, directorController, teacherId);
            courseTable.Show();
        }

        private void SearchCourse_Click(object sender, EventArgs e)
        {
            Update();
            isSearchCourseClicked = true;
        }

        private void ResetCourse_Click(object sender, EventArgs e)
        {
            isSearchCourseClicked = false;
            Update();
            ResetCourse_Click();
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
                    UpdateCourseForm updateForm = new UpdateCourseForm(SelectedCourse.Id, teacherId, teacherController, directorController);
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
                    directorController.RemoveCourseFromList(teacherId, SelectedCourse.Id);
                    teacherController.DeleteCourse(SelectedCourse.Id);
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

        private List<Course>? GetFinalDisplayCourses(List<Course> availableCourses, Language? selectedLanguage, LanguageLevel? selectedLevel, DateTime? selectedStartDate, int selectedDuration)
        {
            List<Course> finalCourses = new();

            if (isSearchCourseClicked)
            {
                bool isOnline = courseOnlineCheckBox.IsChecked ?? false;
                List<Course> allFilteredCourses = teacherController.FindCoursesByCriteria(selectedLanguage, selectedLevel, selectedStartDate, selectedDuration, isOnline);
                foreach (Course course in allFilteredCourses)
                {
                    foreach (Course teacherCourse in availableCourses)
                    {
                        if (teacherCourse.Id == course.Id)
                            finalCourses.Add(course);
                    }
                }
            }
            else
            {
                foreach (Course course in availableCourses)
                    finalCourses.Add(course);
            }
            return finalCourses;
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

            Model.Teacher teacher = directorController.GetTeacherById(teacherId);

            List<Course> availableCourses = teacherController.GetAvailableCourses(teacher);

            return GetFinalDisplayCourses(availableCourses, selectedLanguage, selectedLevel, selectedStartDate, selectedDuration);
        }

        private void CreateExam_Click(object sender, RoutedEventArgs e)
        {
            CreateExamForm examTable = new CreateExamForm(teacherController, directorController ,teacherId);
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
                UpdateExamForm modifyDataForm = new UpdateExamForm(teacherController, directorController, teacherId, SelectedExamTerm.ExamID);
                modifyDataForm.Show();
                modifyDataForm.Activate();
            }
        }
        private void ResetExam_Click(object sender, RoutedEventArgs e)
        {
            isSearchExamClicked = false;
            UpdateExam();
            examLanguageComboBox.SelectedItem = null;
            examLevelComboBox.SelectedItem = null;
            examDatePicker.SelectedDate = null;
        }
        private void SearchExam_Click(object sender, RoutedEventArgs e)
        {
            UpdateExam();
            isSearchExamClicked = true;
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
                    teacherController.DeleteExamTerm(SelectedExamTerm.ExamID);
            }
        }

        private void ViewCourse_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCourse == null)
            {
                MessageBox.Show("Please choose a course to view!");
            }
            else
            {
                Course course = teacherController.GetCourseById(SelectedCourse.Id);
                CourseView courseView = new CourseView(course, directorController.GetTeacherById(this.teacherId), mainController);
                courseView.Show();
            }
        }

        private void ViewExam_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedExamTerm == null)
                MessageBox.Show("Please choose an exam term to view!");
            else
            {
                ExamTerm examTerm = teacherController.GetExamTermById(SelectedExamTerm.ExamID);
                Model.Teacher teacher = directorController.GetTeacherById(this.teacherId);
                ExamTermView examTermView = new ExamTermView(examTerm, teacher, teacherController, studentController);
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

            var courses = GetFilteredCourses();

            List<ExamTerm> examTerms = teacherController.GetAllExamTerms();
            List<ExamTerm> finalExamTerms = new List<ExamTerm>();

            foreach (Course course in courses)
            {
                foreach (ExamTerm exam in examTerms)
                {

                    if (course.Id == exam.CourseID)
                    {
                        finalExamTerms.Add(exam);
                    }
                }
            }

            if (isSearchExamClicked)
            {
                TeacherDAO teacherDAO = new TeacherDAO();
                finalExamTerms = teacherDAO.FindExamTermsByCriteria(selectedLanguage, selectedLevel, selectedStartDate);
            }
            return finalExamTerms;
        }


    }
}
