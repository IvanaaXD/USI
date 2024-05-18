using LangLang.Repository;
using LangLang.Domain.IRepository;

namespace LangLang.Controller
{
    public class MainController
    {
        private StudentsController _studentController;
        private TeacherController _teacherController;
        private DirectorService _directorService;
        private CourseController _courseController;

        public MainController()
        {
            _studentController = new StudentsController();
            _teacherController = new TeacherController();

            IDirectorRepository directorRepository = new DirectorRepository();
            _directorService = new DirectorService(directorRepository);

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
            return _directorService;
        }
        public CourseController GetCourseController()
        {
            return _courseController;
        }
    }
}
