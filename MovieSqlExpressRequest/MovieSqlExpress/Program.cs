using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MovieSqlExpress
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (movie_dbContext db = new movie_dbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
            using (movie_dbContext db = new movie_dbContext())
            {
                bool isAvalaible = db.Database.CanConnect();
                if (!isAvalaible)
                {
                    Console.WriteLine("база данных недоступна");
                }
                else Console.WriteLine("база данных доступна");

                Actor misha = new Actor { FirstName = "Миша", SecondName = "Носик" };
                db.Add(misha);
                Actor polina = new Actor { FirstName = "Полина", SecondName = "Полухина" };
                db.AddRange(misha, polina);
                db.SaveChanges();
                var actors = db.Actors.ToList();

                Console.WriteLine("Список актеров:");
                foreach (Actor a in actors)
                {
                    Console.WriteLine($" - {a.Id}: {a.FirstName} {a.SecondName}");


                }
            
                Genre thriller = new Genre { Name = "Thriller" };
                Genre psychological_thriller = new Genre {Name = "Psychological thriller" };
                Genre fantasy= new Genre { Name = "Fantasy" };
                Genre mecha = new Genre { Name = "Mecha" };
                db.AddRange(fantasy, thriller, psychological_thriller);
               
                Movie EVA = new Movie { Name = "Evangelion", Description = "New Genesis Evangelion", Date = DateTime.Now.Date };
                Movie FGO = new Movie { Name = "Fate: Grand Order", Description = "Альтернативная история из линейки Фейт", Date = DateTime.Now.Date };
                Movie FZ = new Movie { Name = "Fate: Zero", Description = "Приквел истории фейт. Рассказывается сюжет 7 войн за грааль", Date = DateTime.Now.Date };
                db.AddRange(FZ,EVA,FGO);
                db.SaveChanges();

                EVA.Genres.Add(mecha);
                EVA.Genres.Add(psychological_thriller);
                EVA.Actors.Add(misha);
                db.SaveChanges();

                var genres = db.Genres.ToList();
                Console.WriteLine("Список жанров:");
                foreach (Genre a in genres)
                {
                    Console.WriteLine($" - {a.Id}: {a.Name}");
                }


                var movies = db.Movies.ToList();
                Console.WriteLine("Список фильмов:");
                foreach (Movie a in movies)
                {
                    Console.WriteLine($" - {a.Id}: {a.Name}, {a.Description}, {a.Date} ");
                }
                Console.WriteLine("-------------------\n");
            }
            using (movie_dbContext db = new movie_dbContext())
            {
                var movies = db.Movies.Include(c => c.Actors).Include(c=> c.Genres).ToList();
                // выводим все курсы
                foreach (var c in movies)
                {
                    Console.WriteLine($"Название фильма: {c.Name}");
                    // выводим всех студентов для данного кура
                    foreach (Actor s in c.Actors)
                        Console.WriteLine($"Имя актеров: {s.FirstName} {s.SecondName} ");
                    foreach (Genre s in c.Genres)
                        Console.WriteLine($"Жанры: {s.Name} ");
                    Console.WriteLine("-------------------");
                }
            }
        }
    }
}