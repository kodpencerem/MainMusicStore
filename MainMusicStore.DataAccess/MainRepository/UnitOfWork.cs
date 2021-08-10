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
            ProductRepository = new ProductRepository(_mainMusicStoreDbContext);
            CompanyRepository = new CompanyRepository(_mainMusicStoreDbContext);
        }


        public ICategoryRepository CategoryRepository { get; private set; }
        public ISpCallRepository SpCallRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public ICoverTypeRepository CoverTypeRepository { get; private set; }
        public ICompanyRepository CompanyRepository { get; set; }

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
