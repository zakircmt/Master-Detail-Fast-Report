using MasterDetailFastReport.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MasterDetailFastReport.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<TblPurchase> Purchases { get; set; }
        public DbSet<TblPurchaseDetail> Details { get; set; }
        public DbSet<TblProductExpanse> Expanses { get; set; }
    }
}