using LangLang.Controller;
using LangLang.DTO;
using LangLang.Domain.Model;
using LangLang.Domain.Model.Enums;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace LangLang.View.Student
{
    /// <summary>
    /// Interaction logic for AvailableExamTermsTable.xaml
    /// </summary>
    public partial class AvailableExamTermsTable : Window, IObserver
    {
        public class ViewModel
        {
            public ObservableCollection<ExamTermDTO> ExamTerms { get; set; }

            public ViewModel()
            {
                ExamTerms = new ObservableCollection<ExamTermDTO>();
            }

        }

        public ViewModel TableViewModel { get; set; }
        public ExamTermDTO SelectedExamTerm { get; set; }
        private StudentsController studentsController { get; set; }
        private TeacherController teacherController { get; set; }
        private ExamTermController examTermController { get; set; }

        private int studentId { get; set; }
        private bool isSearchButtonClicked = false;

        public AvailableExamTermsTable(int studentId)
        {
            InitializeComponent();
            TableViewModel = new ViewModel();
            studentsController = Injector.CreateInstance<StudentsController>();
            teacherController = Injector.CreateInstance<TeacherController>();
            examTermController = Injector.CreateInstance<ExamTermController>();
            this.studentId = studentId;

            languageComboBox.ItemsSource = Enum.GetValues(typeof(Language));
            levelComboBox.ItemsSource = Enum.GetValues(typeof(LanguageLevel));

            DataContext = this;
            teacherController.Subscribe(this);
            Update();
        }

        public void Update()
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
                    MessageBox.Show("No examTerms found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void btnSingUp_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            Update();
            isSearchButtonClicked = true;
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            isSearchButtonClicked = false;
            Update();
            ResetSearchElements();
        }

        private void ResetSearchElements()
        {
            languageComboBox.SelectedItem = null;
            levelComboBox.SelectedItem = null;
            startDateDatePicker.SelectedDate = null;
        }

        private List<ExamTerm> GetFilteredExamTerms()
        {
            Language? selectedLanguage = (Language?)languageComboBox.SelectedItem;
            LanguageLevel? selectedLevel = (LanguageLevel?)levelComboBox.SelectedItem;
            DateTime? selectedStartDate = startDateDatePicker.SelectedDate;
           
            List<ExamTerm> studentsAvailableExamTerms = studentsController.GetAvailableExamTerms(studentId);
            List<ExamTerm> finalExamTerms = new List<ExamTerm>();

            if (isSearchButtonClicked)
            {
                List<ExamTerm> allFilteredExamTerms = examTermController.FindExamTermsByCriteria(selectedLanguage, selectedLevel, selectedStartDate);

                foreach (ExamTerm examTerm in allFilteredExamTerms)
                {
                    foreach (ExamTerm studentExamTerm in studentsAvailableExamTerms)
                    {
                        if (studentExamTerm.ExamID == examTerm.ExamID && !finalExamTerms.Contains(examTerm))
                        {
                            finalExamTerms.Add(examTerm);
                        }
                    }
                }
            }
            else
            {
                foreach (ExamTerm studentExamTerm in studentsAvailableExamTerms)
                {
                    finalExamTerms.Add(studentExamTerm);
                }
            }
            return finalExamTerms;
        }
    }
}
