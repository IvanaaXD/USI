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
        private CourseGradeController _courseGradeController;
        private ExamTermController _examTermController;
        private ExamTermGradeController _examTermGradeController;
        private MailController _mailController;
        private ReportController _reportController;

        public MainController()
        {
            _studentController = new StudentsController();
            _teacherController = new TeacherController();
            _reportController = new ReportController();

            _directorController = new DirectorController();

            _courseController = new CourseController();
            _examTermController = new ExamTermController();

            _courseGradeController = new CourseGradeController();
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
        public CourseGradeController GetCourseGradeController()
        {
            return _courseGradeController;
        }

        public ExamTermGradeController GetExamTermGradeController()
        {
            return _examTermGradeController;
        }

        public MailController GetMailController()
        {
            return _mailController;
        }

        public ReportController GetReportController()
        {
            return _reportController;
        }
    }
}
