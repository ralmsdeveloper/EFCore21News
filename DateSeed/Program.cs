using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace DateSeed
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new Exemplo())
            {

                var scriptMigracao = db.GetService<IMigrator>().GenerateScript();
                var scriptBanco = db.Database.GenerateCreateScript();

                if(db.Database.GetPendingMigrations().Any())
                {
                    db.Database.Migrate();
                }
            }
            Console.ReadKey();
        }
    }

    class Exemplo : DbContext
    {
        public DbSet<Status> Status { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\Sql2016;Integrated Security=true;Initial Catalog=DataSeed");
            optionsBuilder.UseLoggerFactory(new LoggerFactory().AddConsole());
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Status>()
                .SeedData(new[]
                {
                    new Status {Id = Guid.NewGuid(), Descricao = "Ativo" },
                    new Status {Id = Guid.NewGuid(), Descricao = "Desativado" },
                    new Status {Id = Guid.NewGuid(), Descricao = "SPC" },
                    new Status {Id = Guid.NewGuid(), Descricao = "Outros" }
                });
        }
    }

    class Status
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; }
    }
}
