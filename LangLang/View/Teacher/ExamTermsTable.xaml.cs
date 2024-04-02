using System;
using System.Windows;
using LangLang.Model;
using System.Collections.ObjectModel;
using LangLang.Observer;
using LangLang.DTO;
using LangLang.Controller;
using LangLang.Model.Enums;
using System.Collections.Generic;
using LangLang.Model.DAO;

namespace LangLang.View.Teacher
{
    public partial class ExamTermsTable : Window, IObserver
    {
        // Nested ViewModel class
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
        public TeacherController teacherController { get; set; }
        public int teacherId { get; set; }

        private bool isSearchButtonClicked = false;
        DirectorController directorController;
        public ExamTermsTable(int teacherId, DirectorController directorController)
        {
            InitializeComponent();
            TableViewModel = new ViewModel();
            teacherController = new TeacherController();
            this.teacherId = teacherId;
            languageComboBox.ItemsSource = Enum.GetValues(typeof(Language));
            levelComboBox.ItemsSource = Enum.GetValues(typeof(LanguageLevel));
            DataContext = this;
            this.directorController = directorController;
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
                    MessageBox.Show("No exam terms found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            CreateExamForm examTable = new CreateExamForm(teacherController, teacherId);
            examTable.Show();
        }
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedExamTerm == null)
            {
                MessageBox.Show("Please choose exam to update!");
            }
            else
            {
                ModifyExamDataForm modifyDataForm = new ModifyExamDataForm(SelectedExamTerm.ExamID, teacherController);
                modifyDataForm.Show();
                modifyDataForm.Activate();
            }
        }
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            isSearchButtonClicked = false;
            Update();
            ResetSearchElements();
        }

        private void ResetSearchElements()
        {
            languageComboBox.SelectedItem = null;
            levelComboBox.SelectedItem = null;
            examDatePicker.SelectedDate = null;
        }

        private void btnCourseTable(object sender, RoutedEventArgs e)
        {
            CoursesTable courseT = new CoursesTable(teacherId, directorController);
            courseT.Show();
        }


        private void Search_Click(object sender, RoutedEventArgs e)
        {
            Update();
            isSearchButtonClicked = true;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedExamTerm == null)
            {
                MessageBox.Show("Please choose an exam term to cancel!");
            }
            else
            {
                if (DateTime.Now.AddDays(14) > SelectedExamTerm.ExamDate)
                    MessageBox.Show("Cannot cancel an exam that starts in less than a 2 week.");
                else
                    teacherController.DeleteExamTerm(SelectedExamTerm.ExamID);
            }
        }
        private List<ExamTerm> GetFilteredExamTerms()
        {
            Language? selectedLanguage = (Language?)languageComboBox.SelectedItem;
            LanguageLevel? selectedLevel = (LanguageLevel?)levelComboBox.SelectedItem;
            DateTime? selectedStartDate = examDatePicker.SelectedDate;

            List<ExamTerm> finalExamTerms = teacherController.GetAllExamTerms();

            if (isSearchButtonClicked)
            {
                TeacherDAO teacherDAO = new TeacherDAO();
                finalExamTerms = teacherDAO.FindExamTermsByCriteria(selectedLanguage, selectedLevel, selectedStartDate);
            }
            return finalExamTerms;
        }
    }
}
