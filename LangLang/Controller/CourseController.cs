using LangLang.Data;
using LangLang.Domain.IRepository;
using LangLang.Domain.Model;
using LangLang.Domain.Model.Enums;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LangLang.Controller
{
    public class CourseController
    {
        private readonly IStudentRepository _students;
        private readonly ICourseRepository _courses;
        private readonly TeacherController _teacherController;
        private readonly IExamTermRepository _examTerms;
        private readonly IDirectorDbRepository _director;

        private readonly int courseDurationInMinutes = 90;
        private readonly int examDurationInMinutes = 240; 

        public CourseController()
        {
            _students = Injector.CreateInstance<IStudentRepository>();
            _courses = Injector.CreateInstance<ICourseRepository>();
            _teacherController = Injector.CreateInstance<TeacherController>();
            _examTerms = Injector.CreateInstance<IExamTermRepository>();
            _director = Injector.CreateInstance<IDirectorDbRepository>();
        }

        public Course? GetById(int courseId)
        {
            return _courses.GetById(courseId);
        }

        public List<Course> GetAllCourses()
        {
            return _courses.GetAll();
        }

        public List<Course> GetAllCourses(int page, int pageSize, string sortCriteria, List<Course> courses)
        {
            return _courses.GetAllCourses(page, pageSize, sortCriteria, courses);
        }

        public List<Course> GetAvailableCourses(Teacher teacher)
        {
            List<Course> allCourses = GetAllCourses();
            List<int> allTeacherCourses = teacher.CoursesId;

            List<Course> availableCourses = new();

            foreach (Course course in allCourses)
            {
                if (allTeacherCourses.Contains(course.Id))
                    availableCourses.Add(course);
            }
            return availableCourses;
        }

        public Course Add(Course course)
        {
            Course createdCourse = _courses.Add(course);
            return createdCourse;
        }

        public void Update(Course course)
        {
            _courses.Update(course);
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
            List<Course> allAvailableCourses = _courses.GetAll();
            List<ExamTerm> allAvailableExams = _examTerms.GetAll();

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

        private (DateTime, DateTime) GetStartEndDate(Course course)
        {
            DateTime courseStartTime = course.StartDate;
            DateTime courseEndTime = course.StartDate.AddDays(course.Duration * 7).AddMinutes(courseDurationInMinutes);

            return (courseStartTime, courseEndTime);
        }
        private (DateTime, DateTime) GetStartEndDate(ExamTerm examTerm)
        {
            DateTime examStartTime = examTerm.ExamTime;
            DateTime examEndTime = examTerm.ExamTime.AddMinutes(examDurationInMinutes);
            return (examStartTime, examEndTime);
        }
        private bool CompareDates(DateTime startDateOne, DateTime endDateOne, DateTime startDateTwo, DateTime endDateTwo)
        {
            DateTime maxStartTime = startDateOne > startDateTwo ? startDateOne : startDateTwo;
            DateTime minEndTime = endDateOne < endDateTwo ? endDateOne : endDateTwo;

            if ((startDateOne == startDateTwo && endDateOne == endDateTwo) ||
                (maxStartTime < minEndTime))
                return true;

            return false;
        }
        private bool CompareTimes(Course course, Course secondCourse)
        {
            (TimeSpan courseSessionStart, TimeSpan courseSessionEnd) = GetSessionTimes(course);
            (TimeSpan secondCourseSessionStart, TimeSpan secondCourseSessionEnd) = GetSessionTimes(secondCourse);

            TimeSpan maxStartTime = courseSessionStart > secondCourseSessionStart ? courseSessionStart : secondCourseSessionStart;
            TimeSpan minEndTime = courseSessionEnd < secondCourseSessionEnd ? courseSessionEnd : secondCourseSessionEnd;
            if ((courseSessionStart == secondCourseSessionStart) || (maxStartTime < minEndTime))
                return true;
            return false;
        }

        private bool CheckTeacherCoursesOverlap(Course course, Teacher teacher)
        {
            List<Course> teacherCourses = GetAvailableCourses(teacher);
            foreach (Course secondCourse in teacherCourses)
            {
                if (course.Id == secondCourse.Id)
                    continue;

                if (CompareCourseDurations(course, secondCourse))
                    return true;
            }
            return false;
        }

        private bool CompareCourseDurations(Course currentCourse, Course secondCourse)
        {
            (DateTime courseStartTime, DateTime courseEndTime) = GetStartEndDate(currentCourse);

            (DateTime secondCourseStartTime, DateTime secondCourseEndTime) = GetStartEndDate(secondCourse);

            if (CompareDates(courseStartTime, courseEndTime, secondCourseStartTime, secondCourseEndTime))
            {
                bool isSessionOverlap = CheckSessionOverlap(currentCourse, secondCourse);
                if (isSessionOverlap)
                    return true;
            }
            return false;
        }

        private bool CompareCourseAndExam(Course course, ExamTerm examTerm)
        {
            if (!course.WorkDays.Contains(examTerm.ExamTime.DayOfWeek))
                return false;

            (DateTime courseStartTime, DateTime courseEndTime) = GetStartEndDate(course);
            (DateTime examStartTime, DateTime examEndTime) = GetStartEndDate(examTerm);

            if (CompareDates(courseStartTime, courseEndTime, examStartTime, examEndTime))
                return true;

            return false;
        }
        private bool CheckTeacherCourseExamOverlap(Course course, Teacher teacher)
        {
            List<ExamTerm> teacherExams = _teacherController.GetAvailableExamTerms(teacher);
            foreach (ExamTerm examTerm in teacherExams)
            {
                if (CompareCourseAndExam(course, examTerm))
                    return true;
            }
            return false;
        }

        private (TimeSpan,TimeSpan) GetSessionTimes(Course course)
        {
            TimeSpan sessionStart = course.StartDate.TimeOfDay;
            TimeSpan sessionEnd = sessionStart.Add(TimeSpan.FromMinutes(courseDurationInMinutes));

            return (sessionStart, sessionEnd);
        }

        private bool CheckSessionOverlap(Course course, Course secondCourse)
        {
            foreach (DayOfWeek day in course.WorkDays)
            {
                if (secondCourse.WorkDays.Contains(day))
                {
                    if (CompareTimes(course,secondCourse))
                        return true;
                }
            }
            return false;
        }

        private bool CheckClassroomOverlap(Course course, List<Course> allAvailableCourses, List<ExamTerm> allAvailableExams)
        {
            bool isClassroomOneTaken = false;

            foreach (Course secondCourse in allAvailableCourses)
            {
                if (secondCourse.IsOnline)
                    continue;

                    bool isSessionOverlap = CompareCourseDurations(course,secondCourse);
                    if (isSessionOverlap)
                        if (isClassroomOneTaken)
                            return true;
                        else
                            isClassroomOneTaken = true;
            }

            foreach (ExamTerm examTerm in allAvailableExams)
            {
                if (CompareCourseAndExam(course, examTerm))
                {
                    if (isClassroomOneTaken)
                        return true;
                    else
                        isClassroomOneTaken = true;
                }
            }

            return false;
        }

        public void Delete(int courseId)
        {
            _courses.Remove(courseId);
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
            Course? course = GetById(courseId);
            if (course == null)
                return;
            ++course.CurrentlyEnrolled;
            Update(course);
        }
        public void DecrementCourseCurrentlyEnrolled(int courseId)
        {
            Course? course = GetById(courseId);
            if (course == null)
                return;
            --course.CurrentlyEnrolled;
            Update(course);
        }

        public List<Course> FindCoursesByCriteria(Language? language, LanguageLevel? level, DateTime? startDate, int duration, bool? isOnline)
        {
            var filteredCourses = _courses.GetAll().Where(course =>
                (!language.HasValue || course.Language == language.Value) &&
                (!level.HasValue || course.Level == level.Value) &&
                (!startDate.HasValue || course.StartDate.Date >= (startDate.Value.Date)) &&
                (duration == 0 || course.Duration == duration) &&
                (!isOnline.HasValue || course.IsOnline == isOnline.Value)
            ).ToList();

            return filteredCourses;
        }

        public List<Course> FindCoursesByDate(DateTime startDate)
        {
            var filteredCourses = _courses.GetAll().Where(course =>
                course.StartDate.Date >= startDate.Date && course.StartDate.Date <= DateTime.Today.Date
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
            StudentsController studentController = new();
            List<Course> courses = GetAllCourses();
            List<Course> completedCourses = new();
            foreach (Course course in courses)
                if (HasCourseFinished(course, studentController.GetAllStudentsEnrolledCourse(course.Id).Count))
                    completedCourses.Add(course);
            return completedCourses;
        }
        public List<Course> GetCoursesForTopStudentMails()
        {
            MailController mailController = new();
            List<Course> courses = GetCompletedCourses();
            List<Course> sendMailCourses = new();

            foreach (Course course in courses)
                if (!mailController.IsTopStudentsMailSent(course.Id))
                    sendMailCourses.Add(course);
            return sendMailCourses;
        }

        private int GetCoursePenaltyPoints(int courseId)
        {
            PenaltyPointController penaltyPointController = new();
            return penaltyPointController.GetPointsByCourseId(courseId).Count;
        }

        public Dictionary<Course, int> GetPenaltyPointsLastYearPerCourse()
        {
            Dictionary<Course, int> coursePenaltyPoints = new();
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
            List<Course> courses = new();
            foreach (Course course in GetAllCourses())
                if (IsCourseLastYear(course) && !courses.Contains(course))
                    courses.Add(course);
            return courses;
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
            if (teacherCourses == null)
                return teacher;
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
