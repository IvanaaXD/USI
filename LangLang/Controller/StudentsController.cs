using System.Collections.Generic;
using LangLang.Repository;
using LangLang.Observer;
using LangLang.Domain.Model;
using LangLang.Domain.IRepository;
using System;

namespace LangLang.Controller
{
    public class StudentsController : Subject
    {
        private readonly IStudentRepository _students;
        private readonly CourseController courseController;
        private readonly ExamTermController examTermController;
        private readonly IStudentGradeRepository _studentGrades;

        public StudentsController()
        {
            _students = Injector.CreateInstance<IStudentRepository>();
            courseController = Injector.CreateInstance<CourseController>();
            examTermController = Injector.CreateInstance<ExamTermController>();
            _studentGrades = Injector.CreateInstance<IStudentGradeRepository>();
        }

        public void Add(Student student)
        {
            _students.AddStudent(student);
        }
        public void Delete(int studentId)
        {
            Student student = _students.RemoveStudent(studentId);
            DeleteStudentCoursesAndExams(student);
            NotifyObservers();
        }
        public void Update(Student student)
        {
            _students.UpdateStudent(student);
        }
        public void Subscribe(IObserver observer)
        {
            _students.Subscribe(observer);
        }
        public Student? GetStudentById(int id)
        {
            return _students.GetStudentById(id);
        }
        public List<Student> GetAllStudents()
        {
            return _students.GetAllStudents();
        }

        private void DeleteStudentCoursesAndExams(Student student)
        {
            if (student.ActiveCourseId != -1)
                courseController.DecrementCourseCurrentlyEnrolled(student.ActiveCourseId);

            foreach (int examTermId in student.RegisteredExamsIds)
                examTermController.DecrementExamTermCurrentlyAttending(examTermId);
            NotifyObservers();
        }
        public List<Course> GetAvailableCourses(int studentId)
        {
            Student student = GetStudentById(studentId);
            List<Course> allCourses = courseController.GetAllCourses();
            List<Course> availableCourses = new List<Course>();

            foreach (Course course in allCourses)
                if (IsCourseAvailable(course, student))
                    availableCourses.Add(course);

            return availableCourses;
        }
        public List<int> GetCoursesIdByExamTerm(ExamTerm exam)
        {
            List<Course> allCourses = courseController.GetAllCourses();
            List<int> courses = new List<int>();

            foreach (Course course in allCourses)
            {
                if (course.Language == exam.Language && course.Level == exam.Level)
                    courses.Add(course.Id);
            }

            return courses;
        }
        public List<int> GetFinishedCoursesIdByExamTerm(ExamTerm exam)
        {
            List<Course> allCourses = courseController.GetAllCourses();
            List<int> finishedCourses = new List<int>();

            foreach (Course course in allCourses)
            {
                if (course.Language== exam.Language && course.Level == exam.Level && course.StartDate < DateTime.Now)
                    finishedCourses.Add(course.Id);
            }
                
            return finishedCourses;
        }
        private bool IsCourseAvailable(Course course, Student student)
        {
            List<int> passedCoursesIds = GetPassedCourses(student);
            List<int> courseIdsByRegisteredExams = GetCourseIdsByRegisteredExams(student);
            DateTime currentTime = DateTime.Now;

            if (!passedCoursesIds.Contains(course.Id) &&
                  !courseIdsByRegisteredExams.Contains(course.Id) &&
                  !student.RegisteredCoursesIds.Contains(course.Id) &&
                  !student.CompletedCoursesIds.Contains(course.Id) &&
                  (course.StartDate - currentTime).TotalDays > 6 &&
                  ((course.IsOnline == false && course.CurrentlyEnrolled < course.MaxEnrolledStudents) ||
                   (course.IsOnline == true)))
                return true;

            return false;
        }
        private List<int> GetCourseIdsByRegisteredExams(Student student)
        {
            List<int> courses = new List<int>();
            List<int> finishedCourses = new List<int>();
            foreach (int examTermId in student.RegisteredExamsIds)
            {
                ExamTerm examTerm = examTermController.GetExamTermById(examTermId);
                finishedCourses = GetCoursesIdByExamTerm(examTerm);
                foreach (int courseId in finishedCourses)
                    courses.Add(courseId);
            }
            return courses;

        }
        private List<int> GetPassedCourses(Student student)
        {
            List<int> courses = new List<int>();
            List<int> finishedCourses = new List<int>();
            foreach (int examTermId in student.PassedExamsIds)
            {
                ExamTerm examTerm = examTermController.GetExamTermById(examTermId);
                finishedCourses = GetFinishedCoursesIdByExamTerm(examTerm);
                foreach(int courseId in finishedCourses)
                    courses.Add(courseId);
            }
            return courses;
        }
        public List<ExamTerm> GetAvailableExamTerms(int studentId)
        {
            Student student = GetStudentById(studentId);
            DateTime currentTime = DateTime.Now;

            List<ExamTerm> availableExamTerms = new List<ExamTerm>();


            foreach (int courseId in student.CompletedCoursesIds)
            {
                List<ExamTerm> examTerms = examTermController.GetAllExamTerms();
                Course course = courseController.GetCourseById(courseId);

                if (course.StartDate.AddDays(course.Duration * 7) >= DateTime.Now)
                {
                    foreach (ExamTerm examTerm in examTerms)
                    {
                        
                         if (course.Language == examTerm.Language &&
                             course.Level == examTerm.Level && !student.RegisteredExamsIds.Contains(examTerm.ExamID))
                         {
                             availableExamTerms.Add(examTerm);
                         }
                    }
                }
            }

            return availableExamTerms;
        }
        public List<Course> GetRegisteredCourses(int studentId)
        {
            Student student = GetStudentById(studentId);
            List<Course> registeredCourses = new List<Course>();
            foreach (int courseId in student.RegisteredCoursesIds)
                registeredCourses.Add(courseController.GetCourseById(courseId));

            return registeredCourses;
        }
        public List<Course> GetCompletedCourses(int studentId)
        {
            Student student = GetStudentById(studentId);
            List<Course> completedCourses = new List<Course>();
            foreach (int courseId in student.CompletedCoursesIds)
                completedCourses.Add(courseController.GetCourseById(courseId));

            return completedCourses;
        }
        public List<ExamTerm> GetRegisteredExamTerms(int studentId)
        {
            Student student = GetStudentById(studentId);

            List<ExamTerm> registeredExamTerms = new List<ExamTerm>();

            foreach (int id in student.RegisteredExamsIds)
            {
                ExamTerm exam = examTermController.GetExamTermById(id);
                if (exam.ExamTime > DateTime.Now)
                {
                    registeredExamTerms.Add(exam);
                }

            }
            return registeredExamTerms;
        }
        public List<ExamTerm> GetCompletedExamTerms(int studentId)
        {
            Student student = GetStudentById(studentId);
            List<ExamTerm> completedExamTerms = new List<ExamTerm>();

            foreach (int id in student.RegisteredExamsIds)
            {
                ExamTerm examTerm = examTermController.GetExamTermById(id);
                if (examTerm.ExamTime < DateTime.Now)
                {
                    completedExamTerms.Add(examTerm);
                }
            }

            return completedExamTerms;
        }
        public List<Course> GetPassedCourses(int studentId)
        {
            Student student = GetStudentById(studentId);
            List<Course> courses = new List<Course>();
            List<int> finishedCourses = new List<int>();
            foreach (int examTermId in student.PassedExamsIds)
            {
                ExamTerm examTerm = examTermController.GetExamTermById(examTermId);
                finishedCourses = GetFinishedCoursesIdByExamTerm(examTerm);
                foreach (int courseId in finishedCourses)
                    courses.Add(courseController.GetCourseById(courseId));
            }
            return courses;
        }

        public List<Student> GetAllStudentsRequestingCourse(int courseId)
        {
            List<Student> filteredStudents = new List<Student>();
            foreach (Student student in _students.GetAllStudents())
                if (student.RegisteredCoursesIds.Contains(courseId) || student.ActiveCourseId == courseId)
                    filteredStudents.Add(student);

            return filteredStudents;
        }
        public List<Student> GetAllStudentsEnrolledCourse(int courseId)
        {
            List<Student> filteredStudents = new List<Student>();
            foreach (Student student in _students.GetAllStudents())
                if (student.ActiveCourseId == courseId)
                    filteredStudents.Add(student);
            return filteredStudents;
        }

        public List<Student> GetAllStudentsCompletedCourse(int courseId)
        {
            List<Student> filteredStudents = new List<Student>();
            foreach (Student student in _students.GetAllStudents())
                if (student.CompletedCoursesIds.Contains(courseId))
                    filteredStudents.Add(student);
            return filteredStudents;
        }

        public List<Student> GetAllStudentsForExamTerm(int examTermId)
        {
            List<Student> filteredStudents = new List<Student>();
            foreach (Student student in _students.GetAllStudents())
                if (student.RegisteredExamsIds.Contains(examTermId))
                    filteredStudents.Add(student);
            return filteredStudents;
        }

        public bool IsEmailUnique(string email)
        {
            foreach (Student student in _students.GetAllStudents())
                if (student.Email.Equals(email)) return false;

            return true;
        }
        public bool RegisterForCourse(int studentId, int courseId)
        {
            Student student = GetStudentById(studentId);
            if (student.ActiveCourseId != -1)
                return false;

            student.RegisteredCoursesIds.Add(courseId);

            Update(student);
            return true;
        }
        public bool RejectStudentApplication(Student student, Course course)
        {
            student.RegisteredCoursesIds.Remove(course.Id);

            Update(student);
            return true;
        }
        public bool CancelCourseRegistration(int studentId, int courseId)
        {
            Course course = courseController.GetCourseById(courseId);
            DateTime currentDate = DateTime.Now;

            if ((course.StartDate - currentDate).TotalDays < 7)
                return false;

            Student student = GetStudentById(studentId);
            student.RegisteredCoursesIds.Remove(courseId);

            Update(student);
            return true;
        }
        public bool RegisterForExam(int studentId, int examId)
        {
            Student student = GetStudentById(studentId);
            ExamTerm examTerm = examTermController.GetExamTermById(examId);
            List<ExamTerm> completedExams = GetCompletedExamTerms(studentId);
            foreach (ExamTerm term in completedExams)
            {
                if (!term.Informed)
                    return false;
            }
            if (examTerm.CurrentlyAttending >= examTerm.MaxStudents)
                return false;

            student.RegisteredExamsIds.Add(examId);

            examTerm.CurrentlyAttending += 1;
            examTermController.UpdateExamTerm(examTerm);

            Update(student);
            return true;
        }
        public bool CancelExamRegistration(int studentId, int examTermId)
        {
            Student student = GetStudentById(studentId);
            ExamTerm examTerm = examTermController.GetExamTermById(examTermId);
            DateTime currentDate = DateTime.Now;

            if ((examTerm.ExamTime - currentDate).TotalDays >= 10)
            {
                student.RegisteredExamsIds.Remove(examTermId);

                examTerm.CurrentlyAttending -= 1;
                examTermController.UpdateExamTerm(examTerm);

                Update(student);
                return true;
            }

            return false;
        }
        public bool IsStudentAttendingCourse(int studentId)
        {
            return GetStudentById(studentId).ActiveCourseId != -1;
        }
        public bool GivePenaltyPoint(int studentId)
        {
            PenaltyPointRepository penaltyPointDAO = Injector.CreateInstance<PenaltyPointRepository>();
            Student student = GetStudentById(studentId);

            penaltyPointDAO.AddPenaltyPoint(new PenaltyPoint(studentId, student.ActiveCourseId, DateTime.Now, false));

            List<PenaltyPoint> penaltyPoints = penaltyPointDAO.GetPenaltyPointsByStudentId(studentId);
            if (penaltyPoints.Count == 3)
            {
                DeactivateStudentAccount(student);
            }

            Update(student);
            return true;
        }
        public Course? GetActiveCourse(int studentId)
        {
            Student student = GetStudentById(studentId);
            if (IsStudentAttendingCourse(studentId))
                return courseController.GetCourseById(student.ActiveCourseId);

            return null;
        }
        public bool IsQuitCourseMailSent(int studentId, int courseId)
        {
            MailController mailController = Injector.CreateInstance<MailController>();
            Student student = GetStudentById(studentId);
            return mailController.IsQuitCourseMailSent(student.Email, courseId);
        }
        public StudentGrade GradeStudentCourse(StudentGrade grade)
        {
            return _studentGrades.AddGrade(grade);
        }

        public Student GetStudentByEmail(string email)
        {
            foreach (Student student in _students.GetAllStudents())
            {
                if (student.Email == email)
                {
                    return student;
                }
            }
            return null;
        }
        public int IsSomeCourseCompleted(int studentId)
        {
            MailController mailController = Injector.CreateInstance<MailController>();
            Student student = GetStudentById(studentId);
            List<Mail> unreadReceivedMails = mailController.GetUnreadReceivedMails(student);

            foreach (Mail mail in unreadReceivedMails)
            {
                if (mail.TypeOfMessage == Domain.Model.Enums.TypeOfMessage.TeacherGradeStudentMessage)
                {
                    mailController.SetMailToAnswered(mail);
                    return mail.CourseId;
                }
            }
            return -1;
        }
        public bool IsEnterCourseRequestAccepted(int studentId)
        {
            MailController mailController = Injector.CreateInstance<MailController>();
            Student student = GetStudentById(studentId);
            List<Mail> unreadReceivedMails = mailController.GetUnreadReceivedMails(student);

            foreach (Mail mail in unreadReceivedMails)
            {
                Course course = courseController.GetCourseById(mail.CourseId);
                if (mail.TypeOfMessage == Domain.Model.Enums.TypeOfMessage.AcceptEnterCourseRequestMessage &&
                    DateTime.Now.Date >= course.StartDate.AddDays(-7).Date)
                {
                    mailController.SetMailToAnswered(mail);
                    student.ActiveCourseId = course.Id;
                    Update(student);
                    return true;
                }
            }
            return false;
        }
        public int GetCompletedCourseNumber(int studentId)
        {
            Student student = GetStudentById(studentId);
            return student.CompletedCoursesIds.Count;
        }
        public int GetPassedExamsNumber(int studentId)
        {
            Student student = GetStudentById(studentId);
            return student.PassedExamsIds.Count;
        }

        public void ProcessPenaltyPoints()
        {
            DateTime currentDate = DateTime.Now;
            if (currentDate.Day == 1)
            {
                PenaltyPointRepository penaltyPointDAO = Injector.CreateInstance<PenaltyPointRepository>();
                foreach (Student student in _students.GetAllStudents())
                {
                    List<PenaltyPoint> deletedPoints = penaltyPointDAO.GetDeletedPenaltyPointsByStudentId(student.Id);
                    if (deletedPoints.Count > 0)
                    {
                        PenaltyPoint point = deletedPoints[0];
                        point.IsDeleted = true;
                        penaltyPointDAO.UpdatePenaltyPoint(point);
                    }
                }
            }
        }

        public int GetPenaltyPointCount(int studentId)
        {
            PenaltyPointRepository penaltyPointDAO = Injector.CreateInstance<PenaltyPointRepository>();
            return penaltyPointDAO.GetPenaltyPointsByStudentId(studentId).Count;
        }

        public void DeactivateStudentAccount(Student student)
        {
            if (student.ActiveCourseId != -1)
            {
                Course course = courseController.GetCourseById(student.ActiveCourseId);
                DateTime courseEndDate = course.StartDate.AddDays(course.Duration * 7);
                if (DateTime.Now < courseEndDate)
                {
                    course.CurrentlyEnrolled--;
                    courseController.UpdateCourse(course);
                }
            }
            student.ActiveCourseId = -10;

        }
        private Dictionary<int, List<Student>> GetStudentsPerPenaltyPoints()
        {
            PenaltyPointRepository penaltyPointDAO = new PenaltyPointRepository();
            Dictionary<int, List<Student>> studentsPerPenalty = new Dictionary<int, List<Student>>();
            for (int i = 0; i <= 3; i++)
                studentsPerPenalty[i] = new List<Student>();

            foreach (Student student in GetAllStudents())
            {
                int penaltyPoints = penaltyPointDAO.GetPenaltyPointsByStudentId(student.Id).Count;
                studentsPerPenalty[penaltyPoints].Add(student);
            }

            return studentsPerPenalty;
        }
        private double GetStudentAveragePoints(int studentId)
        {
            ExamTermGradeRepository examTermGradeDAO = new ExamTermGradeRepository();
            List<ExamTermGrade> studentExamsGrades = examTermGradeDAO.GetExamTermGradeByStudent(studentId);
            int gradesSum = 0;

            foreach (ExamTermGrade examTermGrade in studentExamsGrades)
                gradesSum += examTermGrade.ReadingPoints + examTermGrade.ListeningPoints + examTermGrade.SpeakingPoints + examTermGrade.WritingPoints;
            return studentExamsGrades.Count > 0 ? gradesSum / studentExamsGrades.Count : 0;
        }
        private Dictionary<Student, double> GetStudentsAverageScore(List<Student> students)
        {
            Dictionary<Student, double> studentsAverageGrade = new Dictionary<Student, double>();

            foreach (Student student in students)
                studentsAverageGrade.Add(student, GetStudentAveragePoints(student.Id));
            return studentsAverageGrade;
        }
        public Dictionary<int, Dictionary<Student, double>> GetStudentsAveragePointsPerPenalty()
        {
            Dictionary<int, List<Student>> studentsPerPenalties = GetStudentsPerPenaltyPoints();
            Dictionary<int, Dictionary<Student, double>> studentsAveragePointsPerPenalty = new Dictionary<int, Dictionary<Student, double>>();

            for (int i = 0; i <= 3; i++)
                studentsAveragePointsPerPenalty.Add(i, GetStudentsAverageScore(studentsPerPenalties[i]));

            return studentsAveragePointsPerPenalty;
        }
        public void CompleteCourse(Student student, Course course)
        {
            student.ActiveCourseId = -1;
            student.CompletedCoursesIds.Add(course.Id);
            Update(student);
        }
    }
}
