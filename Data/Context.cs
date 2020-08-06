using Microsoft.EntityFrameworkCore;

namespace GGStream.Data
{
    public class Context : DbContext
    {
        public Context (DbContextOptions<Context> options)
            : base(options)
        {
        }

        public DbSet<Models.Stream> Stream { get; set; }

        public DbSet<Models.Collection> Collection { get; set; }
    }
}
