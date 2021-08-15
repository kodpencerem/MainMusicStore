using MainMusicStore.DataAccess.IMainRepository;
using MainMusicStore.Models.DbModels;
using MainMusicStore.DataAccess.Data;

namespace MainMusicStore.DataAccess.MainRepository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly MainMusicStoreDbContext _mainMusicStoreDbContext;

        public OrderHeaderRepository( MainMusicStoreDbContext mainMusicStoreDbContext)
            : base(mainMusicStoreDbContext)
        {
            _mainMusicStoreDbContext = mainMusicStoreDbContext;
           
        }

        public void Update(OrderHeader orderHeader)
        {
            _mainMusicStoreDbContext.Update(orderHeader);
        }
    }
}