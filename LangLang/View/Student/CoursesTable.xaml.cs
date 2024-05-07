using LangLang.DTO;
using LangLang.Model.Enums;
using LangLang.Observer;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
namespace LangLang.View.Student
{
    /// <summary>
    /// Interaction logic for CoursesTable.xaml
    /// </summary>
    public partial class CoursesTable : UserControl
    {
        public event EventHandler SearchButtonClicked;

        public CoursesTable()
        {
            InitializeComponent();

            languageComboBox.ItemsSource = Enum.GetValues(typeof(Language));
            levelComboBox.ItemsSource = Enum.GetValues(typeof(LanguageLevel));
        }

        private void SearchButtonEvent_Click(object sender, RoutedEventArgs e)
        {
            SearchButtonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
