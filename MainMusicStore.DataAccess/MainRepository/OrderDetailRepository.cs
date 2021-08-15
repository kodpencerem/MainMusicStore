using MainMusicStore.DataAccess.Data;
using MainMusicStore.DataAccess.IMainRepository;
using MainMusicStore.Models.DbModels;

namespace MainMusicStore.DataAccess.MainRepository
{
    public class OrderDetailRepository : Repository<OrderDetails>, IOrderDetailRepository
    {
        private readonly MainMusicStoreDbContext _mainMusicStoreDbContext;

        public OrderDetailRepository( MainMusicStoreDbContext mainMusicStoreDbContext)
            : base(mainMusicStoreDbContext)
        {
            _mainMusicStoreDbContext = mainMusicStoreDbContext;
            
        }

        public void Update(OrderDetails orderDetails)
        {
            _mainMusicStoreDbContext.Update(orderDetails);
        }
    }
}