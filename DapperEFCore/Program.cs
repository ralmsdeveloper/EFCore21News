using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DapperEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new Exemplo())
            {
                db.Database.EnsureDeleted();

                if (db.Database.EnsureCreated())
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        db.Clientes.Add(new Cliente
                        {
                            RazaoSocial = $"CLIENTE TESTE {i}",
                            Cidade = "ITABAIANA",
                            Endereco = "RUA TESTE",
                            Ativo = i % 2 == 0
                        });
                    }
                    db.SaveChanges();
                }

                var query = EF.CompileQuery((Exemplo ctx, int id)
                                => ctx.Clientes.Single(p => p.Id == id));

                var testes = new StringBuilder();
                for (int y = 1; y <= 10; y++)
                {
                    var tempo = new Stopwatch();
                    tempo.Restart();
                    for (int i = 1; i <= 1000; i++)
                    {
                        var cliente = query(db, i);
                    }
                    tempo.Stop();
                    testes.AppendLine($"Teste {y} EFCore Consultar 1000 registros: {tempo.Elapsed}  **");

                    tempo.Restart();
                    var listaClienteEF = db.Clientes.ToList();
                    tempo.Stop();
                    testes.AppendLine($"Teste {y} EFCore Listar 1000 registros: {tempo.Elapsed} ");

                    tempo.Restart();
                    for (int i = 1; i <= 1000; i++)
                    {
                        var cliente = CarregarCliente(i);
                    }
                    tempo.Stop();
                    testes.AppendLine($"Teste {y} Dapper Consultar 1000 registros: {tempo.Elapsed} ");

                    tempo.Restart();
                    var listaClienteDapper = GetAll();
                    tempo.Stop();
                    testes.AppendLine($"Teste {y} Dapper Listar 1000 registros: {tempo.Elapsed} ");
                }

                System.Threading.Thread.Sleep(2000);
                Console.Clear();
                Console.WriteLine(testes.ToString());
            }

            Console.ReadKey();
        }

        private static List<Cliente> GetAll()
        {
            var query = @"SELECT P.Id,P.RazaoSocial,P.Endereco,P.Cidade,
                          P.Ativo FROM CLIENTES P";

            using (var sqlConnection = new SqlConnection(Exemplo.StringConexao))
            {
                return sqlConnection.Query<Cliente>(query).ToList();
            }
        }

        private static Cliente CarregarCliente(int codigo)
        {
            var query = @"SELECT P.Id,P.RazaoSocial,P.Endereco,P.Cidade,
                          P.Ativo FROM CLIENTES P Where P.Id = @Id; ";

            using (var sqlConnection = new SqlConnection(Exemplo.StringConexao))
            {
                return sqlConnection.Query<Cliente>(query, new { Id = codigo }).First();
            }
        }

    }

    class Exemplo : DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }
        public const string StringConexao = @"Server=.\Sql2016;Integrated Security=true;Initial Catalog=DapperEFCore";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(StringConexao);

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
