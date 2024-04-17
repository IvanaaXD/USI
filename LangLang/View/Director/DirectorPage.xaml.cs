using LangLang.Controller;
using System;
using System.Windows;

namespace LangLang.View.Director
{
    /// <summary>
    /// Interaction logic for WelcomePage.xaml
    /// </summary>
    public partial class DirectorPage : Window
    {
        int directorId;
        DirectorController directorController;
        public DirectorPage(int directorId, DirectorController directorController)
        {
            InitializeComponents();
            this.directorId = directorId;
            this.directorController = directorController;
        }

        private void InitializeComponents()
        {
            throw new NotImplementedException();
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            directorController.Delete(directorId);
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
