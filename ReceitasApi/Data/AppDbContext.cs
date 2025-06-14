using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ReceitasApi.Model;

namespace ReceitasApi.Data;

public class AppDbContext : DbContext
{
    public DbSet<Receita> Receitas { get; set; }

    public DbSet<Alergias> Alergias { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseSqlite("Data Source=receitas.db;Cache=Shared");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var ingredientesConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
        );

        modelBuilder.Entity<Receita>()
            .Property(r => r.Ingredientes)
            .HasConversion(ingredientesConverter);

        base.OnModelCreating(modelBuilder);
    }

}