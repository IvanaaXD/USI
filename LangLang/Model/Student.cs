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
        private List<int> pendingCoursesIds = new List<int>();
        private List<int> pendingExamCoursesIds = new List<int>();
        private List<int> registeredExamIds = new List<int>();

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
        public List<int> PendingCoursesIds
        {
            get { return pendingCoursesIds; }
            set { pendingCoursesIds = value; }
        }
        public List<int> PendingExamCoursesIds
        {
            get { return pendingExamCoursesIds; }
            set { pendingExamCoursesIds = value; }
        }
        public List<int> RegisteredExamsIds
        {
            get { return registeredExamIds; }
            set { registeredExamIds = value; }
        }

        public Student() : base() { }

        public Student(string firstName, string lastName, Gender gender, DateTime dateOfBirth, string phoneNumber, string email, string password,
                       EducationLevel educationLevel, int penaltyPoints, int activeCourseId,
                       List<int> passedExamsIds, List<int> pendingCoursesIds, List<int> pendingExamCoursesIds, List<int> registeredExamIds, List<int> registeredExamsIds)
                       : base(firstName, lastName, gender, dateOfBirth, phoneNumber, email, password)
        {

            this.educationLevel = educationLevel;
            this.penaltyPoints = penaltyPoints;
            this.activeCourseId = activeCourseId;
            this.passedExamsIds = passedExamsIds;
            this.pendingCoursesIds = pendingCoursesIds;
            this.pendingExamCoursesIds = pendingExamCoursesIds;
            this.registeredExamIds = registeredExamIds;
            RegisteredExamsIds = registeredExamsIds;
        }
        public Student(int id, string firstName, string lastName, Gender gender, DateTime dateOfBirth, string phoneNumber, string email, string password,
                       EducationLevel educationLevel, int penaltyPoints, int activeCourseId,
                       List<int> passedExamsIds, List<int> pendingCoursesIds, List<int> pendingExamCoursesIds, List<int> registeredExamIds)
                       : base(id, firstName, lastName, gender, dateOfBirth, phoneNumber, email, password)
        {

            this.educationLevel = educationLevel;
            this.penaltyPoints = penaltyPoints;
            this.activeCourseId = activeCourseId;
            this.passedExamsIds = passedExamsIds;
            this.pendingCoursesIds = pendingCoursesIds;
            this.pendingExamCoursesIds = pendingExamCoursesIds;
            this.registeredExamIds = registeredExamIds;
        }

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
            string pendingCoursesIdsStr = string.Join(",", PendingCoursesIds);
            string pendingExamCoursesIdsStr = string.Join(",", PendingExamCoursesIds);
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
                pendingCoursesIdsStr,
                pendingExamCoursesIdsStr,
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
            {
                passedExamsIds = new List<int>(Array.ConvertAll(values[11].Split(','), int.Parse));
            }
            else
            {
                passedExamsIds = new List<int>();
            }

            if (!string.IsNullOrEmpty(values[12]))
            {
                pendingCoursesIds = new List<int>(Array.ConvertAll(values[12].Split(','), int.Parse));
            }
            else
            {
                pendingCoursesIds = new List<int>();
            }

            if (!string.IsNullOrEmpty(values[13]))
            {
                pendingExamCoursesIds = new List<int>(Array.ConvertAll(values[13].Split(','), int.Parse));
            }
            else
            {
                pendingExamCoursesIds = new List<int>();
            }
            if (!string.IsNullOrEmpty(values[14]))
            {
                pendingExamCoursesIds = new List<int>(Array.ConvertAll(values[14].Split(','), int.Parse));
            }
            else
            {
                pendingExamCoursesIds = new List<int>();
            }
        }
    }
}
