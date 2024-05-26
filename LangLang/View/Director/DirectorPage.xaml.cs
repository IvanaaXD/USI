using LangLang.Controller;
using LangLang.DTO;
using LangLang.Domain.Model.Enums;
using LangLang.Domain.Model;
using LangLang.Observer;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Windows;
using LangLang.View.Teacher;
using System.IO;

namespace LangLang.View.Director
{
    public partial class DirectorPage : Window, IObserver
    {
        public ObservableCollection<TeacherDTO>? Teachers { get; set; }

        public class ViewModel
        {
            public ObservableCollection<TeacherDTO> Teachers { get; set; }
            public ObservableCollection<CourseDTO> Courses { get; set; }
            public ObservableCollection<CourseDTO> CoursesDirector { get; set; }
            public ViewModel()
            {
                Teachers = new ObservableCollection<TeacherDTO>();
                Courses = new ObservableCollection<CourseDTO>();
                CoursesDirector = new ObservableCollection<CourseDTO>();
            }
        }
        private readonly int directorId;
        private readonly DirectorController directorController;
        readonly TeacherController teacherController;
        private readonly ReportController reportController;
        private readonly MainController mainController;

        Domain.Model.Director director;

        public TeacherDTO? SelectedTeacher { get; set; }
        public CourseDTO SelectedCourse { get; set; }

        public ViewModel TableViewModel { get; set; }

        private bool isSearchButtonClicked = false;

        public DirectorPage(int directorId, MainController mainController)
        {
            InitializeComponent();
            this.directorId = directorId;
            this.mainController = mainController;
            this.directorController = Injector.CreateInstance<DirectorController>();
            this.teacherController = mainController.GetTeacherController();
            this.reportController = Injector.CreateInstance<ReportController>();

            TableViewModel = new ViewModel();
            DataContext = this;
            directorController.Subscribe(this);
            this.director = directorController.GetDirector();
            Domain.Model.Director director = directorController.GetDirector();
            firstAndLastName.Text = director.FirstName + " " + director.LastName;

            languageComboBox.ItemsSource = Enum.GetValues(typeof(Language));
            levelComboBox.ItemsSource = Enum.GetValues(typeof(LanguageLevel));

            Update();
            UpdateCourses();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.Show();
            this.Close();
        }

        public void Update()
        {
            try
            {
                TableViewModel.Teachers.Clear();
                TableViewModel.CoursesDirector.Clear();
                var teachers = directorController.GetAllTeachers();
                var coursesId = director.CoursesId;
                var courses = teacherController.GetAllCourses();
                if (coursesId != null)
                {
                    foreach (Domain.Model.Course course in courses)
                        if (coursesId.Contains(course.Id))
                            TableViewModel.CoursesDirector.Add(new CourseDTO(course));
                }
                if (teachers != null)
                {
                    foreach (Domain.Model.Teacher teacher in teachers)
                        TableViewModel.Teachers.Add(new TeacherDTO(teacher));
                }
                else
                {
                    MessageBox.Show("No teachers found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        public void UpdateSearch()
        {
            try
            {
                TableViewModel.Teachers.Clear();
                List<Domain.Model.Teacher> teachers = GetFilteredTeachers();

                if (teachers != null)
                {
                    foreach (Domain.Model.Teacher teacher in teachers)
                        TableViewModel.Teachers.Add(new TeacherDTO(teacher));
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

        private void CreateTeacher_Click(object sender, RoutedEventArgs e)
        {
            CreateTeacherFrom createTeacherFrom = new CreateTeacherFrom(directorController);
            createTeacherFrom.Show();
        }

        private void UpdateTeacher_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTeacher == null)
                MessageBox.Show("Please choose a teacher to update!");
            else
            {
                UpdateTeacherForm updateTeacherForm = new UpdateTeacherForm(SelectedTeacher.Id, directorController);
                updateTeacherForm.Show();
                updateTeacherForm.Activate();
            }
        }

        private void DeleteTeacher_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTeacher == null)
                MessageBox.Show("Please choose a teacher to delete!");
            else 
            {
                var activeCoursesWithoutTeacher = directorController.SetTeacher(SelectedTeacher.ToTeacher());

                if (activeCoursesWithoutTeacher.Count > 0)
                {
                    foreach (var course in activeCoursesWithoutTeacher)
                    { 
                        ChooseTeacherView choseTeacherView = new ChooseTeacherView();
                    }
                }

                directorController.Delete(SelectedTeacher.Id);
            }
        }

        private void SearchTeachers_Click(object sender, RoutedEventArgs e)
        {
            UpdateSearch();
            isSearchButtonClicked = true;
        }

        private void ResetTeachers_Click(object sender, EventArgs e)
        {
            isSearchButtonClicked = false;
            Update();
            ResetSearchElements();
        }

        private void ResetSearchElements()
        {
            languageComboBox.SelectedItem = null;
            levelComboBox.SelectedItem = null;
            startedWorkDatePicker.SelectedDate = null;
        }

        private List<Domain.Model.Teacher> GetFilteredTeachers()
        {
            Language selectedLanguage = Domain.Model.Enums.Language.NULL;
            LanguageLevel selectedLevel = LanguageLevel.NULL;
            DateTime selectedStartDate = DateTime.MinValue;

            if (languageComboBox.SelectedItem != null)
                selectedLanguage = (Language)languageComboBox.SelectedItem;

            if (levelComboBox.SelectedItem != null)
                selectedLevel = (LanguageLevel)levelComboBox.SelectedItem;

            if (startedWorkDatePicker.SelectedDate.HasValue)
                selectedStartDate = (DateTime)startedWorkDatePicker.SelectedDate;

            return GetDisplayTeachers(selectedLanguage, selectedLevel, selectedStartDate);
        }

        private List<Domain.Model.Teacher> GetDisplayTeachers(Language selectedLanguage, LanguageLevel selectedLevel, DateTime selectedStartDate)
        {
            List<Domain.Model.Teacher> finalTeachers = new();

            if (isSearchButtonClicked)
            {
                List<Domain.Model.Teacher> allFilteredTeachers = directorController.FindTeachersByCriteria(selectedLanguage, selectedLevel, selectedStartDate);

                foreach (Domain.Model.Teacher teacher in allFilteredTeachers)
                {

                    finalTeachers.Add(teacher);
                }
            }
            else
            {
                foreach (Domain.Model.Teacher teacher in directorController.GetAllTeachers())
                {
                    finalTeachers.Add(teacher);
                }
            }
            return finalTeachers;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            UpdateCourses();
            isSearchButtonClicked = true;
        }
        public void UpdateCourses()
        {
            try
            {
                TableViewModel.Courses.Clear();
                var courses = mainController.GetCourseController().GetCoursesForTopStudentMails();

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

        private void ViewCourseButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCourse == null)
            {
                MessageBox.Show("Please choose a course to view!");
            }
            else
            {
                CourseView courseView = new CourseView(SelectedCourse.Id, directorController.GetDirector());
                courseView.Show();
                UpdateCourses();
            }
        }
        private void CreateCourse_Click(object sender, RoutedEventArgs e)
        {
            CreateCourseForm courseForm = new CreateCourseForm(-1, mainController);
            courseForm.Show();
        }

        private void AssignTeacherCourse_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCourse == null)
            {
                MessageBox.Show("Please choose a course to assign teacher!");
            }
            else
            {
                // check if course has teacher
                // assign teacher
            }
        }
        private void SendReport_Click(object sender, RoutedEventArgs e) 
        {
            EmailSender emailSender = new EmailSender("smtp.gmail.com", 587, "diirrektorr@gmail.com", "dvwa dbkw bzyl cauy");
            if (ReportOneRadioButton.IsChecked == true)
            {
                reportController.GenerateFirstReport();
                emailSender.SendEmail("diirrektorr@gmail.com", "diirrektorr@gmail.com", "Report 1", "Report 1 body",
                                      "..\\..\\..\\Data\\report1.pdf");
            }
            else if (ReportTwoRadioButton.IsChecked == true)
            {
                reportController.GenerateSecondReport();
                emailSender.SendEmail("diirrektorr@gmail.com", "diirrektorr@gmail.com", "Report 2", "Report 2 body",
                                      "..\\..\\..\\Data\\report2.pdf");
            }
            else if (ReportThreeRadioButton.IsChecked == true)
            {
                reportController.GenerateThirdReport();
                emailSender.SendEmail("diirrektorr@gmail.com", "diirrektorr@gmail.com", "Report 3", "Report 3 body",
                                      "..\\..\\..\\Data\\report3.pdf");
            }
            else if (ReportFourRadioButton.IsChecked == true)
            {
                reportController.GenerateFourthReport();
                emailSender.SendEmail("diirrektorr@gmail.com", "diirrektorr@gmail.com", "Report 4", "Report 4 body",
                      "..\\..\\..\\Data\\report4.pdf");
            } 
            else 
                MessageBox.Show("Please select the report you want to send.");

            
        }
        public void SearchDirectorCourse_Click(object sender, EventArgs e)
        {

        }
        public void ResetDirectorCourse_Click(object sender, EventArgs e)
        {

        }
    }
}
