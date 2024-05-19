using LangLang.Controller;
using LangLang.DTO;
using LangLang.Domain.Model;
using LangLang.Repository;
using System.Windows;
using LangLang.Domain.IRepository;

namespace LangLang.View.Student
{
    /// <summary>
    /// Interaction logic for GradeTeacher.xaml
    /// </summary>
    public partial class GradeTeacher : Window
    {
        private StudentGradeDTO teacherGrade { get; set; }
        private int studentId, courseId, teacherId;
        private TeacherController teacherController;
        private StudentsController studentController;
        private DirectorController directorService;

        public GradeTeacher(int studentId, int courseId, MainController mainController)
        {
            InitializeComponent();

            teacherController = new TeacherController();
            studentController = new StudentsController();
            IDirectorRepository directorRepository = new DirectorRepository();
            directorService = new DirectorController(directorRepository);

            this.studentId = studentId;
            this.courseId = courseId;

            InitializeDTO();

            completedCourseName.Text = GetCourseName(courseId);

            DataContext = teacherGrade;

        }

        private void InitializeDTO()
        {
            Domain.Model.Teacher teacher = directorService.GetTeacherByCourse(courseId);
            this.teacherId = teacher.Id;
            teacherGrade = new StudentGradeDTO(teacher);
        }

        public void GradeStudent_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(gradeValueTextBox.Text))
            {
                teacherGrade.Value = int.Parse(gradeValueTextBox.Text);
                if (teacherGrade.IsValid)
                {
                    teacherGrade.TeacherId = teacherId;
                    teacherGrade.CourseId = courseId;
                    teacherGrade.StudentId = studentId;
                    studentController.GradeStudentCourse(teacherGrade.ToStudentGrade());
                    Close();
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private string GetCourseName(int courseId)
        {
            Course course = teacherController.GetCourseById(courseId);
            return course.Language.ToString() + " " + course.Level.ToString();
        }
    }
}
