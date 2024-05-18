using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using LangLang.Controller;
using LangLang.Model.Enums;
using LangLang.Observer;
using LangLang.Storage;

namespace LangLang.Model.DAO
{
    public class CourseDAO : Subject
    {
        private readonly List<Course> _courses;
        private readonly Storage<Course> _courseStorage;

        private readonly TeacherController teacherController;

        public CourseDAO(TeacherController teacherController)
        {
            _courseStorage = new Storage<Course>("course.csv");
            _courses = _courseStorage.Load();
            this.teacherController = teacherController;
        }

        private int GenerateCourseId()
        {
            if (_courses.Count == 0) return 0;
            return _courses.Last().Id + 1;
        }

        public Course AddCourse(Course course)
        {
            course.Id = GenerateCourseId();
            _courses.Add(course);
            _courseStorage.Save(_courses);
            NotifyObservers();
            return course;
        }

        public Course? UpdateCourse(Course course)
        {
            Course? oldCourse = GetCourseById(course.Id);
            if (oldCourse == null) return null;

            oldCourse.Language = course.Language;
            oldCourse.Level = course.Level;
            oldCourse.Duration = course.Duration;
            oldCourse.WorkDays = course.WorkDays;
            oldCourse.StartDate = course.StartDate;
            oldCourse.IsOnline = course.IsOnline;
            oldCourse.CurrentlyEnrolled = course.CurrentlyEnrolled;
            oldCourse.MaxEnrolledStudents = course.MaxEnrolledStudents;
            oldCourse.ExamTerms = course.ExamTerms;

            _courseStorage.Save(_courses);
            NotifyObservers();
            return oldCourse;
        }

        public Course? RemoveCourse(int id)
        {
            Course? course = GetCourseById(id);
            if (course == null) return null;

            _courses.Remove(course);
            foreach (int examTermId in course.ExamTerms)
            {
                teacherController.DeleteExamTerm(examTermId);
            }

            _courseStorage.Save(_courses);
            NotifyObservers();
            return course;
        }

        public Course? GetCourseById(int id)
        {
            return _courses.Find(v => v.Id == id);
        }

        public List<Course> GetAllCourses()
        {
            return _courses;
        }


        public List<Course> GetAvailableCourses(Teacher teacher)
        {
            List<Course> allCourses = GetAllCourses();
            List<int> allTeacherCourses = teacher.CoursesId;

            List<Course> availableCourses = new List<Course>();

            foreach (Course course in allCourses)
            {
                if (allTeacherCourses.Contains(course.Id))
                {
                    availableCourses.Add(course);
                }
            }
            return availableCourses;
        }

        public List<Course> FindCoursesByCriteria(Language? language, LanguageLevel? level, DateTime? startDate, int duration, bool? isOnline)
        {
            var filteredCourses = _courses.Where(course =>
                (!language.HasValue || course.Language == language.Value) &&
                (!level.HasValue || course.Level == level.Value) &&
                (!startDate.HasValue || course.StartDate.Date == (startDate.Value.Date)) &&
                (duration == 0 || course.Duration == duration) &&
                (!isOnline.HasValue || course.IsOnline == isOnline.Value)
            ).ToList();

            return filteredCourses;
        }
        public String FindLanguageAndLevel(int courseID)
        {
            String res = "";

            Course course = GetAllCourses().FirstOrDefault(c => c.Id == courseID);

            if (course != null)
            {
                res = $"{course.Language}, {course.Level}";
            }
            else
            {
                res = "Language and level not found";
            }

            return res;
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

        public bool CheckTeacherCoursesOverlap(Course course, Teacher teacher)
        {
            int courseDurationInMinutes = 90;

            DateTime courseStartTime = course.StartDate; // start of first ever course session 
            DateTime courseEndTime = course.StartDate.AddDays(course.Duration * 7).AddMinutes(courseDurationInMinutes); // end of last ever course session

            List<Course> teacherCourses = GetAvailableCourses(teacher);
            foreach (Course secondCourse in teacherCourses)
            {
                if (course.Id == secondCourse.Id)
                {
                    continue;
                }
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

            List<ExamTerm> teacherExams = teacherController.GetAvailableExamTerms(teacher);
            foreach (ExamTerm examTerm in teacherExams)
            {
                if (!course.WorkDays.Contains(examTerm.ExamTime.DayOfWeek))
                {
                    continue;
                }
                DateTime examStartTime = examTerm.ExamTime;
                DateTime examEndTime = examTerm.ExamTime.AddMinutes(examDurationInMinutes);

                DateTime maxStartTime = courseStartTime > examStartTime ? courseStartTime : examStartTime;
                DateTime minEndTime = courseEndTime < examEndTime ? courseEndTime : examEndTime;

                if ((courseStartTime == examStartTime || courseEndTime == examEndTime) ||
                    (maxStartTime < minEndTime))
                {

                    return true;
                }
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
                    {
                        return true;
                    }
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
                {
                    continue;
                }

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
        
        public bool IsStudentAccepted(Student student, int courseId)
        {
            List<Mail> sentMail = teacherController.GetAllMail();
            foreach (Mail mail in sentMail)
            {
                if (mail.Receiver == student.Email && mail.CourseId == courseId && mail.TypeOfMessage == TypeOfMessage.AcceptEnterCourseRequestMessage)
                {
                    return true;
                }
            }
            return false;
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

        private int GetCoursePenaltyPoints(int courseId)
        {
            DirectorService directorController = new DirectorService();
            Teacher teacher = directorController.GetTeacherByCourse(courseId);

            if (teacher == null) return 0;

            int penaltyPoints = 0;
            foreach (Mail mail in teacherController.GetSentCourseMail(teacher, courseId))
                if (mail.TypeOfMessage.Equals(TypeOfMessage.PenaltyPointMessage))
                    ++penaltyPoints;
            return penaltyPoints;
        }

        public Dictionary<Course, int> GetPenaltyPointsLastYearPerCourse()
        {
            Dictionary<Course, int> coursePenaltyPoints = new Dictionary<Course, int>();
            foreach (Course course in GetCoursesLastYear())
                coursePenaltyPoints[course] = GetCoursePenaltyPoints(course.Id);
            return coursePenaltyPoints;
        }
    }
}
