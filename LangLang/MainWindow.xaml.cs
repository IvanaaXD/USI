﻿using System.Windows;
using System.Windows.Input;
using LangLang.View.Director;
using LangLang.Controller;
using LangLang.Domain.Model;
using LangLang.View.Teacher;
using LangLang.View.Student;
using System.Diagnostics;
using System.IO;
using LangLang.Data;
using LangLang.Domain.Model.Enums;
using System;
using System.Collections.Generic;

namespace LangLang
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StudentsController studentController { get; set; }
        private DirectorController directorController { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            directorController = Injector.CreateInstance<DirectorController>();
            studentController = Injector.CreateInstance<StudentsController>();

            SetPlaceholders();

            /*using (var dbContext = new AppDbContext())
            {
                DateTime dateOfBirth = DateTime.ParseExact("1999-11-18", "yyyy-MM-dd", null).Date;
                DateTime specificDate = DateTime.ParseExact("2024-11-18", "yyyy-MM-dd", null).Date;
                List<Language> languages = new List<Language>
                {
                    Domain.Model.Enums.Language.English, Domain.Model.Enums.Language.German
                };
                List<LanguageLevel> levels = new List<LanguageLevel> { LanguageLevel.B2, LanguageLevel.A1 };

                Teacher t = new Teacher(1, "Mili", "bhbh", Gender.Female, dateOfBirth, "064544", "gvfc@bnam.com", "password", 0, languages, levels, specificDate, 1);

                dbContext.Teachers.Add(t);
                dbContext.SaveChanges();
            }*/
            /*
            using (var dbContext = new AppDbContext())
            {
                DateTime startDate = DateTime.ParseExact("2024-11-18", "yyyy-MM-dd", null).Date;
                List<DayOfWeek> daysOfWeek = new List<DayOfWeek> { DayOfWeek.Friday };
                Course t = new Course(1,Domain.Model.Enums.Language.German,LanguageLevel.C1,5,daysOfWeek,startDate,false,15,20);
                dbContext.Courses.Add(t);
                dbContext.SaveChanges();
            }*/
            /*
            using (var dbContext = new AppDbContext())
            {
                DateTime specificDate = DateTime.ParseExact("2024-11-18", "yyyy-MM-dd", null).Date;
                ExamTerm e = new ExamTerm(1, Domain.Model.Enums.Language.French, LanguageLevel.B1, 1, specificDate, 150, 0);
                dbContext.ExamTerms.Remove(e);

                dbContext.SaveChanges();
            }*/
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            /*string email = "ivana@gmail.com";
            string password = "ivana123";*/
            string email = Email.Text;
            string password = Password.Password;

            if (HasStudentLoggedIn(email, password) || HasTeacherLoggedIn(email, password) || HasDirectorLoggedIn(email, password))
            {
                this.Close();
                return;
            }
            
             MessageBox.Show("User does not exist.");
        } 

        private bool HasStudentLoggedIn(string email, string password)
        {
            foreach (Student student in studentController.GetAllStudents())
            {
                if (student.Email == email && student.Password == password)
                {
                    if (student.ActiveCourseId != -10)
                    {
                        studentController.ProcessPenaltyPoints();
                        StudentForm welcomePage = new StudentForm(student.Id);
                        welcomePage.Show();
                    }
                    else
                    {
                        MessageBox.Show("Your account has been deactivated.");
                    }

                    return true;
                }
            }
            return false;
        }

        private bool HasTeacherLoggedIn(string email, string password)
        {
            foreach (Teacher teacher in directorController.GetAllTeachers())
            {
                if (teacher.Email == email && teacher.Password == password)
                {
                    TeacherPage teacherPage = new TeacherPage(teacher.Id);
                    teacherPage.Show();
                    return true;
                }
            }
            return false;
        }

        private bool HasDirectorLoggedIn(string email, string password)
        {
            Director director = directorController.GetDirector();

            if (director.Email == email && director.Password == password)
            {
                DirectorPage directorPage = new DirectorPage(director.Id);
                directorPage.Show();
                return true;
            }
            return false;
        }

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            View.Student.RegistrationForm registrationForm = new View.Student.RegistrationForm(studentController);
            registrationForm.Show();
        }

        private void SetPlaceholders()
        {
            EmailPlaceholder.Visibility = Visibility.Visible;
            PasswordPlaceholder.Visibility = Visibility.Visible;

            Email.GotFocus += EmailTextBox_GotFocus;
            Password.GotFocus += PasswordBox_GotFocus;

            Email.LostFocus += EmailTextBox_LostFocus;
            Password.LostFocus += PasswordBox_LostFocus;

            EmailPlaceholder.MouseDown += Placeholder_MouseDown;
            PasswordPlaceholder.MouseDown += Placeholder_MouseDown;
        }

        private void EmailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            EmailPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void EmailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Email.Text))
                EmailPlaceholder.Visibility = Visibility.Visible;
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Password.Password))
                PasswordPlaceholder.Visibility = Visibility.Visible;
        }

        private void Placeholder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == EmailPlaceholder)
            {
                EmailPlaceholder.Visibility = Visibility.Collapsed;
                Email.Focus();
            }
            else if (sender == PasswordPlaceholder)
            {
                PasswordPlaceholder.Visibility = Visibility.Collapsed;
                Password.Focus();
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordPlaceholder != null)
                PasswordPlaceholder.Visibility = string.IsNullOrEmpty(Password.Password) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}

