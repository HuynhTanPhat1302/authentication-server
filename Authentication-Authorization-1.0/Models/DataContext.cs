using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Authentication_Authorization_1._0.Models
{
    public class DataContext : DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Role>().ToTable("Role");

            modelBuilder.Entity<Role>().HasKey(r => r.RoleID);
            modelBuilder.Entity<User>().HasKey(u => u.UserID);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleID);
        }
    }
}
