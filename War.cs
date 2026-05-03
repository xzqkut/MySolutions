using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace War
{
    internal class Program
    {
        static void Main(string[] args)
        {
            War war = new War();
            war.StartBattle();
        }
    }

    interface IAttackStrategy
    {
        void Attack(Soldier attacker, List<Soldier> enemies);
    }

    class War
    {
        private Platoon _platoon1;
        private Platoon _platoon2;

        public War() {
        }

        public void StartBattle()
        {
            SoldierFactory factory = new SoldierFactory();

            Console.WriteLine("Введите кол-во бойцов для 1 отряда");
            int countFirstPlatoon = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Введите кол-во бойцов для 2 отряда");
            int countSecondPlatoon = Convert.ToInt32(Console.ReadLine());

            _platoon1 = new Platoon(factory.CreateSquad(countFirstPlatoon));
            _platoon2 = new Platoon(factory.CreateSquad(countSecondPlatoon));

            Fight();
        }

        private void Fight()
        {
            int round = 1;
            while (_platoon1.HasSoldier && _platoon2.HasSoldier)
            {
                Console.WriteLine($"---Раунд{round}---");
                _platoon1.Attack(_platoon2);
                _platoon2 .Attack(_platoon1);

                _platoon1.RemoveDead();
                _platoon2.RemoveDead();

                round++;
                Console.ReadKey();

                if (_platoon1.HasSoldier)
                {
                    Console.WriteLine("Победил 1 отряд");
                }
                else if (_platoon2.HasSoldier)
                {
                    Console.WriteLine("победил 2 отряд");

                }
                else
                {
                    Console.WriteLine("Ничья");
                }
            }
        }
        private int ReadInt(string message)
        {
            while (true)
            {
                Console.WriteLine(message);
                if(int.TryParse(Console.ReadLine(), out int count))
                {
                    return count;
                }

                Console.WriteLine("Ошибка ввода");
            }
        }
    }

    class Platoon
    {
        private List<Soldier> _soldiers = new List<Soldier>();
        private IAttackStrategy _attackStrategy;

        public Platoon(List<Soldier> soldiers)
        {
            _soldiers = soldiers;
        }

        public bool HasSoldier => _soldiers.Count > 0;
        
        public void ShowPlatoon()
        {
            Console.WriteLine("Взвод:");
            foreach (var soldier in _soldiers)
            {
                Console.WriteLine($"{soldier.Name}. Здоровье:{soldier.Health}|Урон:{soldier.Damage}|Броня:{soldier.Armor}");
            }
        }

        public void RemoveDead()
        {
            for(int i = _soldiers.Count - 1; i >= 0; i--)
            {
                if (_soldiers[i].IsAlive == false)
                {
                    Console.WriteLine($"{_soldiers[i].Name} погиб!");
                    _soldiers.RemoveAt(i);
                }
            }
        }

        public void Attack(Platoon enemy)
        {
            foreach (var soldier in _soldiers)
            {
                soldier.Attack(enemy._soldiers);
            }
        }

        public List<Soldier> GetSoldiers()
        {
            return _soldiers;
        }
    }

    class Soldier
    {
        private IAttackStrategy _attackStrategy;
        public Soldier(string name, int health, int damage, int armor, IAttackStrategy attackStrategy)
        {
            Health = health;
            Damage = damage;
            Armor = armor;
            Name = name;
            _attackStrategy = attackStrategy;
        }

        public int Health { get; private set; }
        public int Damage { get; private set; }
        public int Armor { get; private set; }
        public string Name { get; private set; }
        public bool IsAlive => Health > 0;
        
        public void Attack(List<Soldier> enemies)
        {
            Console.WriteLine($"{Name}, атакует!");
            _attackStrategy.Attack(this, enemies);
        }

        public void TakeDamage(int damage)
        {
            int finalDamage = damage - Armor;

            if (finalDamage < 0)
            {
                finalDamage = 0;
            }

            Health-= finalDamage;

            Console.WriteLine($"{Name} получил {finalDamage} урона | осталось HP:{Health}");
        }

    }

    class SoldierFactory
    {
        private static Random _random = new Random();

        public List<Soldier> CreateSquad(int count)
        {
            List<Soldier> squad = new List<Soldier>();
            for (int i = 0; i < count; i++)
            {
                squad.Add(CreateRandomSoldier());
            }
            return squad;
        }

        public Soldier CreateRandomSoldier()
        {
            int type = _random.Next(4);

            switch (type)
            {
                case 0:
                    return new Soldier("Снайпер", 100, 50, 20, new PreciseAttack());
                case 1:
                    return new Soldier("Штурмовик", 200, 40, 30, new SingleAttack());
                case 2:
                    return new Soldier("Разведчик", 90, 50, 10, new StrongAttack());
                case 3:
                    return new Soldier("Пулеметчик", 250, 30, 30, new MultiAttack());
                default:
                    throw new Exception("Неизвестный тип бойца");
            }
        }
    }

    class SingleAttack : IAttackStrategy
    {
        private static Random _random = new Random();
        public void Attack(Soldier attacker, List<Soldier> enemies)
        {
            int index = _random.Next(enemies.Count);
            if (enemies.Count == 0)
            {
                return;
            }
            else
            {
                enemies[index].TakeDamage(attacker.Damage);
            }
        }
    }

    class MultiAttack : IAttackStrategy
    {
        private static Random _randomTarget = new Random();

        public void Attack(Soldier attacker, List<Soldier> enemies)
        {
            if (enemies.Count == 0)
            {
                return;
            }
            else
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[_randomTarget.Next(enemies.Count)].TakeDamage(attacker.Damage);
                }
            }
        }
    }

    class StrongAttack : IAttackStrategy
    {
        private Random _randomTarget = new Random();
        private int _strongAttackMultiplier = 2;
        public void Attack(Soldier attacker, List<Soldier> enemies)
        {
            int doubleDamage = attacker.Damage * _strongAttackMultiplier;
            if (enemies.Count == 0)
            {
                return;
            }

            int index = _randomTarget.Next(enemies.Count);
            enemies[index].TakeDamage(doubleDamage);

            Console.WriteLine($"Был нанесен сокрушительный удар:{doubleDamage} урона");
        }
    }

    class PreciseAttack : IAttackStrategy
    {
        private static Random _random = new Random();

        public void Attack(Soldier attacker, List<Soldier> enemies)
        {
            if (enemies.Count == 0)
            {
                return;
            }

            int index = _random.Next(enemies.Count);

            var target = enemies[new Random().Next(enemies.Count)];

            target.TakeDamage(target.Health);
            Console.WriteLine($"Точный выстрел: {target.Health} урона по {target.Name}");
        }
    }
}
