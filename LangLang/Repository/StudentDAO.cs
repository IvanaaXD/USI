﻿using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.Domain.Model;
using LangLang.Storage;
using LangLang.Observer;
using LangLang.Controller;

namespace LangLang.Repository
{
    public class StudentDAO : Subject
    {
        private readonly List<Student> _students;
        private readonly Storage<Student> _storage;
        private TeacherRepository teacherDAO;
        private CourseController courseController;
        private ExamTermController examTermController;
        private PenaltyPointDAO penaltyPointDAO;
        private MailDAO mailDAO;

        public StudentDAO()
        {
            _storage = new Storage<Student>("students.csv");
            _students = _storage.Load();
            teacherDAO = new TeacherRepository();
            penaltyPointDAO = new PenaltyPointDAO();    
            mailDAO = new MailDAO();
            courseController = new CourseController(new CourseRepository(), new TeacherController());
            examTermController = new ExamTermController(new ExamTermRepository(), new TeacherController());

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
                courseController.DecrementCourseCurrentlyEnrolled(student.ActiveCourseId);

            foreach (int examTermId in student.RegisteredExamsIds)
            {
                examTermController.DecrementExamTermCurrentlyAttending(examTermId);
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
                  (course.StartDate - currentTime).TotalDays > 6 &&
                  ((course.IsOnline == false && course.CurrentlyEnrolled < course.MaxEnrolledStudents) ||
                   (course.IsOnline == true)))
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
            return examTermController.FindExamTermsByCriteria(course.Language, course.Level, null);
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
        public bool RejectStudentApplication(Student student, Course course)
        {
            student.RegisteredCoursesIds.Remove(course.Id);

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
            examTermController.UpdateExamTerm(examTerm);

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
                examTermController.UpdateExamTerm(examTerm);

                _storage.Save(_students);
                NotifyObservers();
                return true;
            }

            return false;
        }
        public bool GivePenaltyPoint(int studentId)
        {
            Student student = GetStudentById(studentId);
            penaltyPointDAO.AddPenaltyPoint(new PenaltyPoint(studentId, student.ActiveCourseId, DateTime.Now, false));

            List<PenaltyPoint> penaltyPoints = penaltyPointDAO.GetPenaltyPointsByStudentId(studentId);
            if (penaltyPoints.Count == 3)
            {
                DeactivateStudentAccount(student);
            }

            _storage.Save(_students);
            NotifyObservers();
            return true;
        }
        public int GetPenaltyPointCount(int studentId)
        {
            return penaltyPointDAO.GetPenaltyPointsByStudentId(studentId).Count;
        }
        public void ProcessPenaltyPoints()
        {
            DateTime currentDate = DateTime.Now;
            if (currentDate.Day == 1)
            {
                foreach (Student student in _students)
                {
                    List<PenaltyPoint> deletedPoints = penaltyPointDAO.GetDeletedPenaltyPointsByStudentId(student.Id);
                    if (deletedPoints.Count > 0)
                    {
                        PenaltyPoint point = deletedPoints[0];
                        point.IsDeleted = true;
                        penaltyPointDAO.UpdatePenaltyPoint(point);
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
                    courseController.UpdateCourse(course);
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
                if (mail.TypeOfMessage == Domain.Model.Enums.TypeOfMessage.TeacherGradeStudentMessage)
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
                if (mail.TypeOfMessage == Domain.Model.Enums.TypeOfMessage.AcceptEnterCourseRequestMessage &&
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
        private Dictionary<int, List<Student>> GetStudentsPerPenaltyPoints()
        {
            PenaltyPointDAO penaltyPointDAO = new PenaltyPointDAO();
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

        private int GetStudentGradesSum(int studentId)
        {
            CourseGradeDAO courseGradeDAO = new CourseGradeDAO();
            int gradesSum = 0;
            foreach (CourseGrade courseGrade in courseGradeDAO.GetAllCourseGrades())
                if (courseGrade.StudentId == studentId)
                    //gradesSum += courseGrade.Value;
                    gradesSum += 0;
            return gradesSum;
        }
        private double GetStudentAverageGrades(int studentId)
        {
            ExamTermGradeRepository examTermGradeDAO = new ExamTermGradeRepository();
            List<ExamTermGrade> studentExamsGrades = examTermGradeDAO.GetExamTermGradeByStudent(studentId);
            int gradesSum = 0;

            foreach (ExamTermGrade examTermGrade in studentExamsGrades)
                gradesSum += examTermGrade.Value;
            return studentExamsGrades.Count > 0  ? gradesSum / studentExamsGrades.Count : 0;
        }
        private Dictionary<Student, double> GetStudentsAverageScore(List<Student> students)
        {
            Dictionary<Student, double> studentsAverageGrade = new Dictionary<Student, double>();

            foreach (Student student in students)
                studentsAverageGrade.Add(student, GetStudentAverageGrades(student.Id));
            return studentsAverageGrade;
        }

        public Dictionary<int, Dictionary<Student, double>> GetStudentsAveragePointsPerPenalty()
        {
            Dictionary<int, List<Student>> studentsPerPenalties = GetStudentsPerPenaltyPoints();
            Dictionary<int, Dictionary<Student,double>> studentsAveragePointsPerPenalty = new Dictionary<int, Dictionary<Student, double>>();

            for (int i = 0; i <= 3; i++)
                studentsAveragePointsPerPenalty.Add(i,GetStudentsAverageScore(studentsPerPenalties[i]));

            return studentsAveragePointsPerPenalty;
        }
        public void CompleteCourse(Student student, Course course)
        {
            student.ActiveCourseId = -1;
            student.CompletedCoursesIds.Add(course.Id);
            UpdateStudent(student);
        }
    }
}
