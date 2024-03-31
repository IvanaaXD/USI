using System;
using System.Windows;
using LangLang.Model;
using System.Collections.ObjectModel;
using LangLang.Observer;
using LangLang.DTO;
using LangLang.Controller;

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

        public ExamTermsTable()
        {
            InitializeComponent();
            TableViewModel = new ViewModel();
            teacherController = new TeacherController();
            DataContext = this;
            teacherController.Subscribe(this);
            Update();
        }

        public void Update()
        {
            try
            {
                TableViewModel.ExamTerms.Clear();
                var examTerms = teacherController.GetAllExamTerms();
                if (examTerms != null)
                {
                    foreach (ExamTerm examTerm in examTerms)
                        TableViewModel.ExamTerms.Add(new ExamTermDTO(examTerm));
                }
                else
                {
                    MessageBox.Show("No exam terms found."); // Display message if no exam terms found
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}"); // Display error message
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedExamTerm == null)
            {
                MessageBox.Show("Please choose an exam term to cancel!");
            }
            else
            {
                if (DateTime.Now.AddDays(14) > SelectedExamTerm.ExamTime)
                    MessageBox.Show("Cannot cancel an exam that starts in less than a 2 week.");
                else
                    teacherController.RemoveExamTerm(SelectedExamTerm.ExamID);
            }
        }
    }
}
