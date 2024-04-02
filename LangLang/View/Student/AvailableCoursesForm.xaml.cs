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

using LangLang.DTO;
using LangLang.Controller;
using LangLang.Model;
using LangLang.Observer;

namespace LangLang.View.Student
{
    /// <summary>
    /// Interaction logic for AvailableCoursesForm.xaml
    /// </summary>
    public partial class AvailableCoursesForm : Window, IObserver
    {
        public class ViewModel
        {
            public ObservableCollection<CourseDTO> Courses { get; set; }

            public ViewModel()
            {
                Courses = new ObservableCollection<CourseDTO>();
            }


        }

        public ViewModel TableViewModel { get; set; }
        public CourseDTO SelectedCourse { get; set; }
        private StudentsController studentsController { get; set; }

        private int studentId { get; set; }

        public AvailableCoursesForm(int studentId)
        {
            InitializeComponent();
            TableViewModel = new ViewModel();
            studentsController = new StudentsController();
            this.studentId = studentId;
            DataContext = this;
            studentsController.Subscribe(this);
            Update();
        }

        public void Update()
        {
            try
            {
                TableViewModel.Courses.Clear();
                var courses = studentsController.GetAvailableCourses(studentId);
                if (courses != null)
                {
                    foreach (Course course in courses)
                        TableViewModel.Courses.Add(new CourseDTO(course));
                }
                else
                {
                    MessageBox.Show("No courses found."); // Display message if no courses found
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}"); // Display error message
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
