using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class AppContext : DbContext
    {
        readonly string connectionString;

        public AppContext(string connString)
        {
            connectionString = connString;
        }

        public DbSet<Todo> Todos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
