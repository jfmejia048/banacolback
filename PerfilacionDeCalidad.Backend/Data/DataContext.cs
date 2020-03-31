using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PerfilacionDeCalidad.Backend.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Buques> Buques { get; set; }

        public DbSet<Destinos> Destinos { get; set; }

        public DbSet<Exportadores> Exportadores { get; set; }

        public DbSet<Fincas> Fincas { get; set; }

        public DbSet<Frutas> Frutas { get; set; }

        public DbSet<Palets> Palets { get; set; }

        public DbSet<Pomas> Pomas { get; set; }

        public DbSet<Puertos> Puertos { get; set; }

        public DbSet<Tracking> Tracking { get; set; }

        public DbSet<TypeDocument> TypeDocuments { get; set; }

        public DbSet<TypeUser> TypeUsers { get; set; }

        public DbSet<TypeVias> TypeVias { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasIndex(x => x.Document).IsUnique();
            base.OnModelCreating(builder);
        }
    }
}
