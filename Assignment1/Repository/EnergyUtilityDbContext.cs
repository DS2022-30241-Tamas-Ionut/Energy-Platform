using Assignment1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Repository
{
    public class EnergyUtilityDbContext : IdentityDbContext<User>
    {
        public DbSet<Device> Device { get; set; }
        public DbSet<EnergyConsumption> EnergyConsumption { get; set; }
        public EnergyUtilityDbContext()
        {

        }

        public EnergyUtilityDbContext(DbContextOptions<EnergyUtilityDbContext> options) : base(options)
        {

        }
    }
}
