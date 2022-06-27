using ActiveXTService.Data.Model;
using Microsoft.EntityFrameworkCore;


namespace ActiveXTService.Data
{
    public class SftpDataContext : DbContext
    {
        public SftpDataContext(DbContextOptions<SftpDataContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.UseSerialColumns();
        }
        public DbSet<ImportedFile> ImportedFiles { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}
