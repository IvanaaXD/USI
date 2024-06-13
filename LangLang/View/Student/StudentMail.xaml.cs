using LangLang.Controller;
using LangLang.Domain.Model;
using LangLang.DTO;
using LangLang.Observer;
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
using System.Windows.Shapes;

namespace LangLang.View.Student
{
    /// <summary>
    /// Interaction logic for StudentMail.xaml
    /// </summary>
    public partial class StudentMail : Window, IObserver
    {
        public class ViewModel
        {
            public ObservableCollection<MailDTO> Mails { get; set; }

            public ViewModel()
            {
                Mails = new ObservableCollection<MailDTO>();
            }
        }
        public ViewModel SentMailsTableViewModel { get; set; }
        public ViewModel ReceivedMailsTableViewModel { get; set; }
        public MailDTO SelectedMail { get; set; }
        private MailController mailController;
        private Domain.Model.Student student;
        
        public StudentMail(Domain.Model.Student student)
        {
            InitializeComponent();

            SentMailsTableViewModel = new ViewModel();
            ReceivedMailsTableViewModel = new ViewModel();
            SelectedMail = new MailDTO();

            mailController = Injector.CreateInstance<MailController>();
            this.student = student;

            DataContext = this;
            Update();
        }

        public void Update()
        {
            try
            {
                SentMailsTableViewModel.Mails.Clear();
                ReceivedMailsTableViewModel.Mails.Clear();

                var receivedMails = mailController.GetReceivedMails(student);
                var sentMails = mailController.GetSentMails(student);

                AddSentMailsToTable(sentMails);
                AddReceivedMailsToTable(receivedMails);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        private void AddSentMailsToTable(List<Mail> mails)
        {
            if (mails != null)
                foreach (Mail mail in mails)
                {
                    SentMailsTableViewModel.Mails.Add(new MailDTO(mail));
                }
        }
        private void AddReceivedMailsToTable(List<Mail> mails)
        {
            if (mails != null)
                foreach (Mail mail in mails)
                {
                    ReceivedMailsTableViewModel.Mails.Add(new MailDTO(mail));
                }
        }
        private void ReceivedMailDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedMail != null)
            {
                receivedMailSenderTextBlock.Text = SelectedMail.Sender;
                receivedMailDateTextBlock.Text = SelectedMail.DateOfMessage.ToString("dd-MM-yyyy HH:mm");
                receivedMailTypeTextBlock.Text = SelectedMail.TypeOfMessage.ToString();
                receivedMailMessageTextBlock.Text = SelectedMail.Message;
            }
        }
        private void SentMailDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedMail != null)
            {
                sentMailSenderTextBlock.Text = SelectedMail.Receiver;
                sentMailDateTextBlock.Text = SelectedMail.DateOfMessage.ToString("dd-MM-yyyy HH:mm");
                sentMailTypeTextBlock.Text = SelectedMail.TypeOfMessage.ToString();
                sentMailMessageTextBlock.Text = SelectedMail.Message;

            }
        }
    }
}
