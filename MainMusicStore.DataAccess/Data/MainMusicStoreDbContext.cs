using MainMusicStore.Models.DbModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MainMusicStore.DataAccess.Data
{
    public class MainMusicStoreDbContext : IdentityDbContext
    {
        public MainMusicStoreDbContext(DbContextOptions<MainMusicStoreDbContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }

        public DbSet<Category> Categories { get; set; }
    }
}
