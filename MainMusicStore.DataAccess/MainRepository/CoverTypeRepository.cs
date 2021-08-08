using System.Linq;
using MainMusicStore.DataAccess.Data;
using MainMusicStore.DataAccess.IMainRepository;
using MainMusicStore.Models.DbModels;

namespace MainMusicStore.DataAccess.MainRepository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly MainMusicStoreDbContext _mainMusicStoreDbContext;

        public CoverTypeRepository(MainMusicStoreDbContext mainMusicStoreDbContext)
            : base(mainMusicStoreDbContext)
        {
            _mainMusicStoreDbContext = mainMusicStoreDbContext;
        }

        public void Update(CoverType coverType)
        {
            var coverTypeData = _mainMusicStoreDbContext.CoverTypes.FirstOrDefault(x => x.Id == coverType.Id);
            if (coverTypeData != null)
            {
                coverTypeData.Name = coverType.Name;
            }
            _mainMusicStoreDbContext.SaveChanges();
        }
    }
}
