using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Converters;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ConversoesDeTipos
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new ExemploContext())
            {
                var cores = db.Cores.ToList();
            }

            Console.ReadKey();
        }
    }

    class ExemploContext : DbContext
    {
        public DbSet<Cor> Cores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\Sql2016;Integrated Security=true;Initial Catalog=ConversaoTipos");
            //Adicionar Log
            optionsBuilder.UseLoggerFactory(new LoggerFactory().AddConsole());
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cor>(
                p =>
                {
                    p.Property(n => n.Nome).HasConversion<string>();
                    p.Property(n => n.Numero).HasConversion<string>();

                    p.Property(n => n.Controle)
                        .HasConversion(
                            new ValueConverter<string,int>
                                (
                                    v => int.Parse(v),
                                    v => v.ToString()
                                )
                        );
                });
        }
    }

    enum Cores
    {
        Branco,
        Azul,
        Vermelho,
        Preto,
        Amarelo
    }

    class Cor
    {
        public int Id { get; set; }
        public Cores Nome { get; set; }
        public int? Numero { get; set; }
        public string Controle { get; set; }
    }
}
