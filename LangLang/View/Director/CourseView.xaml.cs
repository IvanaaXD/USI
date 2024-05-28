using LangLang.Controller;
using LangLang.Domain.Model;
using LangLang.Domain.Model.Enums;
using LangLang.DTO;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace LangLang.View.Director
{
    /// <summary>
    /// Interaction logic for CourseView.xaml
    /// </summary>
    public partial class CourseView : Window, IObserver
    {
        public ObservableCollection<StudentDTO> Students { get; set; }
        public class ViewModel
        {
            public ObservableCollection<StudentDTO> Students { get; set; }
            public ViewModel()
            {
                Students = new ObservableCollection<StudentDTO>();
            }
        }
        public ViewModel StudentsTableViewModel { get; set; }
        private CourseController courseController;
        private StudentsController studentController;
        private CourseGradeController courseGradeController;
        private MailController mailController;
        private Course course;
        private Domain.Model.Director director;

        private bool isFilterButtonClicked = false;
        public CourseView(int courseId, Domain.Model.Director director)
        {
            InitializeComponent();
            InitializeControllers();

            course = courseController.GetCourseById(courseId);
            this.director = director;

            StudentsTableViewModel = new ViewModel();
            AddCourseInfo();

            DataContext = this;
            Update();
        }
        private void InitializeControllers()
        {
            courseController = Injector.CreateInstance<CourseController>();
            studentController = Injector.CreateInstance<StudentsController>();
            courseGradeController = Injector.CreateInstance<CourseGradeController>();
            mailController = Injector.CreateInstance<MailController>();
        }
        private void SendMail_Click(object sender, EventArgs e)
        {
            foreach(Domain.Model.Student student in GetFilteredStudents())
            {
                string emailBody = $"Dear {student.FirstName}, Congratulations on being one of the top students in {course.Language} {course.Level}!" +
                                   $" Your hard work and dedication have truly paid off. Thank you for your outstanding performance. Best regards, {director.FirstName}";
                var examTerm = new ExamTerm();
                examTerm.ExamID = -1;
                mailController.ConstructMail(director, student, course, examTerm, TypeOfMessage.TopStudentsMessage,emailBody);
            }
            SendMailButton.Visibility = Visibility.Collapsed;
            MessageBox.Show("You have successfully sent your emails.");
        }

        public void Update()
        {
            try
            {
                StudentsTableViewModel.Students.Clear();
                var students = GetFilteredStudents();

                if (students != null)
                    foreach (Domain.Model.Student student in students)
                    {
                        StudentDTO dtoStudent = CreateStudentForTable(student);
                        StudentsTableViewModel.Students.Add(dtoStudent);
                    }
                else
                    MessageBox.Show("No students found.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        private void FilterButton_Click(object sender, EventArgs e)
        {
            Update();
            isFilterButtonClicked = true;
        }
        private void ResetButton_Click(object sender, EventArgs e)
        {
            isFilterButtonClicked = false;
            penaltiesTextBox.Text = string.Empty;
            studentsCountTextBox.Text = string.Empty;
            Update();
        }
        private List<Domain.Model.Student> GetFilteredStudents()
        {
            StudentGradePriority priority = knowledgeButton.IsChecked == true ? StudentGradePriority.Knowledge : StudentGradePriority.Activity;
            int penaltiesCount = 3;
            if (!string.IsNullOrEmpty(penaltiesTextBox.Text))
                if (int.TryParse(penaltiesTextBox.Text, out int penalties))
                    penaltiesCount = penalties;
            int studentsCount = 10;
            if (!string.IsNullOrEmpty(studentsCountTextBox.Text))
                if (int.TryParse(studentsCountTextBox.Text, out int maxStudents))
                    studentsCount = maxStudents;

            return DoFilter(priority,penaltiesCount,studentsCount);
        }

        private List<Domain.Model.Student> DoFilter(StudentGradePriority priority, int penalties, int maxStudents)
        {
            List<Domain.Model.Student> students = studentController.GetAllStudentsCompletedCourse(course.Id);

            if (!isFilterButtonClicked)
                return students;

            return courseGradeController.GetBestStudents(course.Id, students, priority, penalties,maxStudents);
        }
        private StudentDTO CreateStudentForTable(Domain.Model.Student student)
        {
            StudentDTO dtoStudent = new StudentDTO(student);

            CourseGrade grade = courseGradeController.GetCourseGradeByStudent(student.Id, course.Id);
            if (grade == null)
            {
                dtoStudent.ActivityGrade = 0;
                dtoStudent.KnowledgeGrade = 0;
            }
            else
            {
                dtoStudent.ActivityGrade = grade.StudentActivityValue;
                dtoStudent.KnowledgeGrade = grade.StudentKnowledgeValue;
            }

            dtoStudent.PenaltyPoints = studentController.GetPenaltyPointCount(student.Id);

            return dtoStudent;
        }
        private void AddCourseInfo()
        {
            courseLanguageTextBlock.Text = $"{course.Language}";
            courseLevelTextBlock.Text = $"{course.Level}";
            courseStartDateTextBlock.Text = course.StartDate.ToString("dd-MM-yyyy HH:mm");
            courseDurationTextBlock.Text = course.Duration.ToString();
            courseCurrentyEnrolledTextBlock.Text = course.CurrentlyEnrolled.ToString();
            AddCourseStatus();
        }

        private void AddCourseStatus()
        {
            string courseStatusCheck;

            if (courseController.HasStudentAcceptingPeriodEnded(course) && !courseController.HasCourseStarted(course))
                courseStatusCheck = "Final Student Enrollments";
            else if (courseController.HasStudentAcceptingPeriodEnded(course) && !courseController.HasGradingPeriodStarted(course))
                courseStatusCheck = "Course Active";
            else if (courseController.HasStudentAcceptingPeriodEnded(course) && !courseController.HasCourseFinished(course, courseController.GetStudentCount(studentController, course)))
                courseStatusCheck = "Student Grading Period";
            else if (courseController.HasCourseFinished(course, courseController.GetStudentCount(studentController, course)))
                courseStatusCheck = "Course Finished And Students Graded";
            else
                courseStatusCheck = "Requests Open For Students";

            courseStatus.Text = courseStatusCheck;
        }
        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
