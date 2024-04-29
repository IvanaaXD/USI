using LangLang.Controller;
using LangLang.DTO;
using LangLang.Model;
using LangLang.Model.Enums;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LangLang.View.Teacher
{
    public partial class ExamTermView : Window, IObserver
    {
        public ObservableCollection<TeacherDTO> Teachers { get; set; }

        public class ViewModel
        {
            public ObservableCollection<TeacherDTO> Teachers { get; set; }

            public ViewModel()
            {
                Teachers = new ObservableCollection<TeacherDTO>();
            }
        }
        readonly ExamTerm examTerm;
        readonly Model.Teacher teacher;
        readonly TeacherController teacherController;
        public TeacherDTO SelectedExamTerm { get; set; }

        public ViewModel TableViewModel { get; set; }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public ExamTermView(ExamTerm examTerm, Model.Teacher teacher, TeacherController teacherController)
        {
            InitializeComponent();
            this.examTerm = examTerm;

            this.teacherController = teacherController;
            this.Teachers = Teachers;

            this.teacher = teacher;

            TableViewModel = new ViewModel();
            teacherController = new TeacherController();

            DataContext = this;

            teacherController.Subscribe(this);  
            Update();

            if (examTerm.Confirmed)
            {
                Confirm.Visibility = Visibility.Collapsed;
            }
        }

        private void ConfirmExamTerm_Click(object sender, RoutedEventArgs e)
        {
            teacherController.ConfirmExamTerm(this.examTerm.ExamID);
            MessageBox.Show("ExamTerm confirmed.");
            Confirm.Visibility = Visibility.Collapsed;
        }
    }
}
