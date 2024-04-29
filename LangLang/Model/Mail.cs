using LangLang.Model.Enums;
using System;
using LangLang.Storage.Serialization;


namespace LangLang.Model
{
    public class Mail : ISerializable
    {
        private int id;
        private string sender;
        private string recevier;
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
        public string Recevier
        {
            get { return recevier; }
            set { recevier = value; }
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

        public Mail(int id, string sender, string recevier, TypeOfMessage typeOfMessage, DateTime dateOfMessage, string message, bool answered)
        {

            this.id = id;
            this.sender = sender;
            this.recevier = recevier;
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
                Recevier,
                TypeOfMessage.ToString(),
                DateOfMessage.ToString("yyyy-MM-dd"),
                Message,
                Answered.ToString()
            };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            if (values.Length != 7)
            {
                throw new ArgumentException("Invalid number of student values in CSV");
            }

            id = int.Parse(values[0]);
            sender = values[1];
            recevier = values[2];
            typeOfMessage = (TypeOfMessage)Enum.Parse(typeof(TypeOfMessage), values[3]);
            dateOfMessage = DateTime.ParseExact(values[4], "yyyy-MM-dd", null);
            message = values[5];
            answered = bool.Parse(values[6]);
        }
    }
}
