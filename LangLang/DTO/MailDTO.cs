using LangLang.Controller;
using LangLang.Model.Enums;
using LangLang.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LangLang.DTO
{
    internal class MailDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private int id;
        private string sender;
        private string recevier;
        private TypeOfMessage typeOfMessage;
        private DateTime dateOfMessage;
        private string message;
        private bool answered;

        private readonly StudentsController _studentController;
        private readonly TeacherController _teacherController;
        private readonly DirectorController _directorController;

        public MailDTO(MainController mainController)
        {
            _studentController = mainController.GetStudentController();
            _teacherController = mainController.GetTeacherController();
            _directorController = mainController.GetDirectorController();
        }

        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        public string Sender
        {
            get { return sender; }
            set { SetProperty(ref sender, value); }
        }

        public string Recevier
        {
            get { return recevier; }
            set { SetProperty(ref recevier, value); }
        }

        public TypeOfMessage TypeOfMessage
        {
            get { return typeOfMessage; }
            set { SetProperty(ref typeOfMessage, value); }
        }

        public DateTime DateOfMessage
        {
            get { return dateOfMessage; }
            set { SetProperty(ref dateOfMessage, value); }
        }

        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public bool Answered
        {
            get { return answered; }
            set { SetProperty(ref answered, value); }
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
                }

                return null;
            }
        }

        private readonly string[] _validatedProperties = {};

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

        public Mail ToMail()
        {
            return new Mail
            {
                Id = id,
                Sender = sender,
                Recevier = recevier,
                TypeOfMessage = typeOfMessage,
                DateOfMessage = dateOfMessage,
                Message = message,
                Answered = answered
            };
        }

        public MailDTO() {}

        // add MainController mainController, 
        // _teacherController = tc;

        public MailDTO(Mail mail)
        {
            id = mail.Id;
            sender = mail.Sender;
            recevier = mail.Recevier;
            typeOfMessage = mail.TypeOfMessage;
            dateOfMessage = mail.DateOfMessage;
            message = mail.Message;
            answered = mail.Answered;
        }
    }
}
