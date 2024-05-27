using LangLang.Controller;
using LangLang.DTO;
using LangLang.Domain.Model;
using LangLang.Controller;
using LangLang.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.DirectoryServices.ActiveDirectory;

namespace LangLang.View.Teacher
{
    public partial class CreateExamForm : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Language[] languageValues => (Language[])Enum.GetValues(typeof(Language));
        public LanguageLevel[] languageLevelValues => (LanguageLevel[])Enum.GetValues(typeof(LanguageLevel));

        private ExamTermDTO _examTerm;
        public TeacherDTO Teacher { get; set; }

        public ExamTermDTO CreatedExamTerm
        {
            get { return _examTerm; }
            set
            {
                _examTerm = value;
                OnPropertyChanged(nameof(CreatedExamTerm));
            }
        }

        private readonly TeacherController teacherController;
        private readonly CourseController courseController;
        private readonly ExamTermController examTermController;
        private readonly DirectorController directorController;
        private int teacherId;
        public CreateExamForm(MainController mainController, int teacherId)
        {
            InitializeComponent();

            directorController = Injector.CreateInstance<DirectorController>();
            teacherController = Injector.CreateInstance<TeacherController>();
            courseController = Injector.CreateInstance<CourseController>();
            examTermController = Injector.CreateInstance<ExamTermController>();

            if (teacherId != -1)
            {
                Domain.Model.Teacher teacher = directorController.GetTeacherById(teacherId);
                CreatedExamTerm = new ExamTermDTO(teacherController, teacher);
                Teacher = new TeacherDTO(directorController.GetTeacherById(teacherId));
            }
            else
            {
                CreatedExamTerm = new ExamTermDTO();
            }


            this.teacherId = teacherId;
            DataContext = this;

            FillLanguageAndLevelCombobox();

            CreatedExamTerm.ExamDate = DateTime.Now;
            CreatedExamTerm.ExamTime = "10:00";
            CreatedExamTerm.MaxStudents = 80;
        }
        private void FillLanguageAndLevelCombobox()
        {
            List<Course> courses = courseController.GetAllCourses();
            List<string> levelLanguageStr = new List<string>();
            if (teacherId == -1)
            {
                foreach (Course course in courses)
                {
                    string languageLevel = $"{course.Language} {course.Level}";
                    if (!levelLanguageStr.Contains(languageLevel))
                        levelLanguageStr.Add(languageLevel);
                }
            }
            else
            {
                foreach (Course course in courses)
                {
                    if (Teacher.CoursesId.Contains(course.Id))
                    {
                        string languageLevel = $"{course.Language} {course.Level}";
                        if (!levelLanguageStr.Contains(languageLevel))
                            levelLanguageStr.Add(languageLevel);
                    }
                }
            }

            languageComboBox.ItemsSource = levelLanguageStr;
        }
        private void PickLanguageAndLevel()
        {
            if (languageComboBox.SelectedItem != null)
            {
                string selectedLanguageAndLevel = (string)languageComboBox.SelectedItem;
                SetLanguageAndLevel(selectedLanguageAndLevel);
            }
        }
        private void SetLanguageAndLevel(string selectedLanguageAndLevel)
        {
            Language lang = Domain.Model.Enums.Language.German;
            LanguageLevel lvl = LanguageLevel.A1;
            string[] parts = selectedLanguageAndLevel.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 2)
            {
                if (Enum.TryParse(parts[0], out Language language))
                    lang = language;
                else
                    MessageBox.Show($"Invalid language: {parts[0]}");
                if (Enum.TryParse(parts[1], out LanguageLevel level))
                    lvl = level;
                else
                    MessageBox.Show($"Invalid level: {parts[1]}");
                SetCourseForExamTerm(lang, lvl);
            }
            else
            {
                MessageBox.Show("Invalid language and level format.");
            }
        }
        public void SetCourseForExamTerm(Language lang, LanguageLevel lvl)
        {
            List<Course> courses;
            if (teacherId == -1)
            {
                courses = courseController.GetAllCourses();
            }
            else
            {
                Domain.Model.Teacher teacher = directorController.GetTeacherById(teacherId);
                courses = teacherController.GetAvailableCourses(teacher);
            }

            foreach (Course course in courses)
            {
                if (course.Language == lang && course.Level == lvl)
                {
                    CreatedExamTerm.CourseID = course.Id;
                    break;
                }
            }
        }
        private void PickDataFromDatePicker()
        {
            if (dpExamDate.SelectedDate.HasValue && !string.IsNullOrWhiteSpace(txtExamTime.Text))
            {
                DateTime startDate = dpExamDate.SelectedDate.Value.Date;
                DateTime startTime;
                if (DateTime.TryParseExact(txtExamTime.Text, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
                {
                    CreatedExamTerm.ExamDate = startDate.Add(startTime.TimeOfDay);
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

        private void Create_Click(object sender, RoutedEventArgs e)
        {

            PickDataFromDatePicker();
            PickLanguageAndLevel();

            CreateExamTerm();

        }
        private void CreateExamTerm()
        {
            if (CreatedExamTerm.IsValid && teacherId != -1)
            {
                int examId = teacherController.GetAllExamTerms().Last().ExamID;
                Domain.Model.Teacher teacher = directorController.GetTeacherById(teacherId);
                List<Course> courses = teacherController.GetAvailableCourses(teacher);

                foreach (Course course in courses)
                {
                    if (course.Id == CreatedExamTerm.CourseID)
                    {
                        course.ExamTerms.Add(examId + 1);
                        courseController.UpdateCourse(course);
                    }
                }
                examTermController.AddExamTerm(CreatedExamTerm.ToExamTerm());
                Close();
                return;
            }
            else if (teacherId == -1)
            {
                int examId = teacherController.GetAllExamTerms().Last().ExamID;
                ExamTerm examTerm = CreatedExamTerm.ToExamTerm();
                int teacherId = directorController.FindMostAppropriateTeacher(examTerm);
                Domain.Model.Teacher teacher = directorController.GetTeacherById(teacherId);
                CreatedExamTerm.SetTeacher(teacher);
                if (CreatedExamTerm.IsValid)
                {
                    List<Course> courses = courseController.GetAllCourses();

                    foreach (Course course in courses)
                    {
                        if (course.Id == CreatedExamTerm.CourseID)
                        {
                            course.ExamTerms.Add(examId + 1);
                            courseController.UpdateCourse(course);
                        }
                    }
                    examTermController.AddExamTerm(CreatedExamTerm.ToExamTerm());
                    Close();
                    return;
                }
            }
            MessageBox.Show("Course cannot be created. Not all fields are valid.");

        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
