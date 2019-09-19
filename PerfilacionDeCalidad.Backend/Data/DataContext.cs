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

        public DbSet<TypeUser> TypeUsers { get; set; }

        public DbSet<TypeVias> TypeVias { get; set; }

        public DbSet<TypeDocument> TypeDocuments { get; set; }
    }
}
