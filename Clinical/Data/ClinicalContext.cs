using Clinical.Models;
using Microsoft.EntityFrameworkCore;

namespace Clinical.Data
{
    public class ClinicalContext : DbContext
    {
        public ClinicalContext(DbContextOptions<ClinicalContext> options)
            : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Priority> Priority { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketAllocation> TicketAllocation { get; set; }
        public DbSet<AuditLog> AuditLog { get; set; }
        public DbSet<Notification> Notification { get; set; }
    }
}
