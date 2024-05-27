using System;
using System.Collections.Generic;
using LangLang.Domain.Model.Enums;

namespace LangLang.Domain.Model
{
    public class Employee : Person
    {
        protected int title;
        protected List<int>? coursesId;
        protected List<int>? examsId;

        public int Title
        {
            get { return title; }
            set { title = value; }
        }
        public List<int> CoursesId
        {
            get { return coursesId; }
            set { coursesId = value; }
        }
        public List<int> ExamsId
        {
            get { return examsId; }
            set { examsId = value; }
        }

        public Employee() : base() { }

        public Employee(int id, string firstName, string lastName, Gender gender, DateTime dateOfBirth, string phoneNumber, string email,
                        string password, int title)
                        : base(id, firstName, lastName, gender, dateOfBirth, phoneNumber, email, password)
        {
            this.title = title;
        }

        public override string[] ToCSV()
        {
            string coursesIdStr = "";
            if (coursesId != null)
                coursesIdStr = string.Join(",", coursesId);
            string examsIdStr = "";
            if (examsId != null)
                examsIdStr = string.Join(",", examsId);
            return new string[] {
                Id.ToString(),
                FirstName,
                LastName,
                Gender.ToString(),
                DateOfBirth.ToString(),
                PhoneNumber,
                Email,
                Password,
                Title.ToString(),
                coursesIdStr,
                examsIdStr
            };
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
            if (!string.IsNullOrEmpty(values[9]))
                coursesId = new List<int>(Array.ConvertAll(values[9].Split(','), int.Parse));
            else
                coursesId = new List<int>();
            if (!string.IsNullOrEmpty(values[10]))
                examsId = new List<int>(Array.ConvertAll(values[10].Split(','), int.Parse));
            else
                examsId = new List<int>();
        }
    }
}
