using LangLang.Model;
using LangLang.Controller;
using LangLang.DTO;
using LangLang.Observer;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;

namespace LangLang.View.Teacher
{
    public partial class ViewCourseTable : Window, IObserver
    {
        public ObservableCollection<StudentDTO> Students { get; set; }
        public class ViewModel
        {
            public ObservableCollection<StudentDTO> Students { get; set; }

            public ViewModel()
            {
                Students = new ObservableCollection<StudentDTO>();
            }
        }
        readonly int teacherId;
        readonly TeacherController teacherController;
        readonly DirectorController directorController;
        public StudentDTO SelectedStudent { get; set; }
        public ViewModel TableViewModel { get; set; }

        public ViewCourseTable(int courseId, int teacherId, TeacherController teacherController, DirectorController directorController)
        {
            InitializeComponent();
            this.teacherId = teacherId;
            this.directorController = directorController;
            this.teacherController = teacherController;

            this.Students = Students;

            TableViewModel=new ViewModel();
            DataContext = this;
            teacherController.Subscribe(this);

            Update();
        }

        public void Update()
        {
            try
            {
                TableViewModel.Students.Clear();
                /*var students = GetCourseStudents();*/

                /*if (students != null)
                {
                    foreach (Model.Student student in students)
                        TableViewModel.Students.Add(new StudentDTO(student));
                }
                else
                {
                    MessageBox.Show("No students found.");
                }*/
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void PenaltyPoint_Click(object sender, RoutedEventArgs e)
        {

        }

        /* private List<Model.Student> GetCourseStudents()
         {

         }*/
    }
}
