
using ApiEcommerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Client relationships
        modelBuilder.Entity<Client>()
            .HasMany(c => c.Phones)
            .WithOne(p => p.Client)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Client>()
            .HasMany(c => c.Addresses)
            .WithOne(a => a.Client)
            .HasForeignKey(a => a.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Only one default phone/address per client
        modelBuilder.Entity<Phone>()
            .HasIndex(p => new { p.ClientId, p.IsDefault })
            .IsUnique()
            .HasFilter("[IsDefault] = 1");
        modelBuilder.Entity<Address>()
            .HasIndex(a => new { a.ClientId, a.IsDefault })
            .IsUnique()
            .HasFilter("[IsDefault] = 1");
    }
    


    public DbSet<Category> Categories { get; set; }

    

    public DbSet<Product> Products { get; set; }

    // sustituido por Identity
    //public DbSet<User> Users { get; set; }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    
    public DbSet<Store> Stores { get; set; }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Phone> Phones { get; set; }
    public DbSet<Address> Addresses { get; set; }
}