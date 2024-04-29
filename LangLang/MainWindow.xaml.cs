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

using LangLang.View.Director;
using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model;
<<<<<<< Updated upstream
using LangLang.Observer;
=======
using LangLang.View.Teacher;
using LangLang.View.Student;
>>>>>>> Stashed changes

namespace LangLang
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IObserver
    {
        public ObservableCollection<TeacherDTO> Teachers { get; set; }
        public TeacherDTO SelectedTeacher { get; set; }
        private DirectorController directorController { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Teachers = new ObservableCollection<TeacherDTO>();
            directorController = new DirectorController();
            directorController.Subscribe(this);

            Update();
        }

        public void Update()
        {
            Teachers.Clear();
            /*foreach (Teacher teacher in directorController.GetAllTeachers())
                Teachers.Add(new TeacherDTO(teacher));*/

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            WelcomePage welcomePage = new WelcomePage(0, directorController);

            welcomePage.Show();
            this.Close();
        }

        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            RegistrationForm regForm = new RegistrationForm(directorController);

            regForm.Show();
        }
    }
}
