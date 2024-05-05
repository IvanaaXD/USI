using LangLang.Model.Enums;
using System;
using LangLang.Storage.Serialization;
using LangLang.Controller;

namespace LangLang.Model
{
    public class Mail : ISerializable
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
            set { id = value; }
        }
        public string Sender
        {
            get { return sender; }
            set { sender = value; }
        }
        public string Receiver
        {
            get { return receiver; }
            set { receiver = value; }
        }
        public int CourseId
        {
            get { return courseId; }
            set { courseId = value; }
        }

        public TypeOfMessage TypeOfMessage
        {
            get { return typeOfMessage; }
            set { typeOfMessage = value; }
        }
        public DateTime DateOfMessage
        {
            get { return dateOfMessage; }
            set { dateOfMessage = value; }
        }
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        public bool Answered
        {
            get { return answered; }
            set { answered = value; }
        }

        public Mail() { }

        public Mail(int id, string sender, string receiver, int courseId, TypeOfMessage typeOfMessage, DateTime dateOfMessage, string message, bool answered)
        {

            this.id = id;
            this.sender = sender;
            this.receiver = receiver;
            this.courseId = courseId;
            this.typeOfMessage = typeOfMessage;
            this.dateOfMessage = dateOfMessage;
            this.message = message;
            this.answered = answered;
        }

        public string[] ToCSV()
        {
            string[] csvValues =
            {
                Id.ToString(),
                Sender,
                Receiver,
                CourseId.ToString(),
                TypeOfMessage.ToString(),
                DateOfMessage.ToString("yyyy-MM-dd"),
                Message,
                Answered.ToString()
            };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            if (values.Length != 8)
            {
                throw new ArgumentException("Invalid number of mail values in CSV");
            }

            id = int.Parse(values[0]);

            sender = values[1];
            receiver = values[2];
            courseId = int.Parse(values[3]);
            typeOfMessage = (TypeOfMessage)Enum.Parse(typeof(TypeOfMessage), values[4]);
            dateOfMessage = DateTime.ParseExact(values[5], "yyyy-MM-dd", null);
            message = values[6];
            answered = bool.Parse(values[7]);
        }
    }
}
