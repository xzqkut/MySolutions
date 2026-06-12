using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquarium
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Simulation simulation = new Simulation(10);

            simulation.SimulateAquarium();
        }
    }

    class Fish
    {

        public Fish(int age, string name, int maxAge)
        {
            Age = age;
            Name = name;
            _maxAge = maxAge;
        }

        public int _maxAge { get; private set; }
        public int Age { get; private set; }
        public string Name { get; private set; }

        public bool IsAlive => Age < _maxAge;

        public int GetFishAge(Fish fish)
        {
            return fish.Age;
        }

        public void Grow()
        {
            if (IsAlive == false)
            {
                return;
            }
            Age++;
        }

    }

    class Aquarium
    {
        private List<Fish> _fish;
        public int MaxSlots { get; private set; }

        public Aquarium(int maxSlots = 10)
        {
            _fish = new List<Fish>();
            MaxSlots = maxSlots;
        }

        public bool IsEmpty => _fish.Count == 0;
        public bool TryAddFish(Fish fish)
        {
            if (_fish.Count >= MaxSlots)
            {
                Console.WriteLine("кол-во рыб в аквариуме уже максимально");
                return false;
               
            }

            _fish.Add(fish);
            return true;
        }

        public bool RemoveFish(int index)
        {
            int userIndex = index - 1;
            if (index < 0 || index >= _fish.Count)
                return false;

            string name = _fish[userIndex].Name;
            
            _fish.RemoveAt(userIndex);
            Console.WriteLine($"Вы достали рыбу {name}");
            return true;
        }

        public void Tick()
        {
            foreach (var fish in _fish)
            {
                fish.Grow();
                Console.WriteLine("Прошел один год...");
            }

            _fish.RemoveAll(fish => !fish.IsAlive);
        }

        public List<Fish> GetFish()
        {
            return _fish;
        }

        public void PrintState()
        {
            foreach (var fish in _fish)
            {
                Console.WriteLine($"{fish.Name} | Возраст:{fish.Age}, | Продолжительность жизни {fish._maxAge}");
            }
        }
    }

    class Simulation
    {

        private const int CommandAddFish = 1;
        private const int CommandRemoveFish = 2;
        private const int CommandTick = 3;
        private const int CommandExit = 4;

        private Aquarium _aquarium;
        private UserUtils _userUtils;
        private string[] _names;

        public Simulation(int maxSlots)
        {
            _aquarium = new Aquarium(maxSlots);
            _userUtils = new UserUtils();

            _names = new string[]
               {
                "карась","горбуша","карп"
                 ,"плотва","яззь","скумбрия"
                ,"форель","рыба-клоун","камбала"
                           ,"бычок"
                }
            ;
        }

        private void RunSimulate()
        {

            bool isOpen = true;

            while (isOpen)
            {
                Console.Clear();

                DrawMenu();
                DrawFish();

                string inputUser = Console.ReadLine();

                if (!int.TryParse(inputUser, out int command))
                {
                    Console.WriteLine("Неверная команда");
                    return;
                }

                switch (command)
                {
                    case CommandAddFish:
                        Console.WriteLine("Добавить рыбу");
                        int count = ReadFishCount();
                        CreateFish(count);
                        break;

                    case CommandRemoveFish:
                        bool removed = _aquarium.RemoveFish(AskFishIndexToRemove());

                        if (removed == false)
                            Console.WriteLine("Ошибка: неверный индекс");
                        else
                            Console.WriteLine("Рыба удалена");
                        break;
                    case CommandTick:
                        _aquarium.Tick();
                        break;
                    case CommandExit:
                        isOpen = false;
                        break;
                    default:
                        Console.WriteLine("Попробуйте еще раз, таких команд нету");
                        break;
                }

                if (_aquarium.IsEmpty)
                {
                    Console.WriteLine("\nВсе рыбы умерли. Симуляция завершена.");
                    break;
                }
            }
        }
        private void CreateFish(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int index = _userUtils.GetRandomValue(0, _names.Length);
                int age = _userUtils.GetRandomValue(0, 7);
                int maxAge = _userUtils.GetRandomValue(8, 12);
                Fish fish = new Fish(age, _names[index], maxAge);
                _aquarium.TryAddFish(fish);
            }
        }

        private int AskFishIndexToRemove()
        {
            while (true)
            {
                string input = Console.ReadLine();

                if (int.TryParse(input, out int index))
                {
                    return index;
                }
                Console.WriteLine("Ошибка");
            }
        }

        private int ReadFishCount()
        {
            while (true)
            {
                Console.WriteLine("Сколько рыб запустить в аквариум?");

                string input = Console.ReadLine();

                if (!int.TryParse(input, out int result))
                {
                    continue;
                }
                if (result > _aquarium.MaxSlots)
                {
                    Console.WriteLine($"Максимум рыб: {_aquarium.MaxSlots}");
                    continue;
                }
                return result;
            }
        }

        public void SimulateAquarium()
        {
            int count = ReadFishCount();

            CreateFish(count);

            _aquarium.PrintState();

            RunSimulate();
        }

        private void DrawFish()
        {
            int startX = 40;
            int y = 1;
            Console.SetCursorPosition(startX, y++);
            Console.WriteLine($"{new string('+', 15)} АКВАРИУМ {new string('+', 15)}");


            foreach (var fish in _aquarium.GetFish())
            {
                Console.SetCursorPosition(startX, y++);
                Console.WriteLine($"{fish.Name} | {fish.Age}/{fish._maxAge}");
            }
        }

        private void DrawMenu()
        {
            Console.SetCursorPosition(0, 0);

            Console.WriteLine($"{CommandAddFish} - Запустить рыбу");
            Console.WriteLine($"{CommandRemoveFish} - Достать рыбу");
            Console.WriteLine($"{CommandTick} - Наблюдать за рыбой");
            Console.WriteLine($"{CommandExit} - Отойти");
        }
    }


    class UserUtils
    {
        private Random _random = new Random();

        public int GetRandomValue(int min, int max)
        {
            return _random.Next(min, max);
        }
    }
}
