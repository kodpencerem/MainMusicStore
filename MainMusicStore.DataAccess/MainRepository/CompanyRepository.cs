using MainMusicStore.DataAccess.Data;
using MainMusicStore.DataAccess.IMainRepository;
using MainMusicStore.Models.DbModels;

namespace MainMusicStore.DataAccess.MainRepository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly MainMusicStoreDbContext _mainMusicStoreDbContext;

        public CompanyRepository(MainMusicStoreDbContext mainMusicStoreDbContext)
            : base(mainMusicStoreDbContext)
        {
            _mainMusicStoreDbContext = mainMusicStoreDbContext;
        }

        public void Update(Company company)
        {
            _mainMusicStoreDbContext.Update(company);
        }
    }
}
