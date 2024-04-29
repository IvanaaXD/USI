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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        private int teacherId;
        public CreateExamForm(TeacherController teacherController, int teacherId)
        {
            InitializeComponent();

            DataContext = this;
            ExamTerm = new ExamTermDTO();
            this.teacherController = teacherController;
            this.teacherId = teacherId;
            ExamTerm.ExamDate = DateTime.Now; // u dto izmeni examDate i time

            ExamTerm.ExamTime = "10:00";
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
                
                TeacherDAO teacherDAO = new TeacherDAO();
                List<Course> courses = teacherDAO.GetAllCourses();

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

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {

            PickDataFromDatePicker();
            PickLanguageAndLevel();

            if (ExamTerm.IsValid)
            {
                teacherController.AddExamTerm(ExamTerm.ToExamTerm());
                Close();
            }
            else
            {
                MessageBox.Show("Exam Term cannot be created. Not all fields are valid.");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
