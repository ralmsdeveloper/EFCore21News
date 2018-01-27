using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ProvedorOracle
{
    /*
    Processo de criação do banco de dados ORACLE

    R:\app\Ralms\virtual\product\12.2.0\dbhome_1\bin

    sqlplus / as sysdba

    CREATE PLUGGABLE DATABASE ef_webinar
        ADMIN USER ef_webinar IDENTIFIED BY ef_webinar
        ROLES = (DBA)
        FILE_NAME_CONVERT = ('\pdbseed\', '\pdb_ef_webinar01\');

    ALTER PLUGGABLE DATABASE ef_webinar OPEN;
    */

    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new ExemploContext())
            {
                var migracao = db.GetService<IMigrator>().GenerateScript();

                if (db.Database.GetPendingMigrations().Any())
                {
                    db.Database.Migrate();
                }

                for (int i = 0; i < 5; i++)
                {
                    db.Pessoas.Add(new Pessoa
                    {
                        DataNascimento = DateTime.Now,
                        RazaoSocial = "CADASTRO PESSOA TESTE",
                        Endereco = "RUA TESTE",
                        Numero = i
                    });
                }

                db.SaveChanges();

                var pessoas = db.Pessoas.ToList();
                foreach (var pessoa in pessoas)
                {
                    Console.WriteLine($"\t{pessoa.Id}-{pessoa.RazaoSocial}");
                }
            }

            Console.ReadKey();
        }
    }

    class ExemploContext : DbContext
    {
        public DbSet<Pessoa> Pessoas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseOracle(@"USER ID=ef_webinarglaucia;PASSWORD=ef_webinarglaucia;DATA SOURCE=127.0.0.1:1521/ef_webinarglaucia");

            optionsBuilder.UseLoggerFactory(new LoggerFactory().AddConsole());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    class Pessoa
    {
        public int Id { get; set; }
        public string RazaoSocial { get; set; }
        public DateTime DataNascimento { get; set; }
        public bool Ativo { get; set; }
        public string Endereco { get; set; }
        public int Numero { get; set; }
    }
}
