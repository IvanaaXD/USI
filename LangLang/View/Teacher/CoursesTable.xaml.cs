using LangLang.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using LangLang.View;
using LangLang.Observer;
using LangLang.DTO;
using LangLang.Controller;

namespace LangLang.View.Teacher
{
    public partial class CoursesTable : Window, IObserver
    {
        // Nested ViewModel class
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
        public TeacherController teacherController { get; set; }

        public CoursesTable()
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
                TableViewModel.Courses.Clear();
                var courses = teacherController.GetAllCourses();
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
    }
}
