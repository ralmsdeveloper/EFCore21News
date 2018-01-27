using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace EFFunctions20
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new Exemplo())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var clientes = db
                    .Clientes
                    .Where(p => EFCore.DateDiff(DatePart.day, p.DataCadastro, DateTime.Now) == 0)
                    .ToList();
            }

            Console.ReadKey();
        }
    }

    class Exemplo : DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\Sql2016;Integrated Security=true;Initial Catalog=OrdemColumas01");

            optionsBuilder.UseLoggerFactory(new LoggerFactory().AddConsole());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.CreateFunctionDateDiff();

            base.OnModelCreating(modelBuilder);
        }
    }

    class Cliente
    {
        public int Id { get; set; }
        public string RazaoSocial { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }
    }
}
