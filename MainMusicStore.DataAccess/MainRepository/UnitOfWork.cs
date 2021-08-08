using MainMusicStore.DataAccess.Data;
using MainMusicStore.DataAccess.IMainRepository;

namespace MainMusicStore.DataAccess.MainRepository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MainMusicStoreDbContext _mainMusicStoreDbContext;

        public UnitOfWork(MainMusicStoreDbContext mainMusicStoreDbContext)
        {
            _mainMusicStoreDbContext = mainMusicStoreDbContext;
            CategoryRepository = new CategoryRepository(_mainMusicStoreDbContext);
            SpCallRepository = new SpCallRepository(_mainMusicStoreDbContext);
            CoverTypeRepository = new CoverTypeRepository(_mainMusicStoreDbContext);
        }


        public ICategoryRepository CategoryRepository { get; private set; }
        public ISpCallRepository SpCallRepository { get; private set; }

        public ICoverTypeRepository CoverTypeRepository { get; private set; }


        public void Save()
        {
            _mainMusicStoreDbContext.SaveChanges();
        }

        public void Dispose()
        {
            _mainMusicStoreDbContext.Dispose();
        }

        
    }
}
