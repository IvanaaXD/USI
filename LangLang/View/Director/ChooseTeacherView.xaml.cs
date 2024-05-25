using LangLang.Observer;
using System.Windows;

namespace LangLang.View.Director
{
    public partial class ChooseTeacherView : Window, IObserver
    {
        public ChooseTeacherView() { }

        public void Update()
        {
            throw new System.NotImplementedException();
        }

        private void Choose_Click(object sender, RoutedEventArgs e)
        { 
        }

        private void TeachersListBox_SelectionChanged(object sender, RoutedEventArgs e) { }

    }
}
