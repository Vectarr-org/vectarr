using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.Context
{
    public class VectarrPostgresDBContext : IdentityDbContext<UserAccount, IdentityRole<int>, int>
    {
        public VectarrPostgresDBContext(DbContextOptions<VectarrPostgresDBContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = true;

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
