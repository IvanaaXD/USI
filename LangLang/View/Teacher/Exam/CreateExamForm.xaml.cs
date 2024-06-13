using LangLang.Controller;
using LangLang.DTO;
using LangLang.Domain.Model;
using LangLang.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;

namespace LangLang.View.Teacher
{
    public partial class CreateExamForm : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Language[] languageValues => (Language[])Enum.GetValues(typeof(Language));
        public LanguageLevel[] languageLevelValues => (LanguageLevel[])Enum.GetValues(typeof(LanguageLevel));

        private ExamTermDTO _examTerm;
        public TeacherDTO Teacher { get; set; }

        public ExamTermDTO CreatedExamTerm
        {
            get { return _examTerm; }
            set
            {
                _examTerm = value;
                OnPropertyChanged(nameof(CreatedExamTerm));
            }
        }

        private readonly TeacherController teacherController;
        private readonly CourseController courseController;
        private readonly ExamTermController examTermController;
        private readonly DirectorController directorController;
        private int teacherId;
        public CreateExamForm(int teacherId)
        {
            InitializeComponent();

            directorController = Injector.CreateInstance<DirectorController>();
            teacherController = Injector.CreateInstance<TeacherController>();
            courseController = Injector.CreateInstance<CourseController>();
            examTermController = Injector.CreateInstance<ExamTermController>();

            if (teacherId != -1)
            {
                Domain.Model.Teacher teacher = directorController.GetTeacherById(teacherId);
                CreatedExamTerm = new ExamTermDTO(teacherController, teacher);
                Teacher = new TeacherDTO(directorController.GetTeacherById(teacherId));
            }
            else
            {
                CreatedExamTerm = new ExamTermDTO();
            }


            this.teacherId = teacherId;
            DataContext = this;

            FillLanguageAndLevelCombobox();

            CreatedExamTerm.ExamDate = DateTime.Now;
            CreatedExamTerm.ExamTime = "10:00";
            CreatedExamTerm.MaxStudents = 80;
        }
        private void FillLanguageAndLevelCombobox()
        {
            List<Domain.Model.Teacher> teachers = directorController.GetAllTeachers();
            List<string> levelLanguageStr = new List<string>();
            if (teacherId == -1)
            {
                foreach (Domain.Model.Teacher teacher in teachers)
                {
                    for(int i = 0; i < teacher.Languages.Count; i++)
                    {
                        string languageLevel = $"{teacher.Languages[i]} {teacher.LevelOfLanguages[i]}";
                        if (!levelLanguageStr.Contains(languageLevel))
                            levelLanguageStr.Add(languageLevel);
                    }
                   
                }
            }
            else
            {
                for (int i = 0; i < Teacher.Languages.Count; i++)
                {
                    string languageLevel = $"{Teacher.Languages[i]} {Teacher.LevelOfLanguages[i]}";
                    if (!levelLanguageStr.Contains(languageLevel))
                        levelLanguageStr.Add(languageLevel);
                }
            }

            languageComboBox.ItemsSource = levelLanguageStr;
        }
        private void PickLanguageAndLevel()
        {
            if (languageComboBox.SelectedItem != null)
            {
                string selectedLanguageAndLevel = (string)languageComboBox.SelectedItem;
                SetLanguageAndLevel(selectedLanguageAndLevel);
            }
        }
        private void SetLanguageAndLevel(string selectedLanguageAndLevel)
        {
            Language lang = Domain.Model.Enums.Language.German;
            LanguageLevel lvl = LanguageLevel.A1;
            string[] parts = selectedLanguageAndLevel.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 2)
            {
                if (Enum.TryParse(parts[0], out Language language))
                    lang = language;
                else
                    MessageBox.Show($"Invalid language: {parts[0]}");
                if (Enum.TryParse(parts[1], out LanguageLevel level))
                    lvl = level;
                else
                    MessageBox.Show($"Invalid level: {parts[1]}");
                SetCourseForExamTerm(lang, lvl);
            }
            else
            {
                MessageBox.Show("Invalid language and level format.");
            }
        }
        public void SetCourseForExamTerm(Language lang, LanguageLevel lvl)
        {
            List<Course> courses;
            if (teacherId == -1)
            {
                courses = courseController.GetAllCourses();
            }
            else
            {
                Domain.Model.Teacher teacher = directorController.GetTeacherById(teacherId);
                courses = teacherController.GetAvailableCourses(teacher);
            }

            foreach (Course course in courses)
            {
                if (course.Language == lang && course.Level == lvl)
                { 
                    CreatedExamTerm.Language = lang;
                    CreatedExamTerm.Level = lvl;    
                    return;
                }
            }
            CreatedExamTerm.Language = lang;
            CreatedExamTerm.Level = lvl;
        }
        private void PickDataFromDatePicker()
        {
            if (dpExamDate.SelectedDate.HasValue && !string.IsNullOrWhiteSpace(txtExamTime.Text))
            {
                DateTime startDate = dpExamDate.SelectedDate.Value.Date;
                DateTime startTime;
                if (DateTime.TryParseExact(txtExamTime.Text, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
                {
                    CreatedExamTerm.ExamDate = startDate.Add(startTime.TimeOfDay);
                }
                else
                {
                    MessageBox.Show("Please enter a valid start time (HH:mm).");
                }
            }
            else
            {
                MessageBox.Show("Please select a valid start date and time.");
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {

            PickDataFromDatePicker();
            PickLanguageAndLevel();

            CreateExamTerm();

        }
        private void CreateExamTerm()
        {
            int createdExamTeacherId = -1;
            int examId = teacherController.GetAllExamTerms().Last().ExamID;
            ExamTerm examTerm = CreatedExamTerm.ToExamTermWithLanguage();
            if (teacherId == -1)
            {
                createdExamTeacherId = directorController.FindMostAppropriateTeacher(examTerm);
                if (createdExamTeacherId == -1)
                {
                    MessageBox.Show("There is no available teacher for that course");
                    return;
                }
            }
            Domain.Model.Teacher teacher;
            if (teacherId != -1)
                teacher = directorController.GetTeacherById(teacherId);
            else
                teacher = directorController.GetTeacherById(createdExamTeacherId);
            CreatedExamTerm.SetTeacher(teacher);

            if (CreatedExamTerm.IsValid)
            {
                bool foundMatchingCourse = false;
                foreach (int courseId in teacher.CoursesId)
                {
                    Course course = courseController.GetCourseById(courseId);
                    if (examTerm.Language == course.Language && examTerm.Level == course.Level)
                    {
                        courseController.Update(course);
                        foundMatchingCourse = true;
                        break;
                    }
                }

                teacher.ExamsId.Add(examId + 1);
                examTermController.Add(CreatedExamTerm.ToExamTermWithLanguage());

                directorController.Update(teacher);

                if (teacherId == -1)
                {
                    MessageBox.Show($"{teacher.FirstName} {teacher.LastName}", "Teacher who was chosen");

                    Domain.Model.Director director = directorController.GetDirector();
                    director.ExamsId.Add(examId + 1);
                    directorController.Update(director);
                    Close();
                }

            }
            else
            {
                MessageBox.Show("Exam cannot be created. Not all fields are valid.");
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
