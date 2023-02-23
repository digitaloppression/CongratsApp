using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CongratsApp
{
    public class AppContext : DbContext
    {
        public DbSet<Birthday> Birthdays => Set<Birthday>();
        public AppContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = Birthdays.db");
        }
        
    }
}
