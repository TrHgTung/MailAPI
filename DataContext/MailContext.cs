using MailAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MailAPI.DataContext
{
    public class MailContext : DbContext
    {
        public MailContext(DbContextOptions<MailContext> options) : base(options)
        {
            
        }

        public DbSet<MailModel> Mails { get; set; }
    }
}
