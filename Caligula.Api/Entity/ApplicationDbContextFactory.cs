using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Caligula.Service.Entity
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Adjust the path to your appsettings.json file if necessary
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .Build();

            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Caligula;Trusted_Connection=True;");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
