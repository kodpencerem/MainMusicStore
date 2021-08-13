using System.Linq;
using MainMusicStore.DataAccess.Data;
using MainMusicStore.DataAccess.IMainRepository;
using MainMusicStore.Models.DbModels;

namespace MainMusicStore.DataAccess.MainRepository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly MainMusicStoreDbContext _mainMusicStoreDbContext;

        public ApplicationUserRepository(MainMusicStoreDbContext mainMusicStoreDbContext)
            : base(mainMusicStoreDbContext)
        {
            _mainMusicStoreDbContext = mainMusicStoreDbContext;
        }

        public void Update(ApplicationUser applicationUser)
        {
            var applicationUserData = _mainMusicStoreDbContext.ApplicationUsers.FirstOrDefault(x => x.Id == applicationUser.Id);
            if (applicationUserData != null)
            {
                applicationUserData.UserName = applicationUser.UserName;
            }
            _mainMusicStoreDbContext.SaveChanges();
        }
    }
}
