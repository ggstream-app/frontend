using GGStream.Models;
using Microsoft.EntityFrameworkCore;

namespace GGStream.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        public DbSet<Stream> Stream { get; set; }

        public DbSet<Collection> Collection { get; set; }
    }
}