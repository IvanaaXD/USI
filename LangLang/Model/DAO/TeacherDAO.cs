using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.Model.Enums;
using LangLang.Observer;
using LangLang.Storage;

namespace LangLang.Model.DAO
{
    public class TeacherDAO : Subject
    {
        private readonly List<Course> _courses;
        private readonly Storage<Course> _courseStorage;
        private readonly List<ExamTerm> _examTerms;
        private readonly Storage<ExamTerm> _examTermsStorage;
        private readonly List<Mail> _mails;
        private readonly Storage<Mail> _mailsStorage;

        public TeacherDAO()
        {
            _courseStorage = new Storage<Course>("course.csv");
            _courses = _courseStorage.Load();
            _examTermsStorage = new Storage<ExamTerm>("exam.csv");
            _examTerms = _examTermsStorage.Load();
            _mailsStorage = new Storage<Mail>("mails.csv");
            _mails = _mailsStorage.Load();
        }

        private int GenerateCourseId()
        {
            if (_courses.Count == 0) return 0;
            return _courses.Last().Id + 1;
        }

        private int GenerateExamId()
        {
            if (_examTerms.Count == 0) return 0;
            return _examTerms.Last().ExamID + 1;
        }
        private int GenerateMailId()
        {
            if (_mails.Count == 0) return 0;
            return _mails.Last().Id + 1;
        }

        public Course AddCourse(Course course)
        {
            course.Id = GenerateCourseId();
            _courses.Add(course);
            _courseStorage.Save(_courses);
            NotifyObservers();
            return course;
        }

        public ExamTerm AddExamTerm(ExamTerm examTerm)
        {
            examTerm.ExamID = GenerateExamId();
            _examTerms.Add(examTerm);
            _examTermsStorage.Save(_examTerms);
            NotifyObservers();
            return examTerm;
        }
        public Mail SendMail(Mail mail)
        {
            mail.Id = GenerateMailId();
            _mails.Add(mail);
            _mailsStorage.Save(_mails);
            NotifyObservers();
            return mail;
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

        public ExamTerm? UpdateExamTerm(ExamTerm examTerm)
        {
            ExamTerm? oldExamTerm = GetExamTermById(examTerm.ExamID);
            if (oldExamTerm == null) return null;

            oldExamTerm.CourseID = examTerm.CourseID;
            oldExamTerm.ExamTime = examTerm.ExamTime;
            oldExamTerm.MaxStudents = examTerm.MaxStudents;

            _examTermsStorage.Save(_examTerms);
            NotifyObservers();
            return oldExamTerm;
        }

        public Course? RemoveCourse(int id)
        {
            Course? course = GetCourseById(id);
            if (course == null) return null;

            _courses.Remove(course);
            foreach (int examTermId in course.ExamTerms)
            {
                RemoveExamTerm(examTermId);
            }

            _courseStorage.Save(_courses);
            NotifyObservers();
            return course;
        }
        public Mail? RemoveMail(int id)
        {
            Mail? mail = GetMailById(id);
            if (mail == null) return null;

            _mails.Remove(mail);
            _mailsStorage.Save(_mails);
            NotifyObservers();
            return mail;
        }
        public ExamTerm? RemoveExamTerm(int id)
        {
            ExamTerm? examTerm = GetExamTermById(id);
            if (examTerm == null) return null;

            _examTerms.Remove(examTerm);
            _examTermsStorage.Save(_examTerms);
            NotifyObservers();
            return examTerm;
        }

        public Course? GetCourseById(int id)
        {
            return _courses.Find(v => v.Id == id);
        }

        public ExamTerm GetExamTermById(int id)

        {
            return _examTerms.Find(et => et.ExamID == id);
        }
        public Mail? GetMailById(int id)
        {
            return _mails.Find(v => v.Id == id);
        }
        public List<Course> GetAllCourses()
        {
            return _courses;
        }

        public List<ExamTerm> GetAllExamTerms()
        {
            return _examTerms;
        }

        public List<Mail> GetAllMail()
        {
            return _mails;
        }


        public List<Mail> GetSentCourseMail(Teacher teacher, int courseId)
        {
            List<Mail> filteredMails = new List<Mail>();

            foreach (Mail mail in _mails)
            {
                if (mail.Sender == teacher.Email && mail.CourseId == courseId)
                {
                    filteredMails.Add(mail);
                }
            }
            return filteredMails;
        }

        public List<Mail> GetReceivedCourseMails(Teacher teacher, int courseId)
        {
            List<Mail> filteredMails = new List<Mail>();

            foreach (Mail mail in _mails)
            {
                if (mail.Receiver == teacher.Email && mail.CourseId == courseId)
                {
                    filteredMails.Add(mail);
                }
            }
            return filteredMails;
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

        public List<ExamTerm> GetAvailableExamTerms(Teacher teacher)
        {
            List<ExamTerm> allExamTerms = GetAllExamTerms();
            List<Course> allTeacherCourses = GetAvailableCourses(teacher);

            List<ExamTerm> availableExamTerms = new();
            List<int> examTermIds = new();

            foreach (Course course in allTeacherCourses)
            {
                foreach (int examId in course.ExamTerms)
                {
                    examTermIds.Add(examId);
                }
            }

            foreach (ExamTerm examTerm in allExamTerms)
            {
                if (examTermIds.Contains(examTerm.ExamID))
                {
                    availableExamTerms.Add(examTerm);
                }
            }

            return availableExamTerms;
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

        public List<ExamTerm> FindExamTermsByCriteria(Language? language, LanguageLevel? level, DateTime? examDate)
        {
            List<ExamTerm> allExams = GetAllExamTerms();

            var filteredExams = new List<ExamTerm>();

            foreach (var exam in allExams)
            {
                Course course = GetCourseById(exam.CourseID);

                bool matchesLanguage = !language.HasValue || course.Language == language;
                bool matchesLevel = !level.HasValue || course.Level == level;
                bool matchesExamDate = !examDate.HasValue || exam.ExamTime.Date == examDate.Value.Date;

                if (matchesLanguage && matchesLevel && matchesExamDate)
                {
                    filteredExams.Add(exam);
                }
            }

            return filteredExams;
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
        public void DecrementExamTermCurrentlyAttending(int examTermId)
        {
            ExamTerm examTerm = GetExamTermById(examTermId);
            --examTerm.CurrentlyAttending;
        }

        public ExamTerm ConfirmExamTerm(int examTermId)
        {
            ExamTerm examTerm = GetExamTermById(examTermId);
            examTerm.Confirmed = true;
            _examTermsStorage.Save(_examTerms);
            NotifyObservers();
            return examTerm;
        }

        public bool CheckTeacherCoursesOverlap(Course course, Teacher teacher)
        {
            int courseDurationInMinutes = 90;

            DateTime courseStartTime = course.StartDate; // start of first ever course session 
            DateTime courseEndTime = course.StartDate.AddDays(course.Duration * 7).AddMinutes(courseDurationInMinutes); // end of last ever course session

            List<Course> teacherCourses = GetAvailableCourses(teacher);
            foreach (Course secondCourse in teacherCourses)
            {
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

            List<ExamTerm> teacherExams = GetAvailableExamTerms(teacher);
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

        public bool CheckClassroomOverlap(Course course, List<Course> allAvailableCourses, List<ExamTerm> allAvailableExams)
        {
            bool isClassroomOneTaken = false;

            int courseDurationInMinutes = 90;
            int examDurationInMinutes = 240;

            DateTime courseStartTime = course.StartDate; // start of first ever course session 
            DateTime courseEndTime = course.StartDate.AddDays(course.Duration * 7).AddMinutes(courseDurationInMinutes); // end of last ever course session

            foreach (Course secondCourse in allAvailableCourses)
            {
                if (secondCourse.IsOnline)
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
    }
}
