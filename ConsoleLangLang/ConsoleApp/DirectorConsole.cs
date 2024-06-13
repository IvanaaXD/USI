using LangLang.Controller;
using LangLang.Domain.Model;
using System;

namespace LangLang.ConsoleApp
{
    public class DirectorConsole
    {
        private static Director director;
        private static DirectorController directorController = Injector.CreateInstance<DirectorController>();
        public static void Display()
        {
            director = directorController.GetDirector();

            while (true)
            {
                Console.WriteLine("Choose an operation:\n" +
                                  "\t1) CRUD operations\n" +
                                  "\t2) Smart selection of course teacher\n" +
                                  "\t3) Smart selection of the theacher on the exam\n" +
                                  "\t4) Exit");

                string operation = Console.ReadLine().ToLower();

                switch (operation)
                {
                    case "1":
                        CRUDConsole.Display(director);
                        break;
                    case "2":
                        return;
                    case "3":
                        return;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid operation.");
                        break;
                }
            }
        }

    }
}
