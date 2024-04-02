using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public TeacherDAO()
        {
            _courseStorage = new Storage<Course>("course.csv");
            _courses = _courseStorage.Load();
            _examTermsStorage = new Storage<ExamTerm>("exam.csv");
            _examTerms = _examTermsStorage.Load();
        }

        private int GenerateCourseId()
        {
            if (_courses.Count == 0) return 0;
            return _courses.Last().CourseID + 1;
        }

        private int GenerateExamId()
        {
            if (_examTerms.Count == 0) return 0;
            return _examTerms.Last().ExamID + 1;
        }

        public Course AddCourse(Course course)
        {
            course.CourseID = GenerateCourseId();
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

        public Course? UpdateCourse(Course course)
        {
            Course? oldCourse = GetCourseById(course.CourseID);
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
            /*foreach(int examTermId in course.ExamTerms)
            {
                // ToDo : Delete each term
            }*/

            _courseStorage.Save(_courses);
            NotifyObservers();
            return course;
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
            return _courses.Find(v => v.CourseID == id);
        }

        public ExamTerm GetExamTermById(int id)

        {
            return _examTerms.Find(et => et.ExamID == id);
        }
        public List<Course> GetAllCourses()
        {
            return _courses;
        }

        public List<ExamTerm> GetAllExamTerms()
        {
            return _examTerms;
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
       
    }
}
        