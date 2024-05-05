
namespace LangLang.Controller
{
    public class MainController
    {
        private StudentsController _studentController;
        private TeacherController _teacherController;
        private DirectorController _directorController;

        public MainController()
        {
            _studentController = new StudentsController();
            _teacherController = new TeacherController();
            _directorController = new DirectorController();
        }

        public StudentsController GetStudentController()
        {
            return _studentController;
        }

        public TeacherController GetTeacherController()
        {
            return _teacherController;
        }

        public DirectorController GetDirectorController()
        {
            return _directorController;
        }
    }
}
