using LangLang.Controller;
using LangLang.DTO;
using LangLang.Domain.Model;
using LangLang.Domain.Model.Enums;
using LangLang.Repository;
using LangLang.Controller;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.DirectoryServices.ActiveDirectory;

namespace LangLang.View.Teacher
{
    public partial class UpdateExamForm : Window
    {
        public ExamTermDTO ExamTerm { get; set; }
        public TeacherDTO Teacher { get; set; }

        private readonly TeacherController teacherController;
        private readonly ExamTermController examTermController;
        private readonly DirectorController directorController;
        private int teacherId;
        private int examId;
        private Domain.Model.Teacher teacher;

        public UpdateExamForm(MainController mainController, int teacherId, int examId)
        {
            this.directorController = mainController.GetDirectorController();
            this.teacherController = mainController.GetTeacherController();
            this.examTermController = mainController.GetExamTermController();

            teacher = directorController.GetTeacherById(teacherId);
            ExamTerm examTerm = teacherController.GetExamTermById(examId);
            ExamTerm = new ExamTermDTO(examTerm, teacher);
            DataContext = ExamTerm;

            InitializeComponent();
            this.teacherId = teacherId;
            this.examId = examId;
            Teacher = new TeacherDTO(directorController.GetTeacherById(teacherId));

            SetInitialLanguageAndLevel(examTerm.CourseID);
            FillLanguageAndLevelCombobox();

            examDatePicker.SelectedDate = ExamTerm.ExamDate;
            examTimeTextBox.Text = ExamTerm.ExamDate.ToString("HH:mm");
            maxStudentsTextBox.Text = ExamTerm.MaxStudents.ToString();
        }
        private void SetInitialLanguageAndLevel(int courseId)
        {
            TeacherRepository teacherDAO = new TeacherRepository();
            string languageAndLevel = teacherDAO.FindLanguageAndLevel(courseId);

            string[] parts = languageAndLevel.Split(',');
            languageAndLevel = parts[0].Trim() + " " + parts[1].Trim();
            languageComboBox.SelectedItem = languageAndLevel;
        }
        private void FillLanguageAndLevelCombobox()
        {
            teacher = directorController.GetTeacherById(teacherId);
            List<Course> courses = teacherController.GetAllCourses();
            List<string> levelLanguageStr = new List<string>();

            foreach (Course course in courses)
            {
                if (teacher.CoursesId.Contains(course.Id))
                {
                    string languageLevel = $"{course.Language} {course.Level}";
                    if (!levelLanguageStr.Contains(languageLevel))
                        levelLanguageStr.Add(languageLevel);
                }
            }
            languageComboBox.ItemsSource = levelLanguageStr;
        }
        private void PickLanguageAndLevel()
        {
            Language lang = Domain.Model.Enums.Language.German;
            LanguageLevel lvl = LanguageLevel.A1;

            if (languageComboBox.SelectedItem != null)
            {
                string selectedLanguageAndLevel = (string)languageComboBox.SelectedItem;
                SetLanguageAndLevelToUpdate(selectedLanguageAndLevel);
            }
        }
        private void SetLanguageAndLevelToUpdate(string selectedLanguageAndLevel)
        {
            string[] parts = selectedLanguageAndLevel.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Language lang = Domain.Model.Enums.Language.German;
            LanguageLevel lvl = LanguageLevel.A1;
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
        private void SetCourseForExamTerm(Domain.Model.Enums.Language lang, Domain.Model.Enums.LanguageLevel lvl)
        {
            //TeacherDAO teacherDAO = new TeacherDAO();
            teacher = directorController.GetTeacherById(teacherId);
            //List<Course> courses = teacherDAO.GetAvailableCourses(teacher);
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
        private void PickDataFromDatePicker()
        {
            if (examDatePicker.SelectedDate.HasValue && !string.IsNullOrWhiteSpace(examTimeTextBox.Text))
            {
                DateTime startDate = examDatePicker.SelectedDate.Value.Date;
                DateTime startTime;
                if (DateTime.TryParseExact(examTimeTextBox.Text, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
                    ExamTerm.ExamDate = startDate.Add(startTime.TimeOfDay);
                else
                    MessageBox.Show("Please enter a valid start time (HH:mm).");
            }
            else
            {
                MessageBox.Show("Please select a valid start date and time.");
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            PickDataFromDatePicker();
            PickLanguageAndLevel();
            if (ExamTerm.IsValid)
            {
                examTermController.UpdateExamTerm(ExamTerm.ToExamTerm());
                Close();
            }
            else
            {
                MessageBox.Show("Exam Term can not be updated. Not all fields are valid.");
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
