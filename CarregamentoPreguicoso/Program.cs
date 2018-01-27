using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarregamentoPreguicoso
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new ExemploContext())
            {
                db.Database.EnsureDeleted();
                if (db.Database.EnsureCreated())
                {
                    for (int i = 0; i < 5; i++)
                    {
                        db.Set<Desenvolvedor>().Add(new Desenvolvedor
                        {
                            Nome = $"Desenvolvedor {i}",
                            Enderecos = new List<EnderecoVirtual>
                             {
                                 new EnderecoVirtual
                                 {
                                      Url = "https://github.com/ralmsdeveloper"
                                 }
                             }
                        });
                    }

                    db.SaveChanges();
                }

                var desenvolvedores = db.Set<Desenvolvedor>().ToList();
            }
        }
    }

    public class ExemploContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();

            optionsBuilder.UseSqlServer(@"Server=.\Sql2016;Integrated Security=true;Initial Catalog=LazyLoading");
            optionsBuilder.UseLoggerFactory(new LoggerFactory().AddConsole());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<EnderecoVirtual>()
                .HasOne(d => d.Desenvolvedor)
                .WithMany(e => e.Enderecos);
        }
    }

    public class Desenvolvedor
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public virtual IEnumerable<EnderecoVirtual> Enderecos { get; set; }
    }

    public class EnderecoVirtual
    {
        public int Id { get; set; }
        public string Url { get; set; }

        public virtual Desenvolvedor Desenvolvedor { get; set; }
    }
}
