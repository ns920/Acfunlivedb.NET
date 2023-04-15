using Acfunlivedb.NET.DAL.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acfunlivedb.NET.DAL
{
    public class AcfunlivedbDbContext : DbContext
    {
        public AcfunlivedbDbContext(DbContextOptions<AcfunlivedbDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Live> Lives { get; set; }
    }
}