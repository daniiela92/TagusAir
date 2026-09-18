using Microsoft.EntityFrameworkCore;
using TagusAir.Data.Entities;

namespace TagusAir.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Airplane> Airplanes { get; set; }

    }
}
