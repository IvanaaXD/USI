using LangLang.Controller;
using LangLang.Observer;
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



namespace LangLang.View.Director
{
    /// <summary>
    /// Interaction logic for WelcomePage.xaml
    /// </summary>
    public partial class WelcomePage : Window
    {
        int directorId;
        DirectorController directorController;
        public WelcomePage(int directorId, DirectorController directorController)
        {
            InitializeComponent();
            this.directorId = directorId;
            this.directorController = directorController;
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
