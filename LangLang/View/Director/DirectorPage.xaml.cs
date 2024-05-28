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
using System.Runtime.CompilerServices;

namespace LangLang.View.Director
{
    public partial class DirectorPage : Window, IObserver
    {
        public ObservableCollection<TeacherDTO>? Teachers { get; set; }
        public ObservableCollection<CourseDTO> CoursesDirector { get; set; }

        public class ViewModel
        {
            public ObservableCollection<TeacherDTO> Teachers { get; set; }
            public ObservableCollection<CourseDTO> Courses { get; set; }
            public ObservableCollection<CourseDTO> CoursesDirector { get; set; }
            public ObservableCollection<ExamTermDTO> ExamTermsDirector { get; set; }
            public ObservableCollection<ExamTermDTO> GradedExamTerms { get; set; }

            public ViewModel()
            {
                Teachers = new ObservableCollection<TeacherDTO>();
                Courses = new ObservableCollection<CourseDTO>();
                CoursesDirector = new ObservableCollection<CourseDTO>();
                ExamTermsDirector = new ObservableCollection<ExamTermDTO>();
                GradedExamTerms = new ObservableCollection<ExamTermDTO>();
            }
        }
        private readonly int directorId;
        private readonly DirectorController _directorController;
        private readonly TeacherController _teacherController;
        private readonly ReportController _reportController;
        private readonly ExamTermController _examTermController;
        private readonly CourseController _courseController;
        private readonly ExamTermGradeController _examTermGradeController;
        private readonly StudentsController _studentController;

        Domain.Model.Director director;

        public TeacherDTO? SelectedTeacher { get; set; }
        public CourseDTO SelectedCourse { get; set; }
        public ExamTermDTO SelectedExamTerm { get; set; }

        public ViewModel TableViewModel { get; set; }

        private bool isSearchButtonClicked = false;

        public DirectorPage(int directorId)
        {
            InitializeComponent();
            this.directorId = directorId;
            _directorController = Injector.CreateInstance<DirectorController>();
            _teacherController = Injector.CreateInstance<TeacherController>();
            _reportController = Injector.CreateInstance<ReportController>();
            _examTermController = Injector.CreateInstance<ExamTermController>();
            _courseController = Injector.CreateInstance<CourseController>();
            _studentController = Injector.CreateInstance<StudentsController>();
            _examTermGradeController = Injector.CreateInstance<ExamTermGradeController>();

            TableViewModel = new ViewModel();
            DataContext = this;
            _directorController.Subscribe(this);
            this.director = _directorController.GetDirector();
            Domain.Model.Director director = _directorController.GetDirector();
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
                SetTeachers();
                SetCourses();
                SetExamterms();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void SetTeachers()
        {
            TableViewModel.Teachers.Clear();
            var teachers = _directorController.GetAllTeachers();

            if (teachers != null)
            {
                foreach (Domain.Model.Teacher teacher in teachers)
                    TableViewModel.Teachers.Add(new TeacherDTO(teacher));
            }
        }

        private void SetCourses()
        {
            TableViewModel.CoursesDirector.Clear();
            var coursesId = director.CoursesId;
            var courses = _teacherController.GetAllCourses();
            if (coursesId != null)
            {
                foreach (Course course in courses)
                    if (coursesId.Contains(course.Id))
                    {
                        CourseDTO courseViewModel = new CourseDTO(course);
                        Domain.Model.Teacher? courseTeacher = _directorController.GetTeacherByCourse(course.Id);
                        if (courseTeacher != null)
                            courseViewModel.HasTeacher = true;
                        else
                            courseViewModel.HasTeacher = false;
                        TableViewModel.CoursesDirector.Add(courseViewModel);
                        foreach (int examTermId in course.ExamTerms)
                            TableViewModel.ExamTermsDirector.Add(new ExamTermDTO(_examTermController.GetExamTermById(examTermId)));
                    }
            }
        }

        private void SetExamterms()
        {
            TableViewModel.ExamTermsDirector.Clear();
            var exams = _examTermController.GetAllExamTerms();

            foreach (ExamTerm exam in exams)
                TableViewModel.ExamTermsDirector.Add(new ExamTermDTO(exam));
        }

        public bool HasExamTermBeenGraded(ExamTerm examTerm, Domain.Model.Teacher teacher)
        {
            var grades = _examTermGradeController.GetExamTermGradesByTeacherExam(teacher.Id, examTerm.ExamID);
            var examTermStudents = _studentController.GetAllStudentsForExamTerm(examTerm.ExamID);

            if (grades.Count == 0)
                return false;

            if (examTermStudents.Count != grades.Count)
                return false;

            return true;
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
            CreateTeacherFrom createTeacherFrom = new CreateTeacherFrom(_directorController);
            createTeacherFrom.Show();
        }

        private void UpdateTeacher_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTeacher == null)
                MessageBox.Show("Please choose a teacher to update!");
            else
            {
                UpdateTeacherForm updateTeacherForm = new UpdateTeacherForm(SelectedTeacher.Id, _directorController);
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
                int id = SelectedTeacher.Id;
                var activeCoursesWithoutTeacher = _directorController.GetActiveCourses(SelectedTeacher.ToTeacher());

                if (activeCoursesWithoutTeacher.Count > 0)
                {
                    foreach (var course in activeCoursesWithoutTeacher)
                    {
                        ChooseTeacherView choseTeacherView = new ChooseTeacherView(course, id);
                        choseTeacherView.Show();
                        choseTeacherView.Activate();
                    }
                }

                _directorController.Delete(id);
                Update();
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
                List<Domain.Model.Teacher> allFilteredTeachers = _directorController.FindTeachersByCriteria(selectedLanguage, selectedLevel, selectedStartDate);

                foreach (Domain.Model.Teacher teacher in allFilteredTeachers)
                {

                    finalTeachers.Add(teacher);
                }
            }
            else
            {
                foreach (Domain.Model.Teacher teacher in _directorController.GetAllTeachers())
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
                var courses = _courseController.GetCoursesForTopStudentMails();

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
                CourseView courseView = new CourseView(SelectedCourse.Id, _directorController.GetDirector());
                courseView.Show();
                UpdateCourses();
            }
        }
        private void CreateCourse_Click(object sender, RoutedEventArgs e)
        {
            CreateCourseForm courseForm = new CreateCourseForm(-1);
            courseForm.Show();
        }
        private void CreateExam_Click(object sender, RoutedEventArgs e)
        {
            CreateExamForm examForm = new CreateExamForm(-1);
            examForm.Show();
        }

        private void AssignTeacherCourse_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCourse == null)
            {
                MessageBox.Show("Please choose a course to assign teacher!");
            }
            else
            {
                int teacherId = _directorController.FindMostAppropriateTeacher(SelectedCourse.ToCourse());
                if (teacherId != -1)
                {
                    Domain.Model.Teacher teacher = _directorController.GetTeacherById(teacherId);
                    SelectedCourse.SetTeacher(teacher);
                    teacher.CoursesId.Add(SelectedCourse.Id);
                    _directorController.Update(teacher);
                    MessageBox.Show($"{teacher.FirstName} {teacher.LastName}", "Teacher who was chosen");
                }
                else
                {
                    SelectedCourse.HasTeacher = false;
                    MessageBox.Show("There is no available teacher for that course");
                }
            }
        }
        private void SendReport_Click(object sender, RoutedEventArgs e)
        {
            EmailSender emailSender = new EmailSender("smtp.gmail.com", 587, "diirrektorr@gmail.com", "dvwa dbkw bzyl cauy");
            if (ReportOneRadioButton.IsChecked == true)
            {
                _reportController.GenerateFirstReport();
                emailSender.SendEmail("diirrektorr@gmail.com", "diirrektorr@gmail.com", "Report 1", "Report 1 body",
                                      "..\\..\\..\\Data\\report1.pdf");
            }
            else if (ReportTwoRadioButton.IsChecked == true)
            {
                _reportController.GenerateSecondReport();
                emailSender.SendEmail("diirrektorr@gmail.com", "diirrektorr@gmail.com", "Report 2", "Report 2 body",
                                      "..\\..\\..\\Data\\report2.pdf");
            }
            else if (ReportThreeRadioButton.IsChecked == true)
            {
                _reportController.GenerateThirdReport();
                emailSender.SendEmail("diirrektorr@gmail.com", "diirrektorr@gmail.com", "Report 3", "Report 3 body",
                                      "..\\..\\..\\Data\\report3.pdf");
            }
            else if (ReportFourRadioButton.IsChecked == true)
            {
                _reportController.GenerateFourthReport();
                emailSender.SendEmail("diirrektorr@gmail.com", "diirrektorr@gmail.com", "Report 4", "Report 4 body",
                      "..\\..\\..\\Data\\report4.pdf");
            }
            else
                MessageBox.Show("Please select the report you want to send.");
        }

        public void ViewExam_Click(object sender, EventArgs e)
        {
            if (SelectedExamTerm == null)
                MessageBox.Show("Please choose an exam term to view!");
            else
            {
                ExamTerm? examTerm = _teacherController.GetExamTermById(SelectedExamTerm.ExamID);
                Domain.Model.Teacher? teacher = _directorController.GetTeacherByCourse(examTerm.CourseID);
                ExamTermView examTermView = new ExamTermView(examTerm, teacher, this);
                examTermView.Owner = this;
                this.Visibility = Visibility.Collapsed;
                examTermView.Show();
            }
        }

        public void SearchDirectorCourse_Click(object sender, EventArgs e)
        {

        }
        public void ResetDirectorCourse_Click(object sender, EventArgs e)
        {

        }
        public void SearchDirectorExams_Click(object sender, EventArgs e)
        {

        }
        public void ResetExam_Click(object sender, EventArgs e)
        {

        }
    }
}
