using DevTalles.Ecommerce.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DevTalles.Ecommerce.WebAPI.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        DbContext(options)
    {
        public DbSet<Category> Categories { get; set; }
    }
}
