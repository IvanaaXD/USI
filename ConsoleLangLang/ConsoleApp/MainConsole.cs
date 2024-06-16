using System;
using LangLang.Domain.Model;
using LangLang.Controller;
using LangLang.ConsoleApp;

public class MainConsole
{
    private static string currentUserEmail = null;
    private static DirectorController directorController = Injector.CreateInstance<DirectorController>();

    public static void Display()
    {
        Console.WriteLine("Welcome to the Console Application");

        while (true)
        {
            if (currentUserEmail == null)
            {
                bool enteredExit = Login();
                if (enteredExit) break;
            }
        }
        Console.WriteLine("Exiting the Console Application. Goodbye!");
    }

    private static bool Login()
    {
        Console.WriteLine("Please login to continue or type 'x' to quit.");

        while (true)
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();
            if (username == null) return false;

            if (username.ToLower() == "x")
                return true;

            Console.Write("Password: ");
            string password = Console.ReadLine();
            if (password == null) return false;

            if (Authenticate(username, password))
            {
                currentUserEmail = username;
                Console.WriteLine($"Logged in successfully\n");

                if (currentUserEmail == directorController.GetDirector().Email)
                    DirectorConsole.Display();
                else
                    TeacherConsole.Display(currentUserEmail);

                return true;
            }
            else
            {
                Console.WriteLine("Invalid username or password. Please try again.");
            }
        }
    }

    private static bool Authenticate(string email, string password)
    {
        return HasTeacherLoggedIn(email, password) || HasDirectorLoggedIn(email, password);
    }

    private static bool HasTeacherLoggedIn(string email, string password)
    {
        foreach (Teacher teacher in directorController.GetAllTeachers())
            if (teacher.Email == email && teacher.Password == password)
                return true;
        return false;
    }
    private static bool HasDirectorLoggedIn(string email, string password)
    {
        Director director = directorController.GetDirector();

        if (director.Email == email && director.Password == password)
            return true;
        return false;
    }
}
