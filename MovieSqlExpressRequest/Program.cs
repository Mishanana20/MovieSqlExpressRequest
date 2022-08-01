using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MovieSqlExpress;

internal class Program
{
    static void Main(string[] args)
    {
        using (movie_dbContext db = new movie_dbContext())
        {
            bool isAvalaible = db.Database.CanConnect();
            if (!isAvalaible)
            {
                Console.WriteLine("база данных недоступна");
                return;
            }
            else Console.WriteLine("база данных доступна");
        }
        using (movie_dbContext db = new movie_dbContext())
        {
            while (true)
            {
                /*
                 * как сделать?
                 * сначала делаю выборку по названию фильма
                 * получается объект с актерами(много), названием фильма (1) и жанрами (много)
                 * ---------
                 * Если есть хоть один актер/жанр из множеств актеры и жанры, то добавялем в финальную выборку.
                 * или наоборот, если нет хотя бы одного, то удаляем, а если есть, то брейк и дальше проверяем
                 */
                Console.Write("Введите название фильма: ");
                string? filmName = Console.ReadLine();
                var something = (db.Movies.AsNoTracking()
                .Include(a => a.Genres)
                .Include(a => a.Actors).Where(a => EF.Functions.Like(a.Name, $"%{filmName}%"))).ToList();
                if (something.Any())  //if (something != null)
                {
                    Console.WriteLine("Выборка что-то имеет");
                    foreach (var q in something)
                    {
                        Console.WriteLine($"*** {q.Name}");
                        foreach (var a in q.Genres)
                        {
                            Console.WriteLine($"{a.Name}");
                        }
                        foreach (var a in q.Actors)
                        {
                            Console.WriteLine($"{a.FirstName} {a.SecondName}");
                        }
                    }
                }
                else Console.WriteLine("выборка пуста");

                Console.Write("Введите Имя актера: ");
                string? actorName = Console.ReadLine();
                var somethingActors = db.Actors.AsNoTracking()
                                      .Include(a => a.Movies)
                                      .Where(a => EF.Functions.Like(a.FirstName, $"%{actorName}%"));
                if (somethingActors.Any())  //if (something != null)
                {
                    Console.WriteLine("Выборка имеет актеров");
                    foreach (var q in somethingActors)
                    {
                        Console.WriteLine($"*** {q.FirstName} {q.SecondName}");
                        foreach (var a in q.Movies)
                        {
                            Console.WriteLine($"{a.Name}");
                        }

                    }
                }
                else Console.WriteLine("выборка пуста");

                Console.Write("Введите жанр фильма: ");
                string? genreName = Console.ReadLine();
                var somethingGenre = db.Genres.AsNoTracking()
                                      .Include(a => a.Movies)
                                      .Where(a => EF.Functions.Like(a.Name, $"%{genreName}%"));
                if (somethingActors.Any())  //if (something != null)
                {
                    Console.WriteLine("Выборка имеет эти жанры");
                    foreach (var q in somethingGenre)
                    {
                        Console.WriteLine($"*** {q.Name}");
                        foreach (var a in q.Movies)
                        {
                            Console.WriteLine($"{a.Name}");
                        }

                    }
                }
                else Console.WriteLine("выорка пуста");

                List<Movie> movies = new List<Movie>();
                foreach (var movie in something)
                {
                    bool isHasActor = false, isHasGenre = false;
                    foreach (var actor in movie.Actors)
                    {
                        if (actor.FirstName.ToLower().Contains(actorName.ToLower()))
                        {
                            isHasActor = true;
                        }
                    }
                    foreach (var genre in movie.Genres)
                    {
                        if (genre.Name.ToLower().Contains(genreName.ToLower()))
                        {
                            isHasGenre = true;
                        }
                    }
                    if (isHasGenre == true && isHasActor == true)
                    {
                        movies.Add(movie);
                    }
                }
                //если фильмы имеют Any(от фильмов актеров), то добавить
                movies.Distinct();
                if (movies.Any())  //if (something != null)
                {
                    Console.WriteLine(@"/\/\/\/\/\/\/\/\/\/\/\/\/\/\");

                    Console.WriteLine("Результаты поиска:");
                    foreach (var m in movies)
                    {
                        Console.WriteLine($"*** {m.Name}");
                        Console.WriteLine("Актеры:");
                        foreach (var a in m.Actors)
                        {
                            Console.WriteLine($"{a.FirstName} {a.SecondName}");
                        }
                        Console.WriteLine("Жанры:");
                        foreach (var a in m.Genres)
                        {
                            Console.WriteLine($"{a.Name}");
                        }
                    }
                }
                else { Console.WriteLine("По данному запросу ничего не найдено"); }
            }
        }
    }
}