using Contacts.Domain.Entities;
using Contacts.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Infrastructure.Persistence;

/// <summary>
/// Contexto de persistência principal da aplicação.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Inicializa o contexto com as opções configuradas para o provider atual.
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>
    /// Coleção de contatos persistidos no banco.
    /// </summary>
    public DbSet<Contact> Contacts => Set<Contact>();

    /// <summary>
    /// Aplica os mapeamentos de entidade do modelo.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ContactConfiguration());
    }
}
