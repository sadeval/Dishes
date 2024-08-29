using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DishMenu
{
    public class Dish
    {
        public int DishId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }

    public class AppDbContext : DbContext
    {
        public DbSet<Dish> Dishes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder();
                builder.SetBasePath(Directory.GetCurrentDirectory());
                builder.AddJsonFile("appsettings.json");

            var config = builder.Build();
            string? connectionString = config.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    class Program
    {
        static void Main()
        {
            
            using (var db = new AppDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }

            // Проверка доступности базы данных
            using (var context = new AppDbContext())
            {
                if (context.Database.CanConnect())
                {
                    Console.WriteLine("База данных доступна.");
                }
                else
                {
                    Console.WriteLine("Не удалось подключиться к базе данных.");
                    return;
                }
            }

            // Добавление одного объекта
            using (var context = new AppDbContext())
            {
                var dish = new Dish
                {
                    Name = "Суп томатный",
                    Description = "Гаспаччо",
                    Price = 85.00m
                };

                context.Dishes.Add(dish);
                context.SaveChanges();
                Console.WriteLine("Блюдо добавлено.");
            }

            // Добавление коллекции объектов
            using (var context = new AppDbContext())
            {
                var dishes = new List<Dish>
                {
                    new Dish { Name = "Суп грибной", Description = "Грибной суп с лисичками", Price = 56.99m },
                    new Dish { Name = "Борщ", Description = "Традиционный украинский борщ", Price = 64.55m }
                };

                context.Dishes.AddRange(dishes);
                context.SaveChanges();
                Console.WriteLine("Коллекция блюд добавлена.");
            }

            // Получение всех блюд, в названии которых содержится слово "Суп"
            using (var context = new AppDbContext())
            {
                var soupDishes = context.Dishes
                    .Where(d => d.Name.Contains("Суп"))
                    .ToList();

                Console.WriteLine("Блюда с названием, содержащим 'Суп':");
                foreach (var dish in soupDishes)
                {
                    Console.WriteLine($"{dish.DishId}: {dish.Name} - {dish.Price} грн.");
                }
            }

            // Получение блюда по Id
            using (var context = new AppDbContext())
            {
                int id = 1;
                var dish = context.Dishes.Find(id);

                if (dish != null)
                {
                    Console.WriteLine($"Блюдо с Id = {id}: {dish.Name} - {dish.Price} грн.");
                }
                else
                {
                    Console.WriteLine($"Блюдо с Id = {id} не найдено.");
                }
            }

            // Получение самого последнего блюда из таблицы
            using (var context = new AppDbContext())
            {
                var lastDish = context.Dishes
                    .OrderByDescending(d => d.DishId)
                    .FirstOrDefault();

                if (lastDish != null)
                {
                    Console.WriteLine($"Последнее добавленное блюдо: {lastDish.Name} - {lastDish.Price} грн.");
                }
                else
                {
                    Console.WriteLine("Таблица блюд пуста.");
                }
            }
        }
    }
}
