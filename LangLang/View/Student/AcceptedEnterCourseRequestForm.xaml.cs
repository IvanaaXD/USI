using System.Windows;

namespace LangLang.View.Student
{
    /// <summary>
    /// Interaction logic for AcceptedEnterCourseRequestForm.xaml
    /// </summary>
    public partial class AcceptedEnterCourseRequestForm : Window
    {

        public AcceptedEnterCourseRequestForm(string courseName)
        {
            InitializeComponent();
            activeCourseName.Text = courseName;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
