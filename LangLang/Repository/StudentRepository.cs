using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.Domain.Model;
using LangLang.Storage;
using LangLang.Observer;
using LangLang.Controller;
using LangLang.Domain.IRepository;

namespace LangLang.Repository
{
    public class StudentRepository : Subject, IStudentRepository
    {
        private readonly List<Student> _students;
        private readonly Storage<Student> _storage;

        public StudentRepository()
        {
            _storage = new Storage<Student>("students.csv");
            _students = _storage.Load();

        }

        private int GenerateId()
        {
            if (_students.Count == 0) return 1;
            return _students.Last().Id + 1;
        }

        public Student AddStudent(Student student)
        {
            student.Id = GenerateId();
            _students.Add(student);
            _storage.Save(_students);
            NotifyObservers();
            return student;
        }

        public Student? UpdateStudent(Student student)
        {
            Student? oldStudent = GetStudentById(student.Id);
            if (oldStudent == null) return null;

            oldStudent.FirstName = student.FirstName;
            oldStudent.LastName = student.LastName;
            oldStudent.Gender = student.Gender;
            oldStudent.DateOfBirth = student.DateOfBirth;
            oldStudent.PhoneNumber = student.PhoneNumber;
            oldStudent.Email = student.Email;
            oldStudent.Password = student.Password;
            oldStudent.EducationLevel = student.EducationLevel;
            oldStudent.ActiveCourseId = student.ActiveCourseId;
            oldStudent.PassedExamsIds = student.PassedExamsIds;
            oldStudent.RegisteredCoursesIds = student.RegisteredCoursesIds;
            oldStudent.RegisteredExamsIds = student.RegisteredExamsIds;

            _storage.Save(_students);
            NotifyObservers();
            return oldStudent;
        }

        public Student? RemoveStudent(int id)
        {
            Student? student = GetStudentById(id);
            if (student == null) return null;

            _students.Remove(student);
            return student;
        }

        public Student? GetStudentById(int id)
        {
            return _students.Find(v => v.Id == id);
        }

        public List<Student> GetAllStudents()
        {
            return _students;
        }
    }
}
