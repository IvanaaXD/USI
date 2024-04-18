using LangLang.Model.DAO;
using LangLang.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.Model.Enums;

namespace LangLang.Controller
{
    public class TeacherController
    {
        private readonly TeacherDAO _teachers;

        public TeacherController()
        {
            _teachers = new TeacherDAO();
        }
        public Course? GetCourseById(int courseID)
        {
            return _teachers.GetCourseById(courseID);
        }
        public ExamTerm? GetExamTermById(int examId)
        {
            return _teachers.GetExamTermById(examId);
        }
        public List<Course> GetAllCourses()
        {
            return _teachers.GetAllCourses();
        }
        public List<ExamTerm> GetAllExamTerms()
        {
            return _teachers.GetAllExamTerms();
        }
        public List<Course> GetAvailableCourses(Teacher teacher)
        {
            return _teachers.GetAvailableCourses(teacher);
        }
        public Course AddCourse(Course course)
        {
            return _teachers.AddCourse(course);
        }
        public void AddExamTerm(ExamTerm examTerm)
        {
            _teachers.AddExamTerm(examTerm);
        }

        public void UpdateCourse(Course course)
        {
            _teachers.UpdateCourse(course);
        }

        public void UpdateExamTerm(ExamTerm examTerm)
        {
            _teachers.UpdateExamTerm(examTerm);
        }
        public bool CheckExamOverlap(int ExamID, DateTime ExamDate)
        {
            int examDurationInMinutes = 240;

            DateTime examStartDateTime = ExamDate;
            DateTime examEndDateTime = examStartDateTime.AddMinutes(examDurationInMinutes);

            IEnumerable<dynamic> overlappingExams = _teachers.GetAllExamTerms()
                .Where(item =>
                {
                    bool isDifferentId = item.ExamID != ExamID;

                    DateTime itemExamDateTime = item.ExamTime;

                    bool isOverlap = isDifferentId && (itemExamDateTime < examEndDateTime && itemExamDateTime.AddMinutes(examDurationInMinutes) > examStartDateTime);

                    return isOverlap;
                });

            /*
            IEnumerable<dynamic> possibleOverlappingCourses =  _teachers.GetAllCourses()
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
    (IEnumerable<dynamic>)_teachers.GetAllCourses()
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
    (IEnumerable<dynamic>)_teachers.GetAllExamTerms()
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
                if (item.Id == course.Id)
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
            _teachers.RemoveCourse(courseId);
        }

        public void DeleteExamTerm(int examId)
        {
            _teachers.RemoveExamTerm(examId);
        }
        public void Subscribe(IObserver observer)
        {
            _teachers.Subscribe(observer);
        }

        public List<Course> FindCoursesByCriteria(Language? language, LanguageLevel? level, DateTime? startDate, int duration, bool? isOnline)
        {
            return _teachers.FindCoursesByCriteria(language, level, startDate, duration, isOnline);
        }
        public List<ExamTerm> FindExamTermsByCriteria(Language? language, LanguageLevel? level, DateTime? examDate)
        {
            return _teachers.FindExamTermsByCriteria(language, level, examDate);
        }
    }
}
