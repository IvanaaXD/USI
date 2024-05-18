using LangLang.Domain.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LangLang.DTO
{
    public class CourseGradeDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private int id;
        private int studentId;
        private int teacherId;
        private int courseId;
        private int studentActivityValue;
        private int studentKnowledgeValue;

        private string firstName;
        private string lastName;
        private string email;

        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        public int StudentId
        {
            get { return studentId; }
            set { SetProperty(ref studentId, value); }
        }

        public int TeacherId
        {
            get { return teacherId; }
            set { SetProperty(ref teacherId, value); }
        }

        public int CourseId
        {
            get { return courseId; }
            set { SetProperty(ref courseId, value); }
        }

        public int StudentActivityValue
        {
            get { return studentActivityValue; }
            set { SetProperty(ref studentActivityValue, value); }
        }
        public int StudentKnowledgeValue
        {
            get { return studentKnowledgeValue; }
            set { SetProperty(ref studentKnowledgeValue, value); }
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

        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }


        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "StudentActivityValue":
                        if (StudentActivityValue <= 0 || StudentActivityValue > 10)
                            return "Grade value must be between 1 and 10.";
                        break;
                    case "StudentKnowledgeValue":
                        if (StudentKnowledgeValue <= 0 || StudentKnowledgeValue > 10)
                            return "Grade value must be between 1 and 10.";
                        break;
                }

                return null;
            }
        }

        private readonly string[] _validatedProperties = { "StudentActivityValue", "StudentKnowledgeValue" };

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

        public CourseGrade ToCourseGrade()
        {
            return new CourseGrade
            {
                Id = id,
                TeacherId = teacherId,
                StudentId = studentId,
                CourseId = courseId,
                StudentActivityValue = studentActivityValue,
                StudentKnowledgeValue = studentKnowledgeValue
            };
        }

        public CourseGradeDTO() { }

        public CourseGradeDTO(CourseGrade grade, Student student)
        {
            id = grade.Id;
            studentId = grade.StudentId;
            teacherId = grade.TeacherId;
            courseId = grade.CourseId;
            studentActivityValue = grade.StudentActivityValue;
            studentKnowledgeValue = grade.StudentKnowledgeValue;
            firstName = student.FirstName;
            lastName = student.LastName;
            email = student.Email;
        }
    }
}
