using CEPAggregator.Classes;
using CEPAggregator.Classes.Helpers;
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
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, HelperParsersRegistration helperParsers)
            : base(options)
        {
            if (Database.EnsureCreated())
            {
                new InitDBHelper().InitDB(this, helperParsers.Parsers);
            }
        }
    }
}
