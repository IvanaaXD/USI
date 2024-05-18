using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using LangLang.Model.Enums;
using System.Xml.Linq;
using LangLang.Model;
using System.Security;

namespace LangLang.DTO
{
    public class StudentDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        public int id { get; set; }
        private string firstName;
        private string lastName;
        private Gender gender;
        private DateTime dateOfBirth = new DateTime(2000, 1, 1);
        private string phoneNumber;
        private string email;
        private string password;
        private EducationLevel educationLevel;
        private int penaltyPoints;
        private bool addedToCourse;

        public string FirstName
        {
            get { return firstName; }
            set
            {
                if (value != firstName)
                {
                    firstName = value;
                    OnPropertyChanged("FirstName");
                }
            }
        }
        public string LastName
        {
            get { return lastName; }
            set
            {
                if (value != lastName)
                {
                    lastName = value;
                    OnPropertyChanged("LastName");
                }
            }
        }
        public Gender Gender
        {
            get { return gender; }
            set
            {
                gender = value;
                OnPropertyChanged("Gender");
            }
        }

        public DateTime DateOfBirth
        {
            get { return dateOfBirth; }
            set
            {
                dateOfBirth = value;
                OnPropertyChanged("DateOfBirth");
            }
        }

        public string PhoneNumber
        {
            get { return phoneNumber; }
            set
            {
                phoneNumber = value;
                OnPropertyChanged("PhoneNumber");
            }
        }

        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                OnPropertyChanged("Email");
            }
        }

        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }
        public EducationLevel SelectedEducationLevel
        {
            get { return educationLevel; }
            set
            {
                educationLevel = value;
                OnPropertyChanged("SelectedEducationLevel");
            }
        }

        public int PenaltyPoints
        {
            get { return penaltyPoints; }
            set
            {
                penaltyPoints = value;
                OnPropertyChanged("PenaltyPoints");
            }
        }

        public bool AddedToCourse
        {
            get { return addedToCourse; }
            set
            {
                addedToCourse = value;
                OnPropertyChanged("AddedToCourse");
            }
        }
        public int Grade { get; set; }

        public int ExamTermGrade { get; set; }

        public string? Error => null;

        private Regex _FirstNameRegex = new Regex(@"^[A-Za-z]+$");
        private Regex _LastNameRegex = new Regex(@"^[A-Za-z]+$");
        private Regex _PhoneNumberRegex = new Regex(@"^\d{9,15}$");
        private Regex _EmailRegex = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        //private Regex _PasswordRegex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{5,}$"); // Minimum 8 characters, at least one letter and one number

        public string this[string columnName]
        {
            get
            {
                if (columnName == "FirstName")
                {
                    if (string.IsNullOrEmpty(FirstName))
                        return "First name is required";

                    Match match = _FirstNameRegex.Match(FirstName);
                    if (!match.Success)
                        return "Format not good. Try again.";

                }
                else if (columnName == "LastName")
                {
                    if (string.IsNullOrEmpty(LastName))
                        return "Last name is required";

                    Match match = _LastNameRegex.Match(LastName);
                    if (!match.Success)
                        return "Format not good. Try again.";

                }
                else if (columnName == "PhoneNumber")
                {
                    if (string.IsNullOrEmpty(PhoneNumber))
                        return "Phone number is required";

                    Match match = _PhoneNumberRegex.Match(PhoneNumber);
                    if (!match.Success)
                        return "Format not good. Try again.";

                }
                else if (columnName == "Email")
                {
                    if (string.IsNullOrEmpty(Email))
                        return "Email is required";

                    Match match = _EmailRegex.Match(Email);
                    if (!match.Success)
                        return "Format not good. Try again.";

                }
                else if (columnName == "Password")
                {
                    if (string.IsNullOrEmpty(Password))
                        return "Password is required";

                }
                else if (columnName == "DateOfBirth")
                {
                    if (DateOfBirth < new DateTime(1900, 1, 1) || DateOfBirth > DateTime.Today)
                        return "Invalid Date of birth.";
                }

                return null;
            }
        }

        private readonly string[] _validatedProperties = { "FirstName", "LastName", "PhoneNumber", "Email", "Password", "DateOfBirth" };

        public bool IsValid
        {
            get
            {
                foreach (var property in _validatedProperties)
                {
                    Console.WriteLine(property.ToString());
                    if (this[property] != null)
                        return false;
                }

                return true;
            }
        }

        public Student ToStudent()
        {
            return new Student(firstName, lastName, gender, dateOfBirth, phoneNumber, email, password, educationLevel);
        }

        public StudentDTO()
        {
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public StudentDTO(Student student)
        {
            id = student.Id;
            FirstName = student.FirstName;
            LastName = student.LastName;
            Gender = student.Gender;
            DateOfBirth = student.DateOfBirth;
            PhoneNumber = student.PhoneNumber;
            Email = student.Email;
            SelectedEducationLevel = student.EducationLevel;
            Password = student.Password;
            PenaltyPoints = student.PenaltyPoints;
        }

        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
