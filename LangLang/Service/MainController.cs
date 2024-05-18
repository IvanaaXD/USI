namespace LangLang.Controller
{
    public class MainController
    {
        private StudentsController _studentController;
        private TeacherController _teacherController;
        private DirectorService _directorController;
        private CourseController _courseController;

        public MainController()
        {
            _studentController = new StudentsController();
            _teacherController = new TeacherController();
            _directorController = new DirectorService();
            _courseController = new CourseController(_teacherController);
        }

        public StudentsController GetStudentController()
        {
            return _studentController;
        }

        public TeacherController GetTeacherController()
        {
            return _teacherController;
        }

        public DirectorService GetDirectorController()
        {
            return _directorController;
        }
        public CourseController GetCourseController()
        {
            return _courseController;
        }
    }
}
