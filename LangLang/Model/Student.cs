using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using LangLang.Model.Enums;
using LangLang.Storage.Serialization;

namespace LangLang.Model
{
    public class Student : Person
    {
        private EducationLevel educationLevel;
        private int penaltyPoints;
        private int activeCourseId;
        private List<int> passedExamsIds = new List<int>();
        private List<int> registeredCoursesIds = new List<int>();
        private List<int> completedCoursesIds = new List<int>();
        private List<int> registeredExamsIds = new List<int>();

        public EducationLevel EducationLevel
        {
            get { return educationLevel; }
            set { educationLevel = value; }
        }
        public int PenaltyPoints
        {
            get { return penaltyPoints; }
            set { penaltyPoints = value; }
        }
        public int ActiveCourseId
        {
            get { return activeCourseId; }
            set { activeCourseId = value; }
        }
        public List<int> PassedExamsIds
        {
            get { return passedExamsIds; }
            set { passedExamsIds = value; }
        }
        public List<int> RegisteredCoursesIds
        {
            get { return registeredCoursesIds; }
            set { registeredCoursesIds = value; }
        }
        public List<int> CompletedCoursesIds
        {
            get { return completedCoursesIds; }
            set { completedCoursesIds = value; }
        }
        public List<int> RegisteredExamsIds
        {
            get { return registeredExamsIds; }
            set { registeredExamsIds = value; }
        }

        public Student() : base() { }

        public Student(string firstName, string lastName, Gender gender, DateTime dateOfBirth, string phoneNumber, string email, string password,
                       EducationLevel educationLevel)
                       : base(firstName, lastName, gender, dateOfBirth, phoneNumber, email, password)
        {
            this.educationLevel = educationLevel;
            this.penaltyPoints = 0;
            this.activeCourseId = -1;
        }
        public Student(int id, string firstName, string lastName, Gender gender, DateTime dateOfBirth, string phoneNumber, string email, string password,
                      EducationLevel educationLevel)
                      : base(id, firstName, lastName, gender, dateOfBirth, phoneNumber, email, password)
        {
            this.educationLevel = educationLevel;
            this.penaltyPoints = 0;
            this.activeCourseId = -1;
        }

        public override string[] ToCSV()
        {
            string passedExamsIdsStr = string.Join(",", PassedExamsIds);
            string registeredCoursesIdsStr = string.Join(",", RegisteredCoursesIds);
            string completedCoursesIdsStr = string.Join(",", CompletedCoursesIds);
            string registeredExamsIdsStr = string.Join(",", RegisteredExamsIds);


            string[] csvValues =
            {
                Id.ToString(),
                FirstName,
                LastName,
                Gender.ToString(),
                DateOfBirth.ToString("yyyy-MM-dd"),
                PhoneNumber,
                Email,
                Password,
                EducationLevel.ToString(),
                penaltyPoints.ToString(),
                activeCourseId.ToString(),
                passedExamsIdsStr,
                registeredCoursesIdsStr,
                completedCoursesIdsStr,
                registeredExamsIdsStr
            };

            return csvValues;
        }

        public override void FromCSV(string[] values)
        {
            if (values.Length != 15)
            {
                throw new ArgumentException("Invalid number of student values in CSV");
            }

            id = int.Parse(values[0]);
            firstName = values[1];
            lastName = values[2];
            gender = (Gender)Enum.Parse(typeof(Gender), values[3]);
            dateOfBirth = DateTime.ParseExact(values[4], "yyyy-MM-dd", null);
            phoneNumber = values[5];
            email = values[6];
            password = values[7];
            educationLevel = (EducationLevel)Enum.Parse(typeof(EducationLevel), values[8]);
            penaltyPoints = int.Parse(values[9]);
            activeCourseId = int.Parse(values[10]);

            if (!string.IsNullOrEmpty(values[11]))
                passedExamsIds = new List<int>(Array.ConvertAll(values[11].Split(','), int.Parse));
            else
                passedExamsIds = new List<int>();

            if (!string.IsNullOrEmpty(values[12]))
                registeredCoursesIds = new List<int>(Array.ConvertAll(values[12].Split(','), int.Parse));
            else
                registeredCoursesIds = new List<int>();

            if (!string.IsNullOrEmpty(values[13]))
                completedCoursesIds = new List<int>(Array.ConvertAll(values[13].Split(','), int.Parse));
            else
                completedCoursesIds = new List<int>();

            if (!string.IsNullOrEmpty(values[14]))
                registeredExamsIds = new List<int>(Array.ConvertAll(values[14].Split(','), int.Parse));
            else
                registeredExamsIds = new List<int>();
        }
    }
}
