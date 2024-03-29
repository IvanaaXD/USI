using System;
using System.Collections.Generic;
using System.Reflection;
using LangLang.Model.Enums;

namespace LangLang.Model
{
    public class Employee : Person
    {
        protected int title;

        public int Title
        {
            get { return title; }
            set { title = value; }
        }

        public Employee() {}

        public Employee(int id, string firstName, string lastName, Gender gender, DateTime dateOfBirth, string phoneNumber, string email,
                        string password, int title)
                        : base(id, firstName, lastName, gender, dateOfBirth, phoneNumber, email, password)
        {
            this.title = title;
        }

        public override string[] ToCSV()
        {
            return new string[] {
                Id.ToString(),
                FirstName, 
                LastName, 
                Gender.ToString(), 
                DateOfBirth.ToString(), 
                PhoneNumber, 
                Email, 
                Password, 
                Title.ToString() };
        }

        public override void FromCSV(string[] values)
        {
            id = int.Parse(values[0]);
            firstName = values[1];
            lastName = values[2];
            gender = (Gender)Enum.Parse(typeof(Gender), values[3]);
            dateOfBirth = DateTime.ParseExact(values[4], "yyyy-MM-dd", null);
            phoneNumber = values[5];
            email = values[6];
            password = values[7];
            Title = int.Parse(values[8]);
        }
    }
}
