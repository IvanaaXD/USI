using System;
using System.Collections.Generic;
using LangLang.Domain.Model.Enums;
using LangLang.Storage.Serialization;

namespace LangLang.Domain.Model
{
    public class Director : Employee, ISerializable
    {
        public Director() : base() { }

        public Director(int id, string firstName, string lastName, Gender gender, DateTime dateOfBirth, string phoneNumber, string email,
        string password, int title)
        : base(id, firstName, lastName, gender, dateOfBirth, phoneNumber, email, password, title)
        {
        }

        public string[] ToCSV()
        {
            string coursesIdStr = "";
            if (coursesId != null)
                coursesIdStr = string.Join(",", coursesId);
            string examsIdStr = "";
            if (examsId != null)
                examsIdStr = string.Join(",", examsId);

            string dateOfBirthString = dateOfBirth.Date.ToString("yyyy-MM-dd");

            return new string[] {
                Id.ToString(),
                FirstName,
                LastName,
                Gender.ToString(),
                dateOfBirthString,
                PhoneNumber,
                Email,
                Password,
                Title.ToString(),
                coursesIdStr,
                examsIdStr
                };
        }

        public void FromCSV(string[] values)
        {
            id = int.Parse(values[0]);
            firstName = values[1];
            lastName = values[2];
            gender = (Gender)Enum.Parse(typeof(Gender), values[3]);
            dateOfBirth = DateTime.ParseExact(values[4], "yyyy-MM-dd", null);
            phoneNumber = values[5];
            email = values[6];
            password = values[7];
            title = int.Parse(values[8]);
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
