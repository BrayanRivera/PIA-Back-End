using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiPIA.Entidades;

namespace WebApiPIA
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Rifa> Rifas { get; set; }
        public DbSet<Boleto> Boletos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Premio> Premios { get; set; }
    }
}
