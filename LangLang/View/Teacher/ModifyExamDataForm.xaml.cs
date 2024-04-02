using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model.DAO;
using LangLang.Model;
using LangLang.Model.Enums;
using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Interaction logic for ModifyExamDataForm.xaml
    /// </summary>
    public partial class ModifyExamDataForm : Window
    {
        public ExamTermDTO ExamTerm { get; set; }

        private readonly TeacherController teacherController;

        public ModifyExamDataForm(int examId, TeacherController teacherController)
        {
            ExamTerm examTerm = teacherController.GetExamTermById(examId);
            ExamTerm = new ExamTermDTO(examTerm);
            DataContext = ExamTerm;
            
            
            InitializeComponent();
            this.teacherController = teacherController;

            TeacherDAO teacherDAO = new TeacherDAO();
            string languageAndLevel = teacherDAO.FindLanguageAndLevel(examTerm.CourseID);

            string[] parts = languageAndLevel.Split(',');
            languageAndLevel = parts[0].Trim() + " " + parts[1].Trim();
            
            languageComboBox.SelectedItem = languageAndLevel; 
            dpExamDate.SelectedDate = ExamTerm.ExamDate;
            txtExamTime.Text = ExamTerm.ExamDate.ToString("HH:mm"); //ExamTerm.ExamTime;
            txtMaxStudents.Text = ExamTerm.MaxStudents.ToString();

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
                        ExamTerm.CourseID = course.CourseID;
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
        private void btnSaveData_Click(object sender, RoutedEventArgs e)
        {
            PickDataFromDatePicker();
            PickLanguageAndLevel();
            if (ExamTerm.IsValid)
            {
                    teacherController.UpdateExamTerm(ExamTerm.ToExamTerm());
                    Close();   
            }
            else
            {
                MessageBox.Show("Exam Term can not be updated. Not all fields are valid.");
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
