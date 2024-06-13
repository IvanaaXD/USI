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

        public UpdateExamForm(int teacherId, int examId)
        {
            this.directorController = Injector.CreateInstance<DirectorController>();
            this.teacherController = Injector.CreateInstance<TeacherController>();
            this.examTermController = Injector.CreateInstance<ExamTermController>();

            teacher = directorController.GetById(teacherId);
            ExamTerm examTerm = teacherController.GetExamTermById(examId);
            ExamTerm = new ExamTermDTO(examTerm, teacher);
            DataContext = ExamTerm;

            InitializeComponent();
            this.teacherId = teacherId;
            this.examId = examId;
            Teacher = new TeacherDTO(directorController.GetById(teacherId));

            SetInitialLanguageAndLevel(ExamTerm);
            FillLanguageAndLevelCombobox();

            examDatePicker.SelectedDate = ExamTerm.ExamDate;
            examTimeTextBox.Text = ExamTerm.ExamDate.ToString("HH:mm");
            maxStudentsTextBox.Text = ExamTerm.MaxStudents.ToString();
        }
        private void SetInitialLanguageAndLevel(ExamTermDTO ExamTerm)
        {
            string languageAndLevel = ExamTerm.Language+ " "+ ExamTerm.Level;
            languageComboBox.SelectedItem = languageAndLevel;
        }
        private void FillLanguageAndLevelCombobox()
        {
            teacher = directorController.GetById(teacherId);
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
                ExamTerm.Language = lang;
                ExamTerm.Level = lvl;
            }
            else
            {
                MessageBox.Show("Invalid language and level format.");
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
                examTermController.Update(ExamTerm.ToExamTerm());
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
