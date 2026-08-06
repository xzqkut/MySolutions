using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Лямбда;

namespace Лямбда
{
    internal class Program
    {
        enum ColorCommand
        {
            Red = 1,
            Green = 2,
            Blue = 3
        }

        static void Main(string[] args)
        {
            const int CommandSetName = 1;
            const int CommandSetPassword = 2;
            const int CommandWriteName = 3;
            const int CommandConsoleColor = 4;
            const int CommandExit = 5;

        int enteredСommand = 0;
            string name = " ";
            string password = " ";
            string userInput;
            bool isRunning = true;

            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine("МЕНЮ");
                Console.WriteLine("////////////////////////////");
                Console.WriteLine($"{CommandSetName} - Установить имя");
                Console.WriteLine($"{CommandSetPassword} - Установить пароль");
                Console.WriteLine($"{CommandWriteName} - Вывести имя");
                Console.WriteLine($"{CommandConsoleColor} - Цвет консоли");
                Console.WriteLine($"{CommandExit} - Выход");
                Console.WriteLine("//////////////////////////\n");
                Console.WriteLine("Введите команду из пункта меню");
                
                enteredСommand = int.Parse(Console.ReadLine());

                switch (enteredСommand)
                {
                    case CommandSetName:
                        Console.Clear();
                        Console.WriteLine(CommandSetName);
                        Console.Write("Введите имя:");
                        name = Console.ReadLine();
                        Console.WriteLine($"Здравствуйте {name}");
                        PauseForUser();
                        break;

                    case CommandSetPassword:
                        Console.Clear();
                        Console.WriteLine(CommandSetPassword);
                        Console.WriteLine("Установите пароль:");
                        password = Console.ReadLine();
                        PauseForUser();
                        break;

                    case CommandWriteName:
                        Console.WriteLine(CommandWriteName);
                        Console.WriteLine("Введите пароль чтобы вывести имя:");
                        userInput = Console.ReadLine();

                        if (userInput != password)
                        {
                            Console.WriteLine("Неверный пароль.Доступ закрыт!");
                            PauseForUser();
                        }
                        else if (password == userInput)
                        {
                            Console.WriteLine($"Приветствую вас {name}");
                            PauseForUser();
                        }
                        break;

                    case CommandConsoleColor:
                        Console.WriteLine($"Выберите цвет: \n{(int)ColorCommand.Red})Red \n{(int)ColorCommand.Green})Green \n{(int)ColorCommand.Blue})Blue \n ");
                        int colorInput = Int32.Parse(Console.ReadLine());

                        switch ((ColorCommand)colorInput)
                        {
                            case ColorCommand.Red:
                                Console.BackgroundColor = ConsoleColor.Red;
                                break;
                            case ColorCommand.Green:
                                Console.BackgroundColor = ConsoleColor.Green;
                                break;
                            case ColorCommand.Blue:
                                Console.BackgroundColor = ConsoleColor.Blue;
                                break;
                            default:
                                Console.WriteLine("Неверный выбор.");
                                break;
                        }
                        break;

                    case CommandExit:
                        isRunning = false;
                        Console.WriteLine("Пока.");
                        break;

                    default:
                        Console.WriteLine("Неверный ввод");
                        break;
                }
            }
        }
            public static void PauseForUser()
            {
                Console.WriteLine("Для продолжения нажмите Enter.");
                Console.ReadKey();
            }
        }
    }


