using System.Windows;

namespace LangLang.View.Student
{
    /// <summary>
    /// Interaction logic for StudentCoursesForm.xaml
    /// </summary>
    public partial class StudentCoursesForm : Window
    {
        public StudentCoursesForm(int selectedTabIndex)
        {
            InitializeComponent();
            //tabControl.SelectedIndex = selectedTabIndex;
        }
    }
}
