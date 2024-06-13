using System;
using LangLang.Domain.Model;

namespace ConsoleLangLang
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Console Application!");

            // Assuming you have some way to get a Person object.
            // For demonstration, let's create a dummy Director object.
            Director director = new Director
            {
                Id = 1,
                FirstName = "John Doe"
            };

            // Call the Display method to start the CRUD operations for the given person.
            CRUDConsole.Display<Director>(director);
        }
    }
}
