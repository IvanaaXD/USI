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
        private string firstName ;
        private string lastName;
        private Gender gender;
        private DateTime dateOfBirth;
        private string phoneNumber;
        private string email;
        private string password;
        private EducationLevel educationLevel;

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

        public string? Error => null;

        private Regex _FirstNameRegex = new Regex(@"^[A-Za-z]+$");
        private Regex _LastNameRegex = new Regex(@"^[A-Za-z]+$");
        private Regex _PhoneNumberRegex = new Regex(@"^\d{9,15}$");
        private Regex _EmailRegex = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        private Regex _PasswordRegex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{5,}$"); // Minimum 8 characters, at least one letter and one number

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
                    //string passwordStr = SecureStringHelper.ConvertToString(password);

                    if (string.IsNullOrEmpty(password))
                        return "Phone number is required";

                    Match match = _PasswordRegex.Match(password);
                    if (!match.Success)
                        return "Format not good. Try again.";

                }
                return null;
            }
        }

        private readonly string[] _validatedProperties = { "FirstName", "LastName", "PhoneNumber", "Email", "Password" };

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
            //string passwordStr = SecureStringHelper.ConvertToString(password);
            return new Student(firstName,lastName,gender,dateOfBirth,phoneNumber,email,password,educationLevel);
        }

        public StudentDTO()
        {
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public StudentDTO(Student student)
        {
            id = student.Id;
            firstName = student.FirstName;
            lastName = student.LastName;
            gender = student.Gender;
            dateOfBirth = student.DateOfBirth;
            phoneNumber = student.PhoneNumber;
            email = student.Email;
            educationLevel = student.EducationLevel;
            password = student.Password;//SecureStringHelper.ConvertToSecureString(student.Password);
        }

        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
