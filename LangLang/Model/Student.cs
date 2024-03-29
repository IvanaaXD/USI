using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LangLang.Model.Enums;

namespace LangLang.Model
{
    public class Student : Person
    {
        private EducationLevel educationLevel;
        private int penaltyPoints;
        private int activeCourseId;
        private List<int> passedExamsIds = new List<int>();
        private List<int> pendingCoursesIds = new List<int>();

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
        }
        public List<int> PendingCoursesIds
        {
            get { return pendingCoursesIds; }
            set { pendingCoursesIds = value; }
        }

        public Student() { }

        public Student(int id, string firstName, string lastName, Gender gender, DateTime dateOfBirth, string phoneNumber, string email, string password,
                       EducationLevel educationLevel, int penaltyPoints, int activeCourseId, List<int> passedExamsIds, List<int> pendingCoursesIds)
                       : base(firstName, lastName, gender, dateOfBirth, phoneNumber, email, password)
        {

            this.educationLevel = educationLevel;
            this.penaltyPoints = penaltyPoints;
            this.activeCourseId = activeCourseId;
            this.passedExamsIds = passedExamsIds;
            this.pendingCoursesIds = pendingCoursesIds;
        }
        public Student(int id, string firstName, string lastName, Gender gender, DateTime dateOfBirth, string phoneNumber, string email, string password,
                       EducationLevel educationLevel)
                       : base(firstName, lastName, gender, dateOfBirth, phoneNumber, email, password)
        {
            this.educationLevel = educationLevel;
            this.penaltyPoints = 0;
            this.activeCourseId = -1;
        }

        public override string[] ToCSV()
        {
            string passedExamsIdsStr = string.Join(",", PassedExamsIds);
            string pendingCoursesIdsStr = string.Join(",", PendingCoursesIds);

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
                pendingCoursesIdsStr
            };

            return csvValues;
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
            educationLevel = (EducationLevel)Enum.Parse(typeof(EducationLevel), values[8]);
            penaltyPoints = int.Parse(values[9]);
            activeCourseId = int.Parse(values[10]);
            passedExamsIds = new List<int>(Array.ConvertAll(values[11].Split(','), int.Parse));
            pendingCoursesIds = new List<int>(Array.ConvertAll(values[12].Split(','), int.Parse));
        }
    }
}
