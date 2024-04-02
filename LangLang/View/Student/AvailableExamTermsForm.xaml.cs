using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model.Enums;
using LangLang.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace LangLang.View.Student
{
    /// <summary>
    /// Interaction logic for AvailableExamTermsForm.xaml
    /// </summary>
    public partial class AvailableExamTermsForm : Window, IObserver
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

        private int studentId { get; set; }
        private bool isSearchButtonClicked = false;


        public AvailableExamTermsForm(int studentId)
        {
            InitializeComponent();
            TableViewModel = new ViewModel();
            studentsController = new StudentsController();
            teacherController = new TeacherController();
            this.studentId = studentId;

            languageComboBox.ItemsSource = Enum.GetValues(typeof(Language));
            levelComboBox.ItemsSource = Enum.GetValues(typeof(LanguageLevel));

            DataContext = this;
            studentsController.Subscribe(this);
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
                List<ExamTerm> allFilteredExamTerms = teacherController.FindExamTermsByCriteria(selectedLanguage, selectedLevel, selectedStartDate);

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
