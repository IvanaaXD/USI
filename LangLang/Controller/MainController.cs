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
        private ExamTermGradeController _examTermGradeController;
        private MailController _mailController;

        public MainController()
        {
            _studentController = new StudentsController();
            _teacherController = new TeacherController();

            _directorController = new DirectorController(new DirectorRepository());

            _courseController = new CourseController(new CourseRepository(), _teacherController);
            _examTermController = new ExamTermController(new ExamTermRepository(), _teacherController);

            _examTermGradeController = new ExamTermGradeController(new ExamTermGradeRepository());
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

        public ExamTermGradeController GetExamTermGradeController()
        {
            return _examTermGradeController;
        }

        public MailController GetMailController()
        {
            return _mailController;
        }
    }
}
