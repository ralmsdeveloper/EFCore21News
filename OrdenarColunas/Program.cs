using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace OrdenarColunas
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new Exemplo())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                db.Add(new Cliente
                {
                    RazaoSocial = "RAFAEL"
                });
                db.SaveChanges();

                var xxx = db
                    .Clientes
                    .FromSql("SELECT P.Id,P.RazaoSocial,P.Endereco,P.Cidade," +
                             "P.Ativo FROM CLIENTES P WITH(INDEX (IDX_RazaoSocial)) WHERE P.RazaoSocial=@p0",
                             /*Criterio busca*/"RAFAEL")
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
            optionsBuilder.UseSqlServer(@"Server=.\Sql2016;Integrated Security=true;Initial Catalog=OrdemColunas21");

            optionsBuilder.UseLoggerFactory(new LoggerFactory().AddConsole());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Cliente>(p =>
                {
                    p.HasIndex(c => c.RazaoSocial)
                        .HasName("IDX_RazaoSocial");
                });
        }
    }

    class Cliente
    {
        public int Id { get; set; }
        public string RazaoSocial { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public bool Ativo { get; set; }
    }
}
