using System.Linq;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Configuration;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using JetBrains.Annotations;

namespace SW.Dms
{
    public class DmsContext : DbContext
    {
        public DmsContext( DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.BuildDms();
            modelBuilder.SeedSampleData();
        }
    }
}
