using System;
using System.Collections.Generic;

namespace Supermarket
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MarketManager market = new MarketManager();
            market.WorkMarket();
        }
    }

    class MarketManager
    {
        private const string ShowProductsCommand = "1";
        private const string OpenCheckOutCommand = "2";
        private const string ServeClientCommand = "3";
        private const string ExitCommand = "4";

        Supermarket supermarket = new Supermarket(0);


        public void WorkMarket()
        {
            bool isOpen = true;

            Console.WriteLine("Добро пожаловать в магазин.");
            Console.WriteLine($"{ShowProductsCommand}-показать товар");
            Console.WriteLine($"{OpenCheckOutCommand}-открыть кассу");
            Console.WriteLine($"{ServeClientCommand}-обслужить клиента по очереди");
            Console.WriteLine($"{ExitCommand}- выход");

            while (isOpen)
            {


                string input = Console.ReadLine();

                switch (input)
                {
                    case ShowProductsCommand:
                        supermarket.ShowAllProducts();
                        break;
                    case OpenCheckOutCommand:
                        supermarket.GenerateCustomer();
                        break;
                    case ServeClientCommand:
                        supermarket.ServeCustomer();
                        break;
                    case ExitCommand:
                        isOpen = false;
                        break;

                }

            }
            Console.Clear();
        }
    }


    class Person
    {
        private List<Product> _cartProducts;
        private List<Product> _bagProducts;
        public Person(int money)
        {
            _cartProducts = new List<Product>();
            _bagProducts = new List<Product>();
            Money = money;
        }
        public int Money { get; private set; }

        public void ShowCart()
        {
            Console.WriteLine("Товары в корзине:");

            foreach (var product in _cartProducts)
            {
                Console.WriteLine($"{product.Name} - {product.Price}");
            }
            Console.WriteLine($"Итого:{GetCartTotal()}");
        }
        public void AddToCart(Product product)
        {
            _cartProducts.Add(product);
        }

        public int GetCartTotal()
        {
            int sum = 0;

            foreach (var product in _cartProducts)
            {
                sum += product.Price;
            }

            return sum;
        }

        public Product RemoveRandomProduct(Random random)
        {
            if (_cartProducts.Count == 0)
            {
                return null;
            }

            int index = random.Next(_cartProducts.Count);
            Product removed = _cartProducts[index];
            _cartProducts.RemoveAt(index);
            return removed;
        }

        public void CompletePurchase()
        {
            int total = GetCartTotal();

            foreach(var product in _cartProducts)
            {
                _bagProducts.Add(product);
            }

            Money-=total;
            _cartProducts.Clear();
            return total;
        }
    }

    class Product
    {
        public Product(int price, string name)
        {
            Name = name;
            Price = price;
        }
        public string Name { get; private set; }
        public int Price { get; private set; }
    }

    class Supermarket
    {
        Queue<Person> _customers = new Queue<Person>();
        Random _random = new Random();
        List<Product> _products;

        public Supermarket(int money)
        {
            Money = money;
            _products = new List<Product>();

            _products.Add(new Product(1000, "Виски"));
            _products.Add(new Product(300, "Фарш"));
            _products.Add(new Product(200, "Конфеты"));
            _products.Add(new Product(800, "Стиральный порошок"));
            _products.Add(new Product(500, "Сыр"));
            _products.Add(new Product(150, "Пиво"));
            _products.Add(new Product(70, "Батон"));
            _products.Add(new Product(50, "Хлеб"));
            _products.Add(new Product(100, "Стакан кофе"));
            _products.Add(new Product(250, "Зубная паста"));
            _products.Add(new Product(150, "Шоколад"));
            _products.Add(new Product(250, "Чипсы"));
            _products.Add(new Product(150, "Мороженное"));
            _products.Add(new Product(250, "Сигареты"));
            _products.Add(new Product(400, "Огурцы"));
            _products.Add(new Product(399, "Помидоры"));
            _products.Add(new Product(200, "Бананы"));
            _products.Add(new Product(700, "Рыба"));
            _products.Add(new Product(300, "Чай"));
            _products.Add(new Product(700, "Банка Кофе"));

        }
        public int Money { get; private set; }

        public void ServeCustomer()
        {
            if (_customers.Count == 0)
            {
                Console.WriteLine("Очередь пуста");
                return;
            }

            Person customer = _customers.Dequeue();

            Console.WriteLine("\n--- Новый клиент ---");

            customer.ShowCart();

            int cartTotal = customer.GetCartTotal();

            while (cartTotal > customer.Money)
            {
                Console.WriteLine($"Не хватает денег! Нужно {cartTotal}, есть {customer.Money}");
                Product removed = customer.RemoveRandomProduct(_random);
                Console.WriteLine($"Клиент выбросил: {removed.Name}");
               
            }

            int payment = customer.GetCartTotal();
            customer.CompletePurchase();
            Money += payment;

            Console.WriteLine($"Покупка успешна!");
            Console.WriteLine($"Клиент заплатил: {payment}");
            Console.WriteLine($"Магазин заработал: {Money}");
            Console.WriteLine("----------------------");
        }

        public void FillCustomer(Person person)
        {
            int moneyLeft = person.Money;

            while (true)
            {
                Product product = GetRandomProduct();

                if (moneyLeft - product.Price < 0)
                    break;

                person.AddToCart(product);
                moneyLeft -= product.Price;
            }
        }

        public void GenerateCustomer()
        {
            int minMoney = 150;
            int maxMoney = 3000;
            int customersCoumt = _random.Next(7, 15);

            for (int i = 0; i < customersCoumt; i++)
            {
                int money = _random.Next(minMoney, maxMoney);
                Person person = new Person(money);

                FillCustomer(person);
                _customers.Enqueue(person);
            }
            Console.WriteLine($"В очереди {_customers.Count} человек");
        }
        private Product GetRandomProduct()
        {
            int index = _random.Next(_products.Count);
            return _products[index];
        }

        public void ShowAllProducts()
        {
            foreach (var product in _products)
            {
                Console.WriteLine($"{product.Name} - стоимость {product.Price}");
            }
        }
    }
}
