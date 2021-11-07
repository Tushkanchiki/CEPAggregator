using CEPAggregator.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CEPAggregator.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<CEP> CEPs { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            if (!Database.EnsureCreated())
            {
                InitDB();
            }
        }

        private void InitDB()
        {
            int i = 0;
        }
    }
}
