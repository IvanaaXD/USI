using LangLang.Domain.Model;
using LangLang.Domain.Model.Enums;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using LangLang.Domain.IRepository;
using System.Linq;

namespace LangLang.Controller
{
    public class CourseController
    {
        private readonly IStudentRepository _students;
        private readonly ICourseRepository _courses;
        private readonly ITeacherRepository? _teachers;
        private readonly TeacherController _teacherController;
        private readonly IExamTermRepository? _examTerms;
        private readonly IDirectorRepository? _director;
        private readonly IMailRepository? _mails;

        public CourseController()
        {
            _students = Injector.CreateInstance<IStudentRepository>();
            _courses = Injector.CreateInstance<ICourseRepository>();
            _teachers = Injector.CreateInstance<ITeacherRepository>();
            _teacherController = Injector.CreateInstance<TeacherController>();
            _examTerms = Injector.CreateInstance<IExamTermRepository>();
            _director = Injector.CreateInstance<IDirectorRepository>();
            _mails = Injector.CreateInstance<IMailRepository>();
        }

        public Course? GetCourseById(int courseId)
        {
            return _courses.GetCourseById(courseId);
        }
        public List<Course> GetAllCourses()
        {
            return _courses.GetAllCourses();
        }
        public List<Course> GetAllCourses(int page, int pageSize, string sortCriteria, List<Course> courses)
        {
            return _courses.GetAllCourses(page, pageSize, sortCriteria, courses);
        }

        public List<Course> GetAvailableCourses(Teacher teacher)
        {
            List<Course> allCourses = GetAllCourses();
            List<int> allTeacherCourses = teacher.CoursesId;

            List<Course> availableCourses = new List<Course>();

            foreach (Course course in allCourses)
            {
                if (allTeacherCourses.Contains(course.Id))
                    availableCourses.Add(course);
            }
            return availableCourses;
        }
        public Course AddCourse(Course course, int teacherId)
        {
            Course createdCourse = _courses.AddCourse(course);
            if (teacherId != -1)
            {
                Teacher teacher = _director.GetTeacherById(teacherId);
                teacher.CoursesId.Add(course.Id);
                _director.UpdateTeacher(teacher);
            }
            return createdCourse;
        }
        public void UpdateCourse(Course course)
        {
            _courses.UpdateCourse(course);
        }

        public bool ValidateCourseTimeslot(Course course, Teacher teacher)
        {
            bool isOverlap = CheckCourseOverlap(course, teacher);
            if (!isOverlap)
                return isOverlap;
            return true;
        }
        private bool CheckCourseOverlap(Course course, Teacher teacher)
        {
            List<Course> allAvailableCourses = _courses.GetAllCourses();
            List<ExamTerm> allAvailableExams = _examTerms.GetAllExamTerms();

            bool isSameTeacherCourseOverlap = CheckTeacherCoursesOverlap(course, teacher);
            if (isSameTeacherCourseOverlap)
                return false;

            bool isSameTeacherExamOverlap = CheckTeacherCourseExamOverlap(course, teacher);
            if (isSameTeacherExamOverlap)
                return false;

            if (!course.IsOnline)
            {
                bool isClassroomOverlap = CheckClassroomOverlap(course, allAvailableCourses, allAvailableExams);
                if (isClassroomOverlap)
                    return false;
            }
            return true;

        }
        public bool CheckTeacherCoursesOverlap(Course course, Teacher teacher)
        {
            int courseDurationInMinutes = 90;

            DateTime courseStartTime = course.StartDate; // start of first ever course session 
            DateTime courseEndTime = course.StartDate.AddDays(course.Duration * 7).AddMinutes(courseDurationInMinutes); // end of last ever course session

            List<Course> teacherCourses = GetAvailableCourses(teacher);
            foreach (Course secondCourse in teacherCourses)
            {
                if (course.Id == secondCourse.Id)
                    continue;

                DateTime secondCourseStartTime = secondCourse.StartDate;
                DateTime secondCourseEndTime = secondCourse.StartDate.AddDays(course.Duration * 7).AddMinutes(courseDurationInMinutes);

                DateTime maxStartTime = courseStartTime > secondCourseStartTime ? courseStartTime : secondCourseStartTime;
                DateTime minEndTime = courseEndTime < secondCourseEndTime ? courseEndTime : secondCourseEndTime;

                if ((courseStartTime == secondCourseStartTime && courseEndTime == secondCourseEndTime) ||
                    (maxStartTime < minEndTime))
                {
                    bool isSessionOverlap = CheckSessionOverlap(course, secondCourse);
                    if (isSessionOverlap)
                        return true;
                }
            }
            return false;
        }

        public bool CheckTeacherCourseExamOverlap(Course course, Teacher teacher)
        {
            int courseDurationInMinutes = 90;
            int examDurationInMinutes = 240;

            DateTime courseStartTime = course.StartDate; // start of first ever course session 
            DateTime courseEndTime = course.StartDate.AddDays(course.Duration * 7).AddMinutes(courseDurationInMinutes); // end of last ever course session

            List<ExamTerm> teacherExams = _teacherController.GetAvailableExamTerms(teacher);
            foreach (ExamTerm examTerm in teacherExams)
            {
                if (!course.WorkDays.Contains(examTerm.ExamTime.DayOfWeek))
                    continue;

                DateTime examStartTime = examTerm.ExamTime;
                DateTime examEndTime = examTerm.ExamTime.AddMinutes(examDurationInMinutes);

                DateTime maxStartTime = courseStartTime > examStartTime ? courseStartTime : examStartTime;
                DateTime minEndTime = courseEndTime < examEndTime ? courseEndTime : examEndTime;

                if ((courseStartTime == examStartTime || courseEndTime == examEndTime) ||
                    (maxStartTime < minEndTime))
                    return true;
            }
            return false;
        }

        private bool CheckSessionOverlap(Course course, Course secondCourse)
        {
            int courseDurationInMinutes = 90;
            foreach (DayOfWeek day in course.WorkDays)
            {
                if (secondCourse.WorkDays.Contains(day))
                {
                    TimeSpan sessionOneStart = course.StartDate.TimeOfDay;
                    TimeSpan sessionTwoStart = secondCourse.StartDate.TimeOfDay;

                    TimeSpan sessionOneEnd = sessionOneStart.Add(TimeSpan.FromMinutes(courseDurationInMinutes));
                    TimeSpan sessionTwoEnd = sessionTwoStart.Add(TimeSpan.FromMinutes(courseDurationInMinutes));

                    TimeSpan maxStartTime = sessionOneStart > sessionTwoStart ? sessionOneStart : sessionTwoStart;
                    TimeSpan minEndTime = sessionOneEnd < sessionTwoEnd ? sessionOneEnd : sessionTwoEnd;
                    if ((sessionOneStart == sessionTwoStart) || (maxStartTime < minEndTime))
                        return true;
                }
            }
            return false;
        }
        public (DateTime, DateTime) GetCourseDates(Course course, Course secondCourse)
        {
            DateTime courseStartTime = course.StartDate;
            DateTime courseEndTime = course.StartDate.AddDays(course.Duration * 7).AddMinutes(90);

            DateTime secondCourseStartTime = secondCourse.StartDate;
            DateTime secondCourseEndTime = secondCourse.StartDate.AddDays(course.Duration * 7).AddMinutes(90);

            DateTime maxStartTime = courseStartTime > secondCourseStartTime ? courseStartTime : secondCourseStartTime;
            DateTime minEndTime = courseEndTime < secondCourseEndTime ? courseEndTime : secondCourseEndTime;

            return (maxStartTime, minEndTime);
        }

        public bool CheckClassroomOverlap(Course course, List<Course> allAvailableCourses, List<ExamTerm> allAvailableExams)
        {
            bool isClassroomOneTaken = false;

            int courseDurationInMinutes = 90;
            int examDurationInMinutes = 240;

            DateTime courseStartTime = course.StartDate;
            DateTime courseEndTime = course.StartDate.AddDays(course.Duration * 7).AddMinutes(courseDurationInMinutes);

            foreach (Course secondCourse in allAvailableCourses)
            {
                if (secondCourse.IsOnline)
                    continue;

                (DateTime maxStartTime, DateTime minEndTime) = GetCourseDates(course, secondCourse);

                if ((courseStartTime == secondCourse.StartDate && courseEndTime == secondCourse.StartDate.AddDays(course.Duration * 7).AddMinutes(90)) ||
                    (maxStartTime < minEndTime))
                {
                    bool isSessionOverlap = CheckSessionOverlap(course, secondCourse);
                    if (isSessionOverlap)
                        if (isClassroomOneTaken)
                            return true;
                        else
                            isClassroomOneTaken = true;
                }
            }

            foreach (ExamTerm examTerm in allAvailableExams)
            {
                if (!course.WorkDays.Contains(examTerm.ExamTime.DayOfWeek))
                    continue;

                DateTime examStartTime = examTerm.ExamTime;
                DateTime examEndTime = examTerm.ExamTime.AddMinutes(examDurationInMinutes);

                DateTime maxStartTime = courseStartTime > examStartTime ? courseStartTime : examStartTime;
                DateTime minEndTime = courseEndTime < examEndTime ? courseEndTime : examEndTime;

                if ((courseStartTime == examStartTime || courseEndTime == examEndTime) ||
                    (maxStartTime < minEndTime))
                {
                    if (isClassroomOneTaken)
                        return true;
                    else
                        isClassroomOneTaken = true;
                }
            }

            return false;
        }

        public void DeleteCourse(int courseId)
        {
            _courses.RemoveCourse(courseId);
            RemoveCourseFromRequests(courseId);
        }

        public void RemoveCourseFromRequests(int courseId)
        {
            List<Student> students = _students.GetAllStudents();
            foreach(Student student in students)
            {
                if (student.RegisteredCoursesIds.Contains(courseId))
                {
                    student.RegisteredCoursesIds.Remove(courseId);
                    _students.UpdateStudent(student);
                }
            }       
        }

        public void Subscribe(IObserver observer)
        {
            _courses.Subscribe(observer);
        }

        public void IncrementCourseCurrentlyEnrolled(int courseId)
        {
            Course course = GetCourseById(courseId);
            ++course.CurrentlyEnrolled;
            UpdateCourse(course);
        }
        public void DecrementCourseCurrentlyEnrolled(int courseId)
        {
            Course course = GetCourseById(courseId);
            --course.CurrentlyEnrolled;
            UpdateCourse(course);
        }

        public List<Course> FindCoursesByCriteria(Language? language, LanguageLevel? level, DateTime? startDate, int duration, bool? isOnline)
        {
            var filteredCourses = _courses.GetAllCourses().Where(course =>
                (!language.HasValue || course.Language == language.Value) &&
                (!level.HasValue || course.Level == level.Value) &&
                (!startDate.HasValue || course.StartDate.Date >= (startDate.Value.Date)) &&
                (duration == 0 || course.Duration == duration) &&
                (!isOnline.HasValue || course.IsOnline == isOnline.Value)
            ).ToList();

            return filteredCourses;
        }

        public List<Course> FindCoursesByDate(DateTime? startDate)
        {
            var filteredCourses = _courses.GetAllCourses().Where(course =>
                (course.StartDate.Date >= (startDate.Value.Date) && course.StartDate.Date <= DateTime.Today.Date)
            ).ToList();

            return filteredCourses;
        }

        public bool HasStudentAcceptingPeriodEnded(Course course)
        {
            return (course.StartDate <= DateTime.Now.AddDays(7));
        }

        public bool HasCourseStarted(Course course)
        {
            return (course.StartDate <= DateTime.Now);
        }

        public bool HasGradingPeriodStarted(Course course)
        {
            return (course.StartDate.AddDays(7 * course.Duration) <= DateTime.Now);
        }

        public bool HasCourseFinished(Course course, int studentCount)
        {
            if (course.StartDate.AddDays(course.Duration * 7) >= DateTime.Now)
                return false;

            if (studentCount == 0)
                return true;

            return false;
        }
        public List<Student> GetCourseStudents(StudentsController studentController, Course course)
        {
            var students = studentController.GetAllStudentsRequestingCourse(course.Id);

            if (HasCourseStarted(course) && !HasCourseFinished(course, GetStudentCount(studentController, course)))
                students = studentController.GetAllStudentsEnrolledCourse(course.Id);

            else if (HasCourseFinished(course, GetStudentCount(studentController, course)))
                students = studentController.GetAllStudentsCompletedCourse(course.Id);

            return students;
        }

        public int GetStudentCount(StudentsController studentController, Course course)
        {
            return studentController.GetAllStudentsEnrolledCourse(course.Id).Count;
        }
        private List<Course> GetCompletedCourses()
        {
            StudentsController studentController = new StudentsController();
            List<Course> courses = GetAllCourses();
            List<Course> completedCourses = new List<Course>();
            foreach (Course course in courses)
                if (HasCourseFinished(course, studentController.GetAllStudentsEnrolledCourse(course.Id).Count))
                    completedCourses.Add(course);
            return completedCourses;
        }
        public List<Course> GetCoursesForTopStudentMails()
        {
            MailController mailController = new MailController();
            List<Course> courses = GetCompletedCourses();
            List<Course> sendMailCourses = new List<Course>();

            foreach (Course course in courses)
                if (!mailController.IsTopStudentsMailSent(course.Id))
                    sendMailCourses.Add(course);
            return sendMailCourses;
        }

        private int GetCoursePenaltyPoints(int courseId)
        {
            PenaltyPointController penaltyPointController = new PenaltyPointController();
            return penaltyPointController.GetPointsByCourseId(courseId).Count;
        }

        public Dictionary<Course, int> GetPenaltyPointsLastYearPerCourse()
        {
            Dictionary<Course, int> coursePenaltyPoints = new Dictionary<Course, int>();
            foreach (Course course in GetCoursesLastYear())
                coursePenaltyPoints[course] = GetCoursePenaltyPoints(course.Id);
            return coursePenaltyPoints;
        }

        private bool IsDateWithinLastYear(DateTime date)
        {
            DateTime todayDate = DateTime.Now;
            DateTime oneYearAgoDate = DateTime.Now.AddYears(-1);

            return date >= oneYearAgoDate && date <= todayDate;
        }

        private bool IsCourseYearlong(DateTime courseStartDate, DateTime courseEndDate)
        {
            DateTime todayDate = DateTime.Now;
            DateTime oneYearAgoDate = DateTime.Now.AddYears(-1);

            return courseStartDate < oneYearAgoDate && courseEndDate > todayDate;
        }

        private bool IsCourseLastYear(Course course)
        {
            DateTime courseStartDate = course.StartDate;
            DateTime courseEndDate = course.StartDate.AddDays(course.Duration * 7);

            return IsDateWithinLastYear(courseStartDate) || IsDateWithinLastYear(courseEndDate) ||
                   IsCourseYearlong(courseStartDate, courseEndDate);

        }
        public List<Course> GetCoursesLastYear()
        {
            List<Course> courses = new List<Course>();
            foreach (Course course in GetAllCourses())
                if (IsCourseLastYear(course) && !courses.Contains(course))
                    courses.Add(course);
            return courses;
        }

        public List<Course> GetCoursesByTeacher(int teacherId)
        {
            return _courses.GetCoursesByTeacher(teacherId);
        }

        public List<Course>? GetCoursesForDisplay(bool isSearchClicked, List<Course> availableCourses, Language? selectedLanguage, LanguageLevel? selectedLevel, DateTime? selectedStartDate, int selectedDuration, bool isOnline)
        {
            List<Course> finalCourses = new();
            if (!isSearchClicked)
                return availableCourses;

            List<Course> allFilteredCourses = FindCoursesByCriteria(selectedLanguage, selectedLevel, selectedStartDate, selectedDuration, isOnline);
            foreach (Course course in allFilteredCourses)
            {
                foreach (Course teacherCourse in availableCourses)
                {
                    if (teacherCourse.Id == course.Id)
                        finalCourses.Add(course);
                }
            }

            return finalCourses;
        }

        public Teacher DeleteCoursesByTeacher(Teacher teacher)
        {
            var courses = GetAllCourses();
            var teacherCourses = teacher.CoursesId;
            var director = _director.GetDirector();

            foreach (var course in courses)
                if (teacherCourses.Contains(course.Id) && course.StartDate > DateTime.Today.Date && !director.CoursesId.Contains(course.Id))
                    teacherCourses.Remove(course.Id);

            teacher.CoursesId = teacherCourses;
            return teacher;
        }

        public bool IsCourseActive(Course course)
        {
            if (DateTime.Today.Date > course.StartDate.Date && course.StartDate.Date.AddDays(course.Duration*7) > DateTime.Today.Date) 
                return true;
            return false;
        }
    }
}
