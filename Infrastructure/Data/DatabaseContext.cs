using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class DatabaseContext : IdentityDbContext<AppUser>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }

        public DbSet<Domain.Entities.Kurye> Kuryeler { get; set; }

        public DbSet<Domain.Entities.Kuryeci> Kuryeciler { get; set; }

        public DbSet<Domain.Entities.Kullanici> Kullanicilar { get; set; }

        public DbSet<Domain.Entities.Sikayet> Sikayetler { get; set; }
    }
}
