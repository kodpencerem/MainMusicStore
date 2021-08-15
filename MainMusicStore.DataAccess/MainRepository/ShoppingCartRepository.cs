using MainMusicStore.DataAccess.IMainRepository;
using MainMusicStore.Models.DbModels;
using MainMusicStore.DataAccess.Data;

namespace MainMusicStore.DataAccess.MainRepository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly MainMusicStoreDbContext _mainMusicStoreDbContext;

        public ShoppingCartRepository(MainMusicStoreDbContext mainMusicStoreDbContext)
            : base(mainMusicStoreDbContext)
        {
            _mainMusicStoreDbContext = mainMusicStoreDbContext;
            
        }

        public void Update(ShoppingCart shoppingCart)
        {
            _mainMusicStoreDbContext.Update(shoppingCart);
        }
    }
}