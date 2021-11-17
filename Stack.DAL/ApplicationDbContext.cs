
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Stack.Entities.Models;


namespace Stack.DAL
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>  
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        // For defining many to many relation keys . 
        //protected override void OnModelCreating(ModelBuilder builder)
        //{

        //    builder.Entity<OrderService>().HasKey(sc => new { sc.ServiceId, sc.OrderId });

        //    base.OnModelCreating(builder);

        //}

        //public virtual DbSet<Barber> Barbers { get; set; }
       

    }
}
