using System;

public class MainConsole
{
    public static void Run()
    {
        while (true)
        {
            Console.WriteLine("Choose an operation: Create (c), Read (r), Update (u), Delete (d), Exit (e)");
            string operation = Console.ReadLine().ToLower();

            switch (operation)
            {
                case "c":
                   
                    break;
                case "r":
                   
                    break;
                case "e":
                    return;
                default:
                    Console.WriteLine("Invalid operation.");
                    break;
            }
        }
    }
}
