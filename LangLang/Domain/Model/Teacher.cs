using LangLang.Domain.Model.Enums;
using LangLang.Storage.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LangLang.Domain.Model
{
    public class Teacher : Employee, ISerializable
    {
        protected List<Language>? languages;
        protected List<LanguageLevel>? levelOfLanguages;
        protected DateTime startedWork;
        protected int averageRating;

        public List<Language>? Languages
        {
            get { return languages; }
            set { languages = value; }
        }

        public List<LanguageLevel>? LevelOfLanguages
        {
            get { return levelOfLanguages; }
            set { levelOfLanguages = value; }
        }

        public DateTime StartedWork
        {
            get { return startedWork; }
            set { startedWork = value; }
        }

        public int AverageRating
        {
            get { return averageRating; }
            set { averageRating = value; }
        }

        public Teacher() : base() { }

        public Teacher(int id, string firstName, string lastName, Gender gender, DateTime dateOfBirth, string phoneNumber, string email, string password,
                       int title, List<Language>? languages, List<LanguageLevel>? levelOfLanguages, DateTime startedWork, int averageRating)
                       : base(id, firstName, lastName, gender, dateOfBirth, phoneNumber, email, password, title)
        {
            this.languages = languages;
            this.levelOfLanguages = levelOfLanguages;
            this.startedWork = startedWork;
            this.averageRating = averageRating;
        }

        public override string[] ToCSV()
        {
            string languagesCsv = string.Join(",", languages ?? Enumerable.Empty<Language>());
            string levelOfLanguagesCsv = string.Join(",", levelOfLanguages ?? Enumerable.Empty<LanguageLevel>());
            string startedWorkString = startedWork.Date.ToString("yyyy-MM-dd");
            string dateOfBirthString = dateOfBirth.Date.ToString("yyyy-MM-dd");

            string coursesIdCsv = "";
            if (coursesId != null)
                coursesIdCsv = string.Join(",", coursesId);
            string examsIdStr = "";
            if (examsId != null)
                examsIdStr = string.Join(",", examsId);

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
                languagesCsv,
                levelOfLanguagesCsv,
                startedWorkString,
                averageRating.ToString(),
                coursesIdCsv,
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
            title = int.Parse(values[8]);

            languages = new List<Language>();
            foreach (string lang in values[9].Split(','))
            {
                languages.Add((Language)Enum.Parse(typeof(Language), lang));
            }

            levelOfLanguages = new List<LanguageLevel>();
            foreach (string level in values[10].Split(','))
            {
                levelOfLanguages.Add((LanguageLevel)Enum.Parse(typeof(LanguageLevel), level));
            }

            startedWork = DateTime.ParseExact(values[11], "yyyy-MM-dd", null);
            averageRating = int.Parse(values[12]);

            if (!string.IsNullOrEmpty(values[13]))
                coursesId = new List<int>(Array.ConvertAll(values[13].Split(','), int.Parse));
            else
                coursesId = new List<int>();

            if (!string.IsNullOrEmpty(values[14]))
                examsId = new List<int>(Array.ConvertAll(values[14].Split(','), int.Parse));
            else
                examsId = new List<int>();
        }
    }
}
