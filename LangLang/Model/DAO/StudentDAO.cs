using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LangLang.Storage;
using LangLang.Observer;
using System.Printing;
using System.Windows.Input;
using System.Windows;
using System.Linq.Expressions;
using LangLang.DTO;

namespace LangLang.Model.DAO
{
    public class StudentDAO : Subject
    {
        private readonly List<Student> _students;
        private readonly Storage<Student> _storage;
        private TeacherDAO teacherDAO;
        private MailDAO mailDAO;

        public StudentDAO()
        {
            _storage = new Storage<Student>("students.csv");
            _students = _storage.Load();
            teacherDAO = new TeacherDAO();
            mailDAO = new MailDAO();
        }

        private int GenerateId()
        {
            if (_students.Count == 0) return 1;
            return _students.Last().Id + 1;
        }

        public Student AddStudent(Student student)
        {
            student.Id = GenerateId();
            _students.Add(student);
            _storage.Save(_students);
            NotifyObservers();
            return student;
        }

        public Student? UpdateStudent(Student student)
        {
            Student? oldStudent = GetStudentById(student.Id);
            if (oldStudent == null) return null;

            oldStudent.FirstName = student.FirstName;
            oldStudent.LastName = student.LastName;
            oldStudent.Gender = student.Gender;
            oldStudent.DateOfBirth = student.DateOfBirth;
            oldStudent.PhoneNumber = student.PhoneNumber;
            oldStudent.Email = student.Email;
            oldStudent.Password = student.Password;
            oldStudent.EducationLevel = student.EducationLevel;
            oldStudent.PenaltyPoints = student.PenaltyPoints;
            oldStudent.ActiveCourseId = student.ActiveCourseId;
            oldStudent.PassedExamsIds = student.PassedExamsIds;
            oldStudent.RegisteredCoursesIds = student.RegisteredCoursesIds;
            oldStudent.RegisteredExamsIds = student.RegisteredExamsIds;

            _storage.Save(_students);
            NotifyObservers();
            return oldStudent;
        }

        public Student? RemoveStudent(int id)
        {
            Student? student = GetStudentById(id);
            if (student == null) return null;

            DeleteStudentCoursesAndExams(student);
            _students.Remove(student);
            _storage.Save(_students);
            NotifyObservers();
            return student;
        }

        private void DeleteStudentCoursesAndExams(Student student)
        {
            if (student.ActiveCourseId != -1)
                teacherDAO.DecrementCourseCurrentlyEnrolled(student.ActiveCourseId);

            foreach (int examTermId in student.RegisteredExamsIds)
            {
                teacherDAO.DecrementExamTermCurrentlyAttending(examTermId);
            }
            NotifyObservers();
        }

        public Student? GetStudentById(int id)
        {
            return _students.Find(v => v.Id == id);
        }

        public List<Student> GetAllStudents()
        {
            return _students;
        }

        public bool IsStudentAttendingCourse(int studentId)
        {
            Student student = GetStudentById(studentId);
            return student.ActiveCourseId != -1;
        }

        public List<Course> GetAvailableCourses(int studentId)
        {
            Student student = GetStudentById(studentId);
            List<Course> allCourses = teacherDAO.GetAllCourses();
            List<Course> availableCourses = new List<Course>();

            foreach (Course course in allCourses)
                if (IsCourseAvailable(course, student))
                    availableCourses.Add(course);

            return availableCourses;
        }

        private bool IsCourseAvailable(Course course, Student student)
        {
            List<int> passedCoursesIds = GetPassedCourses(student);
            List<int> courseIdsByRegisteredExams = GetCourseIdsByRegisteredExams(student);
            DateTime currentTime = DateTime.Now;

            if   (!passedCoursesIds.Contains(course.Id) &&
                  !courseIdsByRegisteredExams.Contains(course.Id) &&
                  !student.RegisteredCoursesIds.Contains(course.Id) &&
                  !student.CompletedCoursesIds.Contains(course.Id) &&
                  (course.CurrentlyEnrolled < course.MaxEnrolledStudents) &&
                  (course.StartDate - currentTime).TotalDays > 6)
                return true;

            return false;
        }

        private List<int> GetCourseIdsByRegisteredExams(Student student)
        {
            List<int> courses = new List<int>();
            foreach (int examTermId in student.RegisteredExamsIds)
            {
                ExamTerm examTerm = teacherDAO.GetExamTermById(examTermId);
                if (!courses.Contains(examTerm.CourseID))
                    courses.Add(examTerm.CourseID);
            }
            return courses;
        }
        private List<int> GetPassedCourses(Student student)
        {
            List<int> courses = new List<int>();
            foreach (int examTermId in student.PassedExamsIds)
            {
                ExamTerm examTerm = teacherDAO.GetExamTermById(examTermId);
                courses.Add(examTerm.CourseID);

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
                List<ExamTerm> examTerms = teacherDAO.GetAllExamTerms();
                Course course = teacherDAO.GetCourseById(courseId);

                foreach (ExamTerm examTerm in examTerms)
                {
                    Course secondCourse = teacherDAO.GetCourseByExamId(examTerm.ExamID);
                    if (examTerm.CurrentlyAttending < examTerm.MaxStudents &&
                        (examTerm.ExamTime - currentTime).TotalDays > 30 &&
                        course.Language == secondCourse.Language &&
                        course.Level == secondCourse.Level && !student.RegisteredExamsIds.Contains(examTerm.ExamID))
                    {
                        availableExamTerms.Add(examTerm);
                    }
                }
            }

            return availableExamTerms;
        }
        public List<ExamTerm> GetRegisteredExamTerms(int studentId)
        {
            Student student = GetStudentById(studentId);

            List<ExamTerm> registeredExamTerms = new List<ExamTerm>();

            foreach (int id in student.RegisteredExamsIds)
            {
                ExamTerm exam = teacherDAO.GetExamTermById(id);
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
                ExamTerm examTerm = teacherDAO.GetExamTermById(id);
                if (examTerm.ExamTime < DateTime.Now)
                {
                    completedExamTerms.Add(examTerm);
                }
            }

            return completedExamTerms;
        }

        private List<ExamTerm> GetExamTermsByCourse(int courseId)
        {
            Course course = teacherDAO.GetCourseById(courseId);
            return teacherDAO.FindExamTermsByCriteria(course.Language, course.Level, null);
        }

        public List<Course> GetRegisteredCourses(int studentId)
        {
            Student student = GetStudentById(studentId);
            List<Course> registeredCourses = new List<Course>();
            foreach (int courseId in student.RegisteredCoursesIds)
                registeredCourses.Add(teacherDAO.GetCourseById(courseId));

            return registeredCourses;
        }
        public List<Course> GetCompletedCourses(int studentId)
        {
            Student student = GetStudentById(studentId);
            List<Course> completedCourses = new List<Course>();
            foreach (int courseId in student.CompletedCoursesIds)
                completedCourses.Add(teacherDAO.GetCourseById(courseId));

            return completedCourses;
        }
        public List<Course> GetPassedCourses(int studentId)
        {
            Student student = GetStudentById(studentId);
            List<Course> registeredCourses = new List<Course>();
            foreach (int examTermId in student.PassedExamsIds)
            {
                ExamTerm examTerm = teacherDAO.GetExamTermById(examTermId);
                registeredCourses.Add(teacherDAO.GetCourseById(examTerm.CourseID));
            }

            return registeredCourses;
        }
        public List<Student> GetAllStudentsRequestingCourse(int courseId)
        {
            List<Student> filteredStudents = new List<Student>();
            foreach (Student student in _students)
                if (student.RegisteredCoursesIds.Contains(courseId) || student.ActiveCourseId == courseId)
                    filteredStudents.Add(student);

            return filteredStudents;
        }
        public List<Student> GetAllStudentsForCourse(int courseId)
        {
            List<Student> filteredStudents = new List<Student>();
            foreach (Student student in _students)
                if (student.ActiveCourseId == courseId)
                    filteredStudents.Add(student);
            return filteredStudents;
        }

        public List<Student> GetAllStudentsCompletedCourse(int courseId)
        {
            List<Student> filteredStudents = new List<Student>();
            foreach (Student student in _students)
                if (student.CompletedCoursesIds.Contains(courseId))
                    filteredStudents.Add(student);
            return filteredStudents;
        }

        public List<Student> GetAllStudentsForExamTerm(int examTermId)
        {
            List<Student> filteredStudents = new List<Student>();
            foreach (Student student in _students)
                if (student.RegisteredExamsIds.Contains(examTermId))
                    filteredStudents.Add(student);
            return filteredStudents;
        }

        public bool IsEmailUnique(string email)
        {
            foreach (Student student in _students)
                if (student.Email.Equals(email)) return false;
            
            return true;
        }

        public bool RegisterForCourse(int studentId, int courseId)
        {
            Student student = GetStudentById(studentId);
            if (student.ActiveCourseId != -1)
                return false;

            student.RegisteredCoursesIds.Add(courseId);

            _storage.Save(_students);
            NotifyObservers();
            return true;
        }

        public bool CancelCourseRegistration(int studentId, int courseId)
        {
            Course course = teacherDAO.GetCourseById(courseId);
            DateTime currentDate = DateTime.Now;

            if ((course.StartDate - currentDate).TotalDays < 7)
                return false;

            Student student = GetStudentById(studentId);
            student.RegisteredCoursesIds.Remove(courseId);

            _storage.Save(_students);
            NotifyObservers();
            return true;
        }

        public bool RegisterForExam(int studentId, int examId)
        {
            Student student = GetStudentById(studentId);
            ExamTerm examTerm = teacherDAO.GetExamTermById(examId);
            List<ExamTerm> completedExams = GetCompletedExamTerms(studentId); 
            foreach(ExamTerm term in completedExams)
            {
                if (!term.Informed)
                    return false;
            }
            if (examTerm.CurrentlyAttending >= examTerm.MaxStudents)
                return false;

            student.RegisteredExamsIds.Add(examId);

            examTerm.CurrentlyAttending += 1;
            teacherDAO.UpdateExamTerm(examTerm);

            _storage.Save(_students);
            NotifyObservers();
            return true;
        }

        public bool CancelExamRegistration(int studentId, int examTermId)
        {
            Student student = GetStudentById(studentId);
            ExamTerm examTerm = teacherDAO.GetExamTermById(examTermId);
            DateTime currentDate = DateTime.Now;

            if ((examTerm.ExamTime - currentDate).TotalDays >= 10)
            {
                student.RegisteredExamsIds.Remove(examTermId);

                examTerm.CurrentlyAttending -= 1;
                teacherDAO.UpdateExamTerm(examTerm);

                _storage.Save(_students);
                NotifyObservers();
                return true;
            }

            return false;
        }


        public bool GivePenaltyPoint(int studentId)
        {
            Student student = GetStudentById(studentId);
            ++student.PenaltyPoints;
            if (student.PenaltyPoints == 3)
            {
                DeactivateStudentAccount(student);
            }

            _storage.Save(_students);
            NotifyObservers();
            return true;
        }
        public void ProcessPenaltyPoints()
        {
            DateTime currentDate = DateTime.Now;
            if (currentDate.Day == 1)
            {
                foreach (Student student in _students)
                {
                    if (student.PenaltyPoints > 0)
                    {
                        student.PenaltyPoints--;
                        if (student.PenaltyPoints == 3)
                        {
                            DeactivateStudentAccount(student);
                        }
                    }
                }
                _storage.Save(_students);
                NotifyObservers();
            }
        }

        public void DeactivateStudentAccount(Student student)
        {
            if (student.ActiveCourseId != -1)
            {
                Course course = teacherDAO.GetCourseById(student.ActiveCourseId);
                DateTime courseEndDate = course.StartDate.AddDays(course.Duration * 7);
                if (DateTime.Now < courseEndDate)
                {
                    course.CurrentlyEnrolled--;
                    teacherDAO.UpdateCourse(course);
                }
            }
            student.ActiveCourseId = -10;

        }
        public Course? GetActiveCourse(int studentId)
        {
            Student student = GetStudentById(studentId);
            if (IsStudentAttendingCourse(studentId))
                return teacherDAO.GetCourseById(student.ActiveCourseId);

            return null;
        }

        public bool IsQuitCourseMailSent(int studentId, int courseId)
        {
            mailDAO = new MailDAO();
            Student student = GetStudentById(studentId);
            return mailDAO.IsQuitCourseMailSent(student.Email, courseId);
        }
        public Student GetStudentByEmail(string email)
        {
            foreach (Student student in _students)
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
            Student student = GetStudentById(studentId);
            List<Mail> unreadReceivedMails = mailDAO.GetUnreadReceivedMails(student);

            foreach(Mail mail in unreadReceivedMails)
            {
                if (mail.TypeOfMessage == Enums.TypeOfMessage.TeacherGradeStudentMessage)
                {
                    mailDAO.SetMailToAnswered(mail);
                    return mail.CourseId;
                }
            }
            return -1;
        }

        public bool IsEnterCourseRequestAccepted(int studentId)
        {
            Student student = GetStudentById(studentId);
            List<Mail> unreadReceivedMails = mailDAO.GetUnreadReceivedMails(student);

            foreach (Mail mail in unreadReceivedMails)
            {
                Course course = teacherDAO.GetCourseById(mail.CourseId);
                if (mail.TypeOfMessage == Enums.TypeOfMessage.AcceptEnterCourseRequestMessage &&
                    DateTime.Now.Date >= course.StartDate.AddDays(-7).Date)
                {
                    mailDAO.SetMailToAnswered(mail);
                    student.ActiveCourseId = course.Id;
                    UpdateStudent(student);
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
        public int GetPenaltyPointsNumber(int studentId)
        {
            Student student = GetStudentById(studentId);
            return student.PenaltyPoints;
        }

    }
}
