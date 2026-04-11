using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Arena arena = new Arena();

            arena.Run();
        }
    }


    public static class UserUtils
    {
        private static Random _random = new Random();
        public const int MinRandomNumber = 1;
        public const int MaxRandomNumber = 101;


        public static int GenerateRandomNumber()
        {
            return _random.Next(MinRandomNumber, MaxRandomNumber);
        }

    }

    class Arena
    {
        private const int FightCommand = 1;
        private const int ExitCommand = 2;

        private List<Fighter> _fighters;

        public Arena()
        {
            _fighters = new List<Fighter>();

            _fighters.Add(new Barbarian(50, 1000, 100));
            _fighters.Add(new Mage(30, 700, 50));
            _fighters.Add(new Rogue(40, 900, 120));
            _fighters.Add(new Ranger(45, 950, 150));
            _fighters.Add(new Monk(50, 1200, 100));
        }

        public void Run()
        {

            bool isWorking = true;

            while (isWorking)
            {
                Console.Clear();
                Console.WriteLine("Добро пожаловать на арену\nУчаствовать или убежать?-Введите 1 или 2");
                Console.WriteLine($"{FightCommand} - Начать бой");
                Console.WriteLine($"{ExitCommand} - Выход");

                string input = Console.ReadLine();
                if (int.TryParse(input, out int command))
                {
                    switch (command)
                    {
                        case FightCommand:
                            Console.Clear();
                            Menu();
                            break;
                        case ExitCommand:
                            Console.WriteLine("Конец игре.");
                            isWorking = false;
                            break;
                    }
                }
            }
        }

        private void ShowFightersHealth(Fighter first, Fighter second)
        {
            Console.WriteLine($"{first.TypeFighter}: {first.Health} HP");
            Console.WriteLine($"{second.TypeFighter}: {second.Health} HP");
            Console.WriteLine("----------------------");
        }
        public void StartFight(Fighter firstFighter, Fighter secondFighter)
        {
            while (firstFighter.IsAlive && secondFighter.IsAlive)
            {
                firstFighter.ProcessBurn();
                secondFighter.ProcessBurn();

                firstFighter.Attack(secondFighter);

                if (secondFighter.IsAlive)
                {
                    secondFighter.Attack(firstFighter);
                }
                Console.ReadKey();

                ShowFightersHealth(firstFighter, secondFighter);
            }
            if (firstFighter.IsAlive)
            {
                ShowWinner(firstFighter);
            }
            else
            {
                ShowWinner(secondFighter);
            }

            Console.WriteLine("\nНажмите любую клавишу чтобы вернуться в меню...");
            Console.ReadKey();

            firstFighter.Reset();
            secondFighter.Reset();

        }

        public void Menu()
        {
            Console.WriteLine("Вам предстоить выбрать бойцов для сражения.");
            ShowFighters();

            Console.WriteLine("\nВыберите первого бойца");

            Fighter firstFighter = ChooseFighter();

            Console.WriteLine("Выберите второго бойца");

            Fighter secondFighter = ChooseFighter();

            StartFight(firstFighter, secondFighter);

        }

        private void ShowWinner(Fighter fighter)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("===================================");
            Console.WriteLine($"        ПОБЕДИЛ {fighter.TypeFighter}");
            Console.WriteLine("===================================");

            Console.ResetColor();
        }
        private Fighter ChooseFighter()
        {
            while (true)
            {
                string chooseFighter = Console.ReadLine();

                if (int.TryParse(chooseFighter, out int index) && IsValidIndex(index))
                {
                    return _fighters[index - 1].Copy();
                }

                Console.WriteLine("неверный выбор попробуйте еще!");
            }
        }

        private bool IsValidIndex(int index)
        {
            return index > 0 && index <= _fighters.Count;
        }

        public void ShowFighters()
        {
            for (int i = 0; i < _fighters.Count; i++)
            {
                Console.WriteLine($"{i + 1}-{_fighters[i].TypeFighter}");
            }
        }
    }

    abstract class Fighter
    {
        private int _burnDamage = 0;
        private int _burnTurns = 0;

        public Fighter(int armor, int health, int damage, string typeFighter)
        {
            Armor = armor;
            Health = health;
            Damage = damage;
            TypeFighter = typeFighter;
            MaxHealth = health;
        }

        public int Armor { get; private set; }
        public int Health { get; private set; }
        public int Damage { get; private set; }
        public string TypeFighter { get; private set; }
        public bool IsAlive => Health > 0;
        protected int MaxHealth { get; private set; }

        public abstract Fighter Copy();


        public void ApplyBurn(int damage, int turns)
        {
            _burnDamage = damage; _burnTurns = turns;
        }

        public void ProcessBurn()
        {
            if (_burnTurns > 0)
            {
                Console.WriteLine($"{TypeFighter} получает урон от горения {_burnDamage}");
                TakeDamage(_burnDamage);
                _burnTurns--;
            }
        }

        public virtual void Reset()
        {
            Health = MaxHealth;
            _burnDamage = 0;
            _burnTurns = 0;
        }
        public virtual void Attack(Fighter enemy)
        {
            enemy.TakeDamage(Damage);
        }

        public virtual void TakeDamage(int damage)
        {
            int finalDamage = damage - Armor;
            if (finalDamage < 0)
            {
                finalDamage = 0;
            }
            Health -= finalDamage;
        }

        protected void Heal(int amount)
        {
            Health += amount;

            Console.WriteLine($"{TypeFighter} восстановил {amount} HP");
        }
    }

    class Mage : Fighter
    {
        private const int FireballCost = 50;
        private const int FireballDamage = 280;
        private const int MaxMana = 100;
        private const int ManaRegen = 20;

        private float _burnPercent = 0.1f;
        private int _burnDuration = 3;
        private int _mana = MaxMana;


        public Mage(int armor, int health, int damage) : base(armor, health, damage, "МАГ") { }

        public override Fighter Copy()
        {
            return new Mage(Armor, MaxHealth, Damage);
        }

        public override void Reset()
        {
            base.Reset();
            _mana = MaxMana;
        }

        public override void Attack(Fighter enemy)
        {
            _mana += ManaRegen;
            if (_mana > MaxMana)
            {
                _mana = MaxMana;
            }

            if (_mana >= FireballCost)
            {
                _mana -= FireballCost;

                Console.WriteLine($"Гориииии!- {TypeFighter} воспользовался заклинанием");

                enemy.TakeDamage(FireballDamage);

                int burnDamage = (int)(FireballDamage * _burnPercent);
                enemy.ApplyBurn(burnDamage, _burnDuration);
            }

            else
            {
                Console.WriteLine($"{TypeFighter} не хватает маны и он атакуюет палочкой");

                enemy.TakeDamage(Damage);

            }
        }
    }

    class Barbarian : Fighter
    {
        private const double RageStackBonus = 0.1;
        public Barbarian(int armor, int health, int damage) : base(armor, health, damage, "ВАРВАР") { }

        public override Fighter Copy()
        {
            return new Barbarian(Armor, MaxHealth, Damage);
        }

        public override void Attack(Fighter enemy)
        {
            int lostHealth = MaxHealth - Health;
            int stackRage = lostHealth / 100;
            int maxStackRage = 5;

            if (stackRage >= maxStackRage)
            {
                stackRage = maxStackRage;
            }

            double bonusDamage = Damage * stackRage * RageStackBonus;
            double finalDamage = bonusDamage + Damage;

            enemy.TakeDamage((int)finalDamage);
        }
    }

    class Rogue : Fighter
    {
        private const int _dodgeChance = 20;
        public Rogue(int armor, int health, int damage) : base(armor, health, damage, "Разбойник")
        {
        }

        public override Fighter Copy()
        {
            return new Rogue(Armor, MaxHealth, Damage);
        }
        public override void TakeDamage(int damage)
        {
            if (UserUtils.GenerateRandomNumber() < _dodgeChance)
            {
                Console.WriteLine($"{TypeFighter}, уклонился повезет в следующий раз -_0");
                return;
            }
            base.TakeDamage(damage);
        }
    }

    class Ranger : Fighter
    {
        private const int _chanceHeadshot = 10;
        private const double _crietMultiplier = 2.5;
        public Ranger(int armor, int health, int damage) : base(armor, health, damage, "Рейнджер")
        {
        }

        public override Fighter Copy()
        {
            return new Ranger(Armor, MaxHealth, Damage);
        }
        public override void Attack(Fighter enemy)
        {
            if (UserUtils.GenerateRandomNumber() < _chanceHeadshot)
            {
                Console.WriteLine("Сегодня везучий день для рейнджера и он бьет прямо в цель");
                int finalDamage = (int)(Damage * _crietMultiplier);
                enemy.TakeDamage(finalDamage);
            }
            else
            {
                enemy.TakeDamage(Damage);
            }
        }
    }

    class Monk : Fighter
    {
        private const int _heal = 100;

        private int _injuries = 0;
        private int _maxInjuries = 3;

        public Monk(int armor, int health, int damage) : base(armor, health, damage, "МОНАХ")
        {
        }

        public override Fighter Copy()
        {
            return new Monk(Armor, MaxHealth, Damage);
        }

        public override void Reset()
        {
            base.Reset();
            _injuries = 0;
        }

        public override void Attack(Fighter enemy)
        {
            enemy.TakeDamage(Damage);

            _injuries++;

            if (_injuries == _maxInjuries)
            {
                Heal(_heal);
                Console.WriteLine($"Монах медитирует и восстановил здоровье");
                _injuries = 0;
            }
        }
    }
}
