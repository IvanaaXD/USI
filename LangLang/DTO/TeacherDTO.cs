using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using LangLang.Model.Enums;
using LangLang.Model;
using System.Runtime.CompilerServices;
using System.Linq;
using LangLang.Controller;


namespace LangLang.DTO
{
    public class TeacherDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private int id;
        private string firstName;
        private string lastName;
        private Gender gender;
        private DateTime dateOfBirth;
        private string phoneNumber;
        private string email;
        private string password;
        private int title;
        private DateTime startedWork;
        private int averageRating;

        private List<Language> languages;
        private List<LanguageLevel> levelOfLanguages;
        private List<int> coursesId;

        public List<string> LevelAndLanguages
        {
            get
            {
                List<string> languageLevelNames = new List<string>();

                var langs = Enum.GetValues(typeof(Language)).Cast<Language>().ToList();
                var levs = Enum.GetValues(typeof(LanguageLevel)).Cast<LanguageLevel>().ToList();

                foreach (var language in langs)
                {
                    if (language.Equals(Language.NULL))
                    {
                        continue;
                    } else
                    {
                        foreach (var level in levs)
                        {
                            if (level.Equals(LanguageLevel.NULL))
                            {
                                continue;
                            } else
                            {
                                languageLevelNames.Add($"{language} {level}");
                            }
                        }
                    }
                }
                return languageLevelNames;
            }
        }

        public List<Gender> GenderValues
        {
            get
            {
                var genders = Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList();
                return genders;
            }
        }

        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        public string FirstName
        {
            get { return firstName; }
            set { SetProperty(ref firstName, value); }
        }

        public string LastName
        {
            get { return lastName; }
            set { SetProperty(ref lastName, value); }
        }

        public Gender Gender
        {
            get { return gender; }
            set { SetProperty(ref gender, value); }
        }

        public DateTime DateOfBirth
        {
            get { return dateOfBirth; }
            set { SetProperty(ref dateOfBirth, value); }
        }

        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { SetProperty(ref phoneNumber, value); }
        }

        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }

        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }

        public int Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public DateTime StartedWork
        {
            get { return startedWork; }
            set { SetProperty(ref startedWork, value); }
        }

        public int AverageRating
        {
            get { return averageRating; }
            set { SetProperty(ref averageRating, value); }
        }

        public List<Language> Languages
        {
            get { return languages; }
            set { SetProperty(ref languages, value); }
        }

        public List<LanguageLevel> LevelOfLanguages
        {
            get { return levelOfLanguages; }
            set { SetProperty(ref levelOfLanguages, value); }
        }

        public List<int> CoursesId
        {
            get { return coursesId; }
            set { SetProperty(ref coursesId, value); }
        }

        public event PropertyChangedEventHandler ?PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public string? Error => null;

        private Regex _FirstNameRegex = new Regex(@"^[A-Za-z]+$");
        private Regex _LastNameRegex = new Regex(@"^[A-Za-z]+$");
        private Regex _PhoneNumberRegex = new Regex(@"^\d{9,15}$");
        private Regex _EmailRegex = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        private Regex _PasswordRegex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{5,}$");

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "FirstName":
                        if (string.IsNullOrEmpty(FirstName))
                            return "Name is required";

                        if (!_FirstNameRegex.IsMatch(FirstName))
                            return "Format not good. Try again.";
                        break;

                    case "LastName":
                        if (string.IsNullOrEmpty(LastName))
                            return "Name is required";

                        if (!_LastNameRegex.IsMatch(LastName))
                            return "Format not good. Try again.";
                        break;

                    case "DateOfBirth":
                        if (DateOfBirth > DateTime.Today)
                            return "Date of birth cannot be in the future";
                        if (DateOfBirth < DateTime.Today.AddYears(-65))
                            return "Date of birth cannot be more than 65 years in the past";
                        break;

                    case "StartedWork":
                        if (StartedWork > DateTime.Today)
                            return "Date of starting work cannot be in the future";
                        if (StartedWork <= DateOfBirth)
                            return "Date of starting work cannot be before the date of birth";
                        if (StartedWork < DateOfBirth.AddYears(18))
                            return "Date of starting work cannot be before the graduation";
                        break;

                    case "PhoneNumber":
                        if (string.IsNullOrEmpty(PhoneNumber))
                            return "Phone number is required";

                        if (!_PhoneNumberRegex.IsMatch(PhoneNumber))
                            return "Format not good. Try again.";
                        break;

                    case "Email":
                        if (string.IsNullOrEmpty(Email))
                            return "Email is required";

                        if (!_EmailRegex.IsMatch(Email))
                            return "Format not good. Try again.";

                        DirectorController directorController = new DirectorController();
                        StudentsController studentsController = new StudentsController();

                        foreach (Teacher teacher in directorController.GetAllTeachers())
                        {
                            if (teacher.Email.Equals(Email) && teacher.Id != Id)
                                return "Email already exists. Try again.";
                        }

                        foreach (Student student in studentsController.GetAllStudents())
                        {
                            if (student.Email.Equals(Email))
                                return "Email already exists. Try again.";
                        }
                        break;

                    case "Password":
                        if (string.IsNullOrEmpty(Password))
                            return "Password is required";

                        directorController = new DirectorController();
                        studentsController = new StudentsController();

                        foreach (Teacher teacher in directorController.GetAllTeachers())
                        {
                            if (teacher.Password.Equals(Password) && teacher.Id != Id)
                                return "Email already exists. Try again.";
                        }

                        foreach (Student student in studentsController.GetAllStudents())
                        {
                            if (student.Password.Equals(Password))
                                return "Email already exists. Try again.";
                        }
                        break;
                }
                return null;
            }
        }

        private readonly string[] _validatedProperties = { "FirstName", "LastName", "DateOfBirth", "StartedWork", "PhoneNumber", "Email", "Password"};

        public bool IsValid
        {
            get
            {
                foreach (var property in _validatedProperties)
                {
                    if (this[property] != null)
                        return false;
                }
                return true;
            }
        }

        public Teacher ToTeacher()
        {

            return new Teacher
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                DateOfBirth = dateOfBirth,
                PhoneNumber = phoneNumber,
                Email = email,
                Password = password,
                Title = 0,
                Languages = languages,
                LevelOfLanguages = levelOfLanguages,
                StartedWork = startedWork,
                AverageRating = averageRating,
                CoursesId = coursesId
            };
        }

        public TeacherDTO() { }

        public TeacherDTO(Student students) { }


        public TeacherDTO(Teacher teacher)
        {
            id = teacher.Id;
            firstName = teacher.FirstName;
            lastName = teacher.LastName;
            gender = teacher.Gender;
            dateOfBirth = teacher.DateOfBirth;
            phoneNumber = teacher.PhoneNumber;
            email = teacher.Email;
            password = teacher.Password;
            title = 0;
            startedWork = teacher.StartedWork;
            averageRating = teacher.AverageRating;

            languages = teacher.Languages;
            levelOfLanguages = teacher.LevelOfLanguages;
            coursesId = teacher.CoursesId;
        }
    }
}
