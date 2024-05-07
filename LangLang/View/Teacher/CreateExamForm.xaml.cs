using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model;
using LangLang.Model.DAO;
using LangLang.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace LangLang.View.Teacher
{
    public partial class CreateExamForm : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Language[] languageValues => (Language[])Enum.GetValues(typeof(Language));
        public LanguageLevel[] languageLevelValues => (LanguageLevel[])Enum.GetValues(typeof(LanguageLevel));

        private ExamTermDTO _examTerm;
        public TeacherDTO Teacher { get; set; }

        public ExamTermDTO ExamTerm
        {
            get { return _examTerm; }
            set
            {
                _examTerm = value;
                OnPropertyChanged(nameof(ExamTerm));
            }
        }

        private TeacherController teacherController;
        private readonly DirectorController directorController;
        private int teacherId;
        public CreateExamForm(TeacherController teacherController, DirectorController directorController, int teacherId)
        {
            InitializeComponent();

            ExamTerm = new ExamTermDTO(teacherController, directorController.GetTeacherById(teacherId));
            Teacher = new TeacherDTO(directorController.GetTeacherById(teacherId));
            

            this.directorController = directorController;
            this.teacherController = teacherController;
            this.teacherId = teacherId;
            DataContext = this;

            List<Course> courses = teacherController.GetAllCourses();
            List<string> levelLanguageStr = new List<string>();

            foreach (Course course in courses)
            {
                if (Teacher.CoursesId.Contains(course.Id))
                {
                    levelLanguageStr.Add($"{course.Language} {course.Level}");
                }
            }

            languageComboBox.ItemsSource = levelLanguageStr;

            ExamTerm.ExamDate = DateTime.Now;
            ExamTerm.ExamTime = "10:00";
            ExamTerm.MaxStudents = 80;
        }

        private void PickLanguageAndLevel()
        {
            Language lang = Model.Enums.Language.German;
            LanguageLevel lvl = LanguageLevel.A1;

            if (languageComboBox.SelectedItem != null)
            {
                string selectedLanguageAndLevel = (string)languageComboBox.SelectedItem;

                string[] parts = selectedLanguageAndLevel.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2)
                {
                    if (Enum.TryParse(parts[0], out Language language))
                    {
                        lang = language;
                    }
                    else
                    {
                        MessageBox.Show($"Invalid language: {parts[0]}");
                    }

                    if (Enum.TryParse(parts[1], out LanguageLevel level))
                    {
                        lvl = level;
                    }
                    else
                    {
                        MessageBox.Show($"Invalid level: {parts[1]}");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid language and level format.");
                }



                Model.Teacher teacher = directorController.GetTeacherById(teacherId);
                List<Course> courses = teacherController.GetAvailableCourses(teacher);

                foreach (Course course in courses)
                {
                    if (course.Language == lang && course.Level == lvl)
                    {
                        ExamTerm.CourseID = course.Id;
                        break;
                    }
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
                    ExamTerm.ExamDate = startDate.Add(startTime.TimeOfDay);
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

            if (ExamTerm.IsValid)
            {
                int examId = teacherController.GetAllExamTerms().Last().ExamID;
                Model.Teacher teacher = directorController.GetTeacherById(teacherId);
                List<Course> courses = teacherController.GetAvailableCourses(teacher);

                foreach (Course course in courses)
                {
                    if (course.Id == ExamTerm.CourseID)
                    {
                        course.ExamTerms.Add(examId + 1);
                        teacherController.UpdateCourse(course);
                    }

                }

                teacherController.AddExamTerm(ExamTerm.ToExamTerm());

                Close();
            }
            else
            {
                MessageBox.Show("Exam Term cannot be created. Not all fields are valid.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
