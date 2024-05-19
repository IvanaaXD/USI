using LangLang.Controller;
using LangLang.DTO;
using LangLang.Domain.Model.Enums;
using LangLang.Domain.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace LangLang.View.Student
{
    /// <summary>
    /// Interaction logic for ExamTermsPage.xaml
    /// </summary>
    public partial class ExamTermsPage : Window, IObserver
    {
        public ObservableCollection<ExamTermDTO> ExamTerms { get; set; }
        public class ViewModel
        {
            public ObservableCollection<ExamTermDTO> ExamTerms { get; set; }
            public ObservableCollection<ExamTermDTO> AvailableExamTerms { get; set; }
            public ObservableCollection<ExamTermDTO> RegisteredExamTerms { get; set; }
            public ObservableCollection<ExamTermDTO> CompletedExamTerms { get; set; }

            public ViewModel()
            {
                ExamTerms = new ObservableCollection<ExamTermDTO>();
                AvailableExamTerms = new ObservableCollection<ExamTermDTO>();
                RegisteredExamTerms = new ObservableCollection<ExamTermDTO>();
                CompletedExamTerms = new ObservableCollection<ExamTermDTO>();
            }
        }
        public ViewModel TableViewModel { get; set; }
        public ExamTermDTO SelectedAvailableExamTerm { get; set; }
        public ExamTermDTO SelectedRegisteredExamTerm { get; set; }
        public ExamTermDTO SelectedCompletedExamTerm { get; set; }
        private StudentsController studentController { get; set; }
        private TeacherController teacherController { get; set; }

        private int studentId { get; set; }
        private bool isSearchButtonClicked = false;

        public ExamTermsPage(int studentId, MainController mainController)
        {
            InitializeComponent();
            TableViewModel = new ViewModel();
            studentController = mainController.GetStudentController();
            teacherController = mainController.GetTeacherController();
            this.studentId = studentId;

            languageComboBox.ItemsSource = Enum.GetValues(typeof(Language));
            levelComboBox.ItemsSource = Enum.GetValues(typeof(LanguageLevel));
            languageComboBoxRegistered.ItemsSource = Enum.GetValues(typeof(Language));
            levelComboBoxRegistered.ItemsSource = Enum.GetValues(typeof(LanguageLevel));
            languageComboBoxCompleted.ItemsSource = Enum.GetValues(typeof(Language));
            levelComboBoxCompleted.ItemsSource = Enum.GetValues(typeof(LanguageLevel));

            DataContext = this;
            //teacherController.Subscribe(this);
            Update();
        }
        public void Update()
        {
            try
            {
                TableViewModel.AvailableExamTerms.Clear();
              
                var examTerms = GetFilteredAvailableExamTerms();

                if (examTerms != null)
                {
                    foreach (ExamTerm examTerm in examTerms)
                        TableViewModel.AvailableExamTerms.Add(new ExamTermDTO(examTerm));
                }
                else
                {
                    MessageBox.Show("No examTerms found.");
                }
                UpdateRegisteredExamsTable();
                UpdateCompletedExamsTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        public void UpdateRegisteredExamsTable()
        {
            try
            {
                TableViewModel.RegisteredExamTerms.Clear();

                var examTerms = GetFilteredRegisteredExamTerms();   

                if (examTerms != null)
                {
                    foreach (ExamTerm examTerm in examTerms)
                        TableViewModel.RegisteredExamTerms.Add(new ExamTermDTO(examTerm));
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

        public void UpdateCompletedExamsTable()
        {
            try
            {
                TableViewModel.CompletedExamTerms.Clear();

                var examTerms = GetFilteredCompletedExamTerms();

                if (examTerms != null)
                {
                    foreach (ExamTerm examTerm in examTerms)
                    {                        
                        TableViewModel.CompletedExamTerms.Add(new ExamTermDTO(examTerm,studentId));
                    }
                        
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

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void SignUp_Click(object sender, EventArgs e)
        {
           bool registered = studentController.RegisterForExam(studentId, SelectedAvailableExamTerm.ExamID);
            if (registered) 
            {
                MessageBox.Show("You have successfully registered for the exam.");
                Update();
            }
            else
            {
                MessageBox.Show("It is not possible to register for the given exam.");

            }
        }
        private void SignOut_Click(object sender, EventArgs e)
        {
            bool unregistered = studentController.CancelExamRegistration(studentId, SelectedRegisteredExamTerm.ExamID);
            if (unregistered)
            {
                MessageBox.Show("You have successfully checked out for the exam.");
                Update();
            }
            else
            {
                MessageBox.Show("It is not possible to unregister from the given exam.");

            }
        }
        private void ViewExam_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCompletedExamTerm == null)
                MessageBox.Show("Please choose an exam term to view!");
            else
            {
                ExamTerm? examTerm = teacherController.GetExamTermById(SelectedCompletedExamTerm.ExamID);
                Domain.Model.Student? student = studentController.GetStudentById(this.studentId);
                ExamTermStudentView? examTermView = new ExamTermStudentView(examTerm, student, teacherController, studentController);
                examTermView.Show();
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            Update();
            isSearchButtonClicked = true;
        }
        private void ResetExam_Click(object sender, EventArgs e)
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

            languageComboBoxRegistered.SelectedItem = null;
            levelComboBoxRegistered.SelectedItem = null;
            startDateDatePickerRegistered.SelectedDate = null;
            languageComboBoxCompleted.SelectedItem = null;
            levelComboBoxCompleted.SelectedItem = null;
            startDateDatePickerCompleted.SelectedDate = null;
        }

        private List<ExamTerm> GetFilteredAvailableExamTerms()
        {
            Language? selectedLanguage = (Language?)languageComboBox.SelectedItem;
            LanguageLevel? selectedLevel = (LanguageLevel?)levelComboBox.SelectedItem;
            DateTime? selectedStartDate = startDateDatePicker.SelectedDate;

            List<ExamTerm> studentsAvailableExamTerms = studentController.GetAvailableExamTerms(studentId);
            List<ExamTerm> finalExamTerms = new List<ExamTerm>();

            if (isSearchButtonClicked)
            {
                List<ExamTerm> allFilteredExamTerms = teacherController.FindExamTermsByCriteria(selectedLanguage, selectedLevel, selectedStartDate);
                finalExamTerms = GetFinalExamTerms(allFilteredExamTerms, studentsAvailableExamTerms);

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

        private List<ExamTerm> GetFilteredRegisteredExamTerms()
        {
            Language? selectedLanguage = (Language?)languageComboBox.SelectedItem;
            LanguageLevel? selectedLevel = (LanguageLevel?)levelComboBox.SelectedItem;
            DateTime? selectedStartDate = startDateDatePicker.SelectedDate;

            List<ExamTerm> studentsRegisteredExamTerms = studentController.GetRegisteredExamTerms(studentId);
            List<ExamTerm> finalExamTerms = new List<ExamTerm>();

            if (isSearchButtonClicked)
            {
                List<ExamTerm> allFilteredExamTerms = teacherController.FindExamTermsByCriteria(selectedLanguage, selectedLevel, selectedStartDate);           
                finalExamTerms = GetFinalExamTerms(allFilteredExamTerms, studentsRegisteredExamTerms);
            }
            else
            {
                foreach (ExamTerm studentExamTerm in studentsRegisteredExamTerms)
                {
                    finalExamTerms.Add(studentExamTerm);
                }
            }
            return finalExamTerms;
        }

        private List<ExamTerm> GetFilteredCompletedExamTerms()
        {
            Language? selectedLanguage = (Language?)languageComboBox.SelectedItem;
            LanguageLevel? selectedLevel = (LanguageLevel?)levelComboBox.SelectedItem;
            DateTime? selectedStartDate = startDateDatePicker.SelectedDate;

            List<ExamTerm> studentsCompletedExamTerms = studentController.GetCompletedExamTerms(studentId);
            List<ExamTerm> finalExamTerms = new List<ExamTerm>();

            if (isSearchButtonClicked)
            {
                List<ExamTerm> allFilteredExamTerms = teacherController.FindExamTermsByCriteria(selectedLanguage, selectedLevel, selectedStartDate);
                finalExamTerms = GetFinalExamTerms(allFilteredExamTerms, studentsCompletedExamTerms);
            }
            else
            {
                foreach (ExamTerm studentExamTerm in studentsCompletedExamTerms)
                {
                    finalExamTerms.Add(studentExamTerm);
                }
            }
            return finalExamTerms;
        }
        private List<ExamTerm> GetFinalExamTerms(List<ExamTerm> allFilteredExamTerms, List<ExamTerm> studentsExamTerms)
        {
            List<ExamTerm> finalExamTerms = new List<ExamTerm>();
            foreach (ExamTerm examTerm in allFilteredExamTerms)
            {
                foreach (ExamTerm studentExamTerm in studentsExamTerms)
                {
                    if (studentExamTerm.ExamID == examTerm.ExamID && !finalExamTerms.Contains(examTerm))
                    {
                        finalExamTerms.Add(examTerm);
                    }
                }
            }
            return finalExamTerms;
        }
    }
}