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
using System.Windows.Navigation;
using System.Windows.Shapes;

using LangLang.View.Student;
using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model;
using LangLang.Observer;

namespace LangLang
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IObserver
    {
        public ObservableCollection<StudentDTO> Students { get; set; }
        public StudentDTO SelectedStudent { get; set; }
        private StudentsController studentsController { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Students = new ObservableCollection<StudentDTO>();
            studentsController = new StudentsController();
            studentsController.Subscribe(this);

            Update();
        }

        public void Update()
        {
            Students.Clear();
            /*foreach (Student teacher in studentController.GetAllStudents())
                Students.Add(new StudentDTO(teacher));*/

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            WelcomePage welcomePage = new WelcomePage(0, studentsController);

            welcomePage.Show();
            this.Close();
        }

        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            RegistrationForm regForm = new RegistrationForm(studentsController);

            regForm.Show();
        }
    }
}
