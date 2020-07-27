using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GGStream.Models;

namespace GGStream.Data
{
    public class Context : DbContext
    {
        public Context (DbContextOptions<Context> options)
            : base(options)
        {
        }

        public DbSet<GGStream.Models.Stream> Stream { get; set; }

        public DbSet<GGStream.Models.Collection> Collection { get; set; }
    }
}
