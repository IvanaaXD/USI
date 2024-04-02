using LangLang.Model.DAO;
using LangLang.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangLang.Model.Enums;

namespace LangLang.Controller
{
    public class TeacherController
    {
        private readonly TeacherDAO _coursesExams;

        public TeacherController()
        {
            _coursesExams = new TeacherDAO();
        }
        public Course GetCourseById(int courseID)
        {
            return _coursesExams.GetCourseById(courseID);
        }
        public ExamTerm? GetExamTermById(int examId)
        {
            return _coursesExams.GetExamTermById(examId);
        }
        public List<Course> GetAllCourses()
        {
            return _coursesExams.GetAllCourses();
        }
        public List<ExamTerm> GetAllExamTerms()
        {
            return _coursesExams.GetAllExamTerms();
        }
        public Course AddCourse(Course course)
        {
            return _coursesExams.AddCourse(course);
        }
        public void AddExamTerm(ExamTerm examTerm)
        {
            _coursesExams.AddExamTerm(examTerm);
        }

        public void UpdateCourse(Course course)
        {
            _coursesExams.UpdateCourse(course);
        }

        public void UpdateExamTerm(ExamTerm examTerm)
        {
            _coursesExams.UpdateExamTerm(examTerm);
        }
        public bool CheckExamOverlap(int ExamID, DateTime ExamDate)
        {
            int examDurationInMinutes = 240;

            DateTime examStartDateTime = ExamDate;
            DateTime examEndDateTime = examStartDateTime.AddMinutes(examDurationInMinutes);

            IEnumerable<dynamic> overlappingExams = _coursesExams.GetAllExamTerms()
                .Where(item =>
                {
                    bool isDifferentId = item.ExamID != ExamID;

                    DateTime itemExamDateTime = item.ExamTime;

                    bool isOverlap = isDifferentId && (itemExamDateTime < examEndDateTime && itemExamDateTime.AddMinutes(examDurationInMinutes) > examStartDateTime);

                    return isOverlap;
                });

            /*
            IEnumerable<dynamic> possibleOverlappingCourses =  _coursesExams.GetAllCourses()
        .Where(c =>
        {
            DateTime courseStartDateTime = c.StartDate;
            DateTime courseEndDateTime = c.StartDate.AddDays(c.Duration * 7);

            bool possible =
                (courseStartDateTime >= examStartDateTime && courseStartDateTime <= examEndDateTime) ||
                (courseEndDateTime >= examStartDateTime && courseEndDateTime <= examEndDateTime) ||
                (courseStartDateTime <= examStartDateTime && courseEndDateTime >= examEndDateTime);
            if (possible == true)
            {

                DayOfWeek dayOfWeek = ExamDate.DayOfWeek;
                if (c.WorkDays.Any(d => d == ExamDate.DayOfWeek))
                {
                    
                }
            }

                
            return true;
        });*/



            return !overlappingExams.Any();
        }
        public string ValidateCourseTimeslot(Course course)
        {
            string overlapResult = CheckOverlap(course, isCourse: true);
            if (overlapResult != null)
                return overlapResult;

            return CheckOverlap(course, isCourse: false);
        }
        private string CheckOverlap(Course course, bool isCourse)
        {
            DateTime sessionStartDateTime = course.StartDate;
            DateTime sessionEndDateTime = course.StartDate.AddDays(course.Duration * 7).AddMinutes(90);

            List<DayOfWeek> courseDays = course.WorkDays;
            int courseDurationInMinutes = 90;
            int examDurationInMinutes = 240;

            IEnumerable<dynamic> overlappingItems = isCourse ?
    (IEnumerable<dynamic>)_coursesExams.GetAllCourses()
    .Where(c =>
    {
        DateTime courseStartDateTime = c.StartDate;
        DateTime courseEndDateTime = c.StartDate.AddDays(c.Duration * 7);

        bool isOverlap =
            (courseStartDateTime >= sessionStartDateTime && courseStartDateTime <= sessionEndDateTime) ||
            (courseEndDateTime >= sessionStartDateTime && courseEndDateTime <= sessionEndDateTime) ||
            (courseStartDateTime <= sessionStartDateTime && courseEndDateTime >= sessionEndDateTime);

        return isOverlap;
    }) :
    (IEnumerable<dynamic>)_coursesExams.GetAllExamTerms()
    .Where(item =>
    {
        DateTime examDateTime = item.ExamTime;
        bool isOverlap = (examDateTime >= sessionStartDateTime && examDateTime <= sessionEndDateTime);

        return isOverlap;
    });


            var overlappingItemsFiltered = overlappingItems
            .Where(item =>
            {
                DateTime itemStartTime = isCourse ? item.StartDate : item.ExamTime;
                DateTime itemEndTime = itemStartTime.AddMinutes(isCourse ? courseDurationInMinutes : examDurationInMinutes);

                bool isOverlap = (sessionStartDateTime.TimeOfDay >= itemStartTime.TimeOfDay && sessionStartDateTime.TimeOfDay < itemEndTime.TimeOfDay) ||
                        (sessionStartDateTime.AddMinutes(courseDurationInMinutes).TimeOfDay > itemStartTime.TimeOfDay && sessionStartDateTime.AddMinutes(courseDurationInMinutes).TimeOfDay <= itemEndTime.TimeOfDay) ||
                        (sessionStartDateTime.TimeOfDay <= itemStartTime.TimeOfDay && sessionStartDateTime.AddMinutes(courseDurationInMinutes).TimeOfDay >= itemEndTime.TimeOfDay);
                return isOverlap;
            });

            int overlapCount = overlappingItemsFiltered.Count();

            int itemsToRemove = 0;

            foreach (var item in overlappingItemsFiltered)
            {
                if (item is ExamTerm)
                {
                    DayOfWeek hep = item.ExamTime.DayOfWeek;
                    if (!courseDays.Any(d => d == item.ExamTime.DayOfWeek))
                    {
                        itemsToRemove++;
                    }
                    continue;
                }
                if (item.CourseID == course.CourseID)
                {
                    itemsToRemove++;
                }
                bool hasMatchingDay = false;
                List<DayOfWeek> itemDayOfWeek = item.WorkDays;
                foreach (var day in courseDays)
                {
                    hasMatchingDay = itemDayOfWeek.Any(d => d == day);
                    if (hasMatchingDay)
                        break;
                }
                if (!hasMatchingDay)
                {
                    itemsToRemove++;
                }
            }
            overlapCount -= itemsToRemove;

            if ((isCourse && course.IsOnline && overlapCount > 0) ||
                (!isCourse && course.IsOnline && overlapCount > 0)) /*||
                (isCourse && !course.IsOnline && overlapCount >= 2))*/
            {
                return $"The timeslot on {sessionStartDateTime.DayOfWeek} at {sessionStartDateTime.TimeOfDay} for the selected duration is not available for {(isCourse ? "an online" : "a")} {(isCourse ? "course." : "exam term.")}";
            }

            return null;

        }

        public void DeleteCourse(int courseId)
        {
            _coursesExams.RemoveCourse(courseId);
        }

        public void DeleteExamTerm(int examId)
        {
            _coursesExams.RemoveExamTerm(examId);
        }
        public void Subscribe(IObserver observer)
        {
            _coursesExams.Subscribe(observer);
        }

        public List<Course> FindCoursesByCriteria(Language? language, LanguageLevel? level, DateTime? startDate, int duration, bool? isOnline)
        {
            return _coursesExams.FindCoursesByCriteria(language, level, startDate, duration, isOnline);
        }
        public List<ExamTerm> FindExamTermsByCriteria(Language? language, LanguageLevel? level, DateTime? examDate)
        {
            return _coursesExams.FindExamTermsByCriteria(language, level, examDate);
        }
    }
}
