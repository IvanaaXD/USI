using System;
using System.Collections.Generic;
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
