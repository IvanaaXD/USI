using LangLang.Controller;
using LangLang.DTO;
using System.ComponentModel;
using System.Windows;
using LangLang.Domain.Model;
using System.Collections.ObjectModel;

namespace LangLang.View.Director
{
    public partial class ChooseTeacherView : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private TeacherDTO? _selectedTeacher;
        public TeacherDTO? SelectedTeacher
        {
            get { return _selectedTeacher; }
            set
            {
                _selectedTeacher = value;
                OnPropertyChanged(nameof(SelectedTeacher));
            }
        }

        public ObservableCollection<TeacherDTO> AvailableTeachers { get; }

        private readonly DirectorController? _directorController;
        private readonly CourseController? _courseController;

        private readonly Course course;

        public ChooseTeacherView(Course course, int teacherId)
        {
            InitializeComponent();
            DataContext = this;
            this.course = course;
            _directorController = Injector.CreateInstance<DirectorController>();
            _courseController = Injector.CreateInstance<CourseController>();

            AvailableTeachers = new ObservableCollection<TeacherDTO>();

            var availableTeachers = _directorController.GetAvailableTeachers(course);
            foreach (var teacher in availableTeachers)
                if (teacher.Id != teacherId)
                    AvailableTeachers.Add(new TeacherDTO(teacher));
        }

        private void Choose_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTeacher != null)
            {
                var teacher = SelectedTeacher.ToTeacher();
                teacher.CoursesId.Add(course.Id);
                _directorController?.Update(teacher);
                Close();
            }
            else
            {
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TeachersListBox_SelectionChanged(object sender, RoutedEventArgs e) {}

    }
}
