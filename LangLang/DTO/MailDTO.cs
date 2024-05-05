using LangLang.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LangLang.Model;

namespace LangLang.DTO
{
    public class MailDTO : INotifyPropertyChanged, IDataErrorInfo
    {
        private int id;
        private string sender;
        private string receiver;
        private int courseId;
        private TypeOfMessage typeOfMessage;
        private DateTime dateOfMessage;
        private string message;
        private bool answered;

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

        public string Receiver
        {
            get { return receiver; }
            set { SetProperty(ref receiver, value); }
        }

        public int CourseId
        {
            get { return courseId; }
            set { SetProperty(ref courseId, value); }
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
                    case "Message":
                        if (Message == null || Message == "")
                            return "Message cannot have an empty body";
                        break;
                }

                return null;
            }
        }

        private readonly string[] _validatedProperties = { "Message" };

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
                Receiver = receiver,
                CourseId = courseId,
                TypeOfMessage = typeOfMessage,
                DateOfMessage = dateOfMessage,
                Message = message,
                Answered = answered,
            };
        }

        public MailDTO() { }

        // add MainController mainController, 
        // _teacherController = tc;

        public MailDTO(Mail mail)
        {
            id = mail.Id;
            sender = mail.Sender;
            receiver = mail.Receiver;
            courseId = mail.CourseId;
            typeOfMessage = mail.TypeOfMessage;
            dateOfMessage = mail.DateOfMessage;
            message = mail.Message;
            answered = mail.Answered;
        }
    }
}
