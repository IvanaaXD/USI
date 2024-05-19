using LangLang.Repository;
using LangLang.Domain.IRepository;

namespace LangLang.Controller
{
    public class MainController
    {
        private StudentsController _studentController;
        private TeacherController _teacherController;
        private DirectorController _directorController;
        private CourseController _courseController;
        private ExamTermController _examTermController;
        private MailController _mailController;

        public MainController()
        {
            _studentController = new StudentsController();
            _teacherController = new TeacherController();

            IDirectorRepository directorRepository = new DirectorRepository();
            _directorController = new DirectorController(directorRepository);

            ICourseRepository courseRepository = new CourseRepository();
            _courseController = new CourseController(courseRepository, _teacherController);

            IExamTermGradeRepository examTermGradeRepository = new ExamTermGradeRepository();
            _examTermController = new ExamTermController(_teacherController, examTermGradeRepository);
            _mailController = new MailController();
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
        public CourseController GetCourseController()
        {
            return _courseController;
        }

        public ExamTermController GetExamTermController()
        {
            return _examTermController;
        }
        public MailController GetMailController()
        {
            return _mailController;
        }
    }
}
