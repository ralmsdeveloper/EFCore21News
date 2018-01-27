using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace EFFunctions
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new ExemploContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var frutas = db
                    .Frutas
                    .Where(p => EF.Functions.DateDiffDay(DateTime.Now,p.Validade) == 0)
                    .ToList();

                Console.WriteLine("Nome:");

                foreach (var fruta in frutas)
                {
                    Console.WriteLine($"\t{fruta.Id}-{fruta.Nome}");
                }
            }

            Console.ReadKey();
        }
    }

    class ExemploContext : DbContext
    {
        public DbSet<Fruta> Frutas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\Sql2016;Integrated Security=true;Initial Catalog=EFFunctions21");
            optionsBuilder.UseLoggerFactory(new LoggerFactory().AddConsole());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fruta>().SeedData
            (
                new Fruta { Id = 1, Nome = "Uva", Validade = DateTime.Now },
                new Fruta { Id = 2, Nome = "Maçã", Validade = DateTime.Now },
                new Fruta { Id = 3, Nome = "Pera", Validade = new DateTime(2018, 01, 27) },
                new Fruta { Id = 4, Nome = "Goiaba", Validade = new DateTime(2018, 01, 28) }
            );
        }
    }

    class Fruta
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime Validade { get; set; }
    }
}
