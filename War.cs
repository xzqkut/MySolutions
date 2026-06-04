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
        void Attack( int damage,List<Soldier> enemies);
    }

    class War
    {
        private Platoon _platoon1;
        private Platoon _platoon2;

        public War()
        {
        }

        public void StartBattle()
        {
            SoldierFactory factory = new SoldierFactory();

            Console.WriteLine("Введите кол-во бойцов для 1 отряда");
            int countFirstPlatoon = ReadInt("Введите кол-во бойцов для 1 отряда");
            Console.WriteLine("Введите кол-во бойцов для 2 отряда");
            int countSecondPlatoon = Convert.ToInt32(Console.ReadLine());

            _platoon1 = new Platoon(factory.CreateSquad(countFirstPlatoon));
            _platoon2 = new Platoon(factory.CreateSquad(countSecondPlatoon));

            Fight();
        }

        private void ShowBattleResult()
        {
            if (_platoon1.HasSoldier&& !_platoon2.HasSoldier)
            {
                Console.WriteLine("Победил 1 отряд");
            }
            else if (!_platoon1.HasSoldier&&_platoon2.HasSoldier)
            {
                Console.WriteLine("победил 2 отряд");

            }
            else
            {
                Console.WriteLine("Ничья");
            }
        }
        private void Fight()
        {
            int round = 1;
            while (_platoon1.HasSoldier && _platoon2.HasSoldier)
            {
                Console.WriteLine($"---Раунд{round}---");
                _platoon1.Attack(_platoon2.GetSoldiers());
                _platoon2.Attack(_platoon1.GetSoldiers());

                _platoon1.RemoveDead();
                _platoon2.RemoveDead();

                round++;
                Console.ReadKey();
            }

            ShowBattleResult();
        }

        private int ReadInt(string message)
        {
            int count;
            bool isValid = true;
            do
            {
                Console.WriteLine(message);
                string input = Console.ReadLine();
                isValid = int.TryParse(input, out count);
                if (!isValid)
                {
                    Console.WriteLine("Ошибка ввода. Введите число");
                }
            }
            while (!isValid);
            return count;
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

        public void ShowInfo()
        {
            Console.WriteLine("Взвод:");
            foreach (var soldier in _soldiers)
            {
                Console.WriteLine($"{soldier.Name}. Здоровье:{soldier.Health}|Урон:{soldier.Damage}|Броня:{soldier.Armor}");
            }
        }

        public void RemoveDead()
        {
            for (int i = _soldiers.Count - 1; i >= 0; i--)
            {
                if (_soldiers[i].IsAlive == false)
                {
                    Console.WriteLine($"{_soldiers[i].Name} погиб!");
                    _soldiers.RemoveAt(i);
                }
            }
        }

        public void Attack(List<Soldier>enemies)
        {
            foreach (var soldier in _soldiers)
            {
                soldier.Attack(enemies);
            }
        }

        public List<Soldier> GetSoldiers()
        {
            return new List<Soldier>( _soldiers );
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
            _attackStrategy.Attack(Damage, enemies);
        }

        public void TakeDamage(int damage)
        {
            int finalDamage = damage - Armor;

            if (finalDamage < 0)
            {
                finalDamage = 0;
            }

            Health -= finalDamage;

            Console.WriteLine($"{Name} получил {finalDamage} урона | осталось HP:{Health}");
        }

        public Soldier Clone()
        {
            return new Soldier(Name, Health, Damage, Armor, _attackStrategy);
        }

    }

    class SoldierFactory
    {
        private List<Soldier> _templates;

        public SoldierFactory()
        {
            _templates = new List<Soldier>()
            {
            new Soldier("Снайпер", 100, 50, 20, new PreciseAttack()),
            new Soldier("Штурмовик", 200, 40, 30, new SingleAttack()),
            new Soldier("Разведчик", 90, 50, 10, new StrongAttack()),
            new Soldier("Пулеметчик", 250, 30, 30, new MultiAttack())
            };

        }
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
            int index = UserUtils.GetRandomNumber(0,_templates.Count);
            return _templates[index].Clone();
           
        }
    }

    class SingleAttack : IAttackStrategy
    {
        
        public void Attack(int damage,List<Soldier> enemies)
        {
            
            if (enemies.Count == 0)
            {
                return;
            }
            int index = UserUtils.GetRandomNumber(0, enemies.Count);
            enemies[index].TakeDamage(damage);
            
        }
    }

    class MultiAttack : IAttackStrategy
    {
        public void Attack(int damage, List<Soldier> enemies)
        {
            if (enemies.Count == 0)
            {
                return;
            }
            else
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    int index = UserUtils.GetRandomNumber(0, enemies.Count);
                    enemies[index].TakeDamage(damage);
                }
            }
        }
    }

    class StrongAttack : IAttackStrategy
    {
        private int _strongAttackMultiplier = 2;
        public void Attack(int damage, List<Soldier> enemies)
        {
            int doubleDamage = damage * _strongAttackMultiplier;
            if (enemies.Count == 0)
            {
                return;
            }

            int index = UserUtils.GetRandomNumber(0,enemies.Count);
            enemies[index].TakeDamage(doubleDamage);

            Console.WriteLine($"Был нанесен сокрушительный удар:{doubleDamage} урона");
        }
    }

    class PreciseAttack : IAttackStrategy
    {
        public void Attack(int damage, List<Soldier> enemies)
        {
            if (enemies.Count == 0)
            {
                return;
            }

            int index = UserUtils.GetRandomNumber(0, enemies.Count);

            var target = enemies[index];

            int damageDealt = target.Health;
            target.TakeDamage(target.Health);
            Console.WriteLine($"Точный выстрел: {damageDealt} урона по {target.Name}");
        }
    }

    class UserUtils
    {
        private static Random s_random=new Random();

        public static int GetRandomNumber(int min,int max)
        {
            return s_random.Next(min,max);
        }
    }
}
