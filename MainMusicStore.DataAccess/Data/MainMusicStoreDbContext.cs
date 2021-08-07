using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MainMusicStore.DataAccess.Data
{
    public class MainMusicStoreDbContext : IdentityDbContext
    {
        public MainMusicStoreDbContext(DbContextOptions<MainMusicStoreDbContext> options)
            : base(options)
        {
        }
    }
}
