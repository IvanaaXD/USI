﻿using LangLang.Repository;
using LangLang.Domain.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.Domain.Model.Enums;

namespace LangLang.Controller
{
    public class TeacherController
    {
        private readonly TeacherDAO _teachers;
        private readonly ExamTermGradeRepository _examTermGrades;
        private readonly CourseGradeDAO _courseGrades;

        public TeacherController()
        {
            _teachers = new TeacherDAO();
            _examTermGrades = new ExamTermGradeRepository();
            _courseGrades = new CourseGradeDAO();
        }
        public Course? GetCourseById(int courseId)
        {
            return _teachers.GetCourseById(courseId);
        }
        public ExamTerm? GetExamTermById(int examId)
        {
            return _teachers.GetExamTermById(examId);
        }
        public Mail? GetMailById(int mailId)
        {
            return _teachers.GetMailById(mailId);
        }
        public List<Course> GetAllCourses()
        {
            return _teachers.GetAllCourses();
        }
        public List<ExamTerm> GetAllExamTerms()
        {
            return _teachers.GetAllExamTerms();
        }
        public List<Mail> GetAllMail()
        {
            return _teachers.GetAllMail();
        }
        public List<Mail> GetSentCourseMail(Teacher teacher, int courseId)
        {
            return _teachers.GetSentCourseMail(teacher, courseId);
        }
        public List<Mail> GetReceivedCourseMails(Teacher teacher, int courseId)
        {
            return _teachers.GetReceivedCourseMails(teacher, courseId);
        }

        public List<CourseGrade> GetAllCourseGrades()
        {
            return _courseGrades.GetAllCourseGrades();
        }

        public List<ExamTermGrade> GetAllExamTermGrades()
        {
            return _examTermGrades.GetAllExamTermGrades();
        }
        public List<CourseGrade> GetCourseGradesByTeacherCourse(int teacherId, int courseId)
        {
            return _courseGrades.GetCourseGradesByTeacherCourse(teacherId, courseId);
        }

        public List<ExamTermGrade> GetExamTermGradesByTeacherExam(int teacherId, int examTermId)
        {
            return _examTermGrades.GetExamTermGradesByTeacherExam(teacherId, examTermId);
        }
        public ExamTermGrade? GetExamTermGradeByStudentExam(int studentId, int examTermId)
        {
            return _examTermGrades.GetExamTermGradeByStudentExam(studentId, examTermId);
        }
        public CourseGrade? GetCourseGradesByStudentTeacherCourse(int studentId, int teacherId, int courseId)
        {
            return _courseGrades.GetCourseGradeByStudentTeacher(studentId, teacherId, courseId);
        }

        public ExamTermGrade? GetExamTermGradeByStudentTeacherExam(int  studentId, int teacherId, int examTermId) 
        {
            return _examTermGrades.GetExamTermGradeByStudentTeacherExam(studentId, teacherId, examTermId);
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
        public Mail SendMail(Mail mail)
        {
            return _teachers.SendMail(mail);
        }

        public Mail AnswerMail(int mailId)
        {
            return _teachers.AnswerMail(mailId);
        }

        public void DeleteMail(int mailId)
        {
            _teachers.RemoveMail(mailId);
        }

        public void UpdateCourse(Course course)
        {
            _teachers.UpdateCourse(course);
        }

        public void UpdateExamTerm(ExamTerm examTerm)
        {
            _teachers.UpdateExamTerm(examTerm);
        }

        public ExamTermGrade GradeStudent(ExamTermGrade grade)
        {
            return _examTermGrades.AddGrade(grade);
        }

        public CourseGrade GradeStudentCourse(CourseGrade grade)
        {
            return _courseGrades.AddGrade(grade);
        }

        public ExamTerm ConfirmExamTerm(int examTermId)
        {
            return _teachers.ConfirmExamTerm(examTermId);
        }
        public bool IsStudentGradedCourse(int studentId, int courseId)
        {
            return _courseGrades.IsStudentGraded(studentId, courseId);
        }

        public bool IsStudentGradedExamTerm(int studentId)
        {
            return _examTermGrades.IsStudentGraded(studentId);
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

            return !overlappingExams.Any();
        }
        public bool ValidateCourseTimeslot(Domain.Model.Course course, Domain.Model.Teacher teacher)
        {
            bool isOverlap = CheckCourseOverlap(course, teacher);
            if (!isOverlap)
                return isOverlap;
            return true;
        }
        private bool CheckCourseOverlap(Domain.Model.Course course, Domain.Model.Teacher teacher)
        {
            List<Domain.Model.Course> allAvailableCourses = _teachers.GetAllCourses();
            List<Domain.Model.ExamTerm> allAvailableExams = _teachers.GetAllExamTerms();

            bool isSameTeacherCourseOverlap = _teachers.CheckTeacherCoursesOverlap(course, teacher);
            if (isSameTeacherCourseOverlap)
                return false;

            bool isSameTeacherExamOverlap = _teachers.CheckTeacherCourseExamOverlap(course, teacher);
            if (isSameTeacherExamOverlap)
                return false;

            if (!course.IsOnline)
            {
                bool isClassroomOverlap = _teachers.CheckClassroomOverlap(course, allAvailableCourses, allAvailableExams);
                if (isClassroomOverlap)
                    return false;
            }
            return true;

        }
        public bool ValidateExamTimeslot(ExamTerm exam, Teacher teacher)
        {
            bool isOverlap = CheckExamOverlap(exam, teacher);
            if (!isOverlap)
                return isOverlap;
            return true;
        }
        private bool CheckExamOverlap(ExamTerm exam, Teacher teacher)
        {
            bool isSameTeacherCourseOverlap = _teachers.CheckTeacherExamOverlapsCourses(exam, teacher);
            if (isSameTeacherCourseOverlap)
                return false;

            bool isSameTeacherExamOverlap = _teachers.CheckTeacherExamsOverlap(exam, teacher);
            if (isSameTeacherExamOverlap)
                return false;

            return true;

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

        public void IncrementCourseCurrentlyEnrolled(int courseId)
        {
            _teachers.IncrementCourseCurrentlyEnrolled(courseId);
        }

        public void DecrementCourseCurrentlyEnrolled(int courseId)
        {
            _teachers.DecrementCourseCurrentlyEnrolled(courseId);
        }

        public List<Course> FindCoursesByCriteria(Language? language, LanguageLevel? level, DateTime? startDate, int duration, bool? isOnline)
        {
            return _teachers.FindCoursesByCriteria(language, level, startDate, duration, isOnline);
        }
        public List<ExamTerm> FindExamTermsByCriteria(Language? language, LanguageLevel? level, DateTime? examDate)
        {
            return _teachers.FindExamTermsByCriteria(language, level, examDate);
        }

        public bool IsStudentAccepted(Student student, int courseId)
        {
            return _teachers.IsStudentAccepted(student, courseId);
        }
        public List<ExamTerm> GetAvailableExamTerms(Teacher teacher)
        {
            return _teachers.GetAvailableExamTerms(teacher);
        }
        public List<Course>? GetCoursesForDisplay(List<Course> availableCourses, Language? selectedLanguage, LanguageLevel? selectedLevel, DateTime? selectedStartDate, int selectedDuration, bool isOnline)
        {
            return _teachers.GetCoursesForDisplay(availableCourses, selectedLanguage, selectedLevel, selectedStartDate, selectedDuration, isOnline);
        }
    }
}
