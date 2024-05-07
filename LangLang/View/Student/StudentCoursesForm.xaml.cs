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
