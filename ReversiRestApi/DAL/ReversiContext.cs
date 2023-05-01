using Microsoft.EntityFrameworkCore;

namespace ReversiRestApi.DAL
{
    public class ReversiContext : DbContext
    {
        public ReversiContext(DbContextOptions options) : base(options) { }
        public DbSet<Spel> Spellen { get; set; }
    }
}