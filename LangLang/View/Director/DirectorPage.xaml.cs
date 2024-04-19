using LangLang.Controller;
using System.Windows;

namespace LangLang.View.Director
{
    /// <summary>
    /// Interaction logic for WelcomePage.xaml
    /// </summary>
    public partial class DirectorPage : Window
    {
        readonly int directorId;
        readonly DirectorController directorController;

        public DirectorPage(int directorId, DirectorController directorController)
        {
            InitializeComponent();
            this.directorId = directorId;
            this.directorController = directorController;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void TeachersTable_Click(object sender, RoutedEventArgs e)
        {
            TeachersTable teachersTable = new TeachersTable();
            teachersTable.Show();
        }
    }
}
