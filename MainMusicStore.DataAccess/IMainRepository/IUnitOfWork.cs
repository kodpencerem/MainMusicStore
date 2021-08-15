using System;

namespace MainMusicStore.DataAccess.IMainRepository
{
    public interface IUnitOfWork : IDisposable
    {

        ICategoryRepository CategoryRepository { get; }
        ISpCallRepository SpCallRepository { get; }
        ICoverTypeRepository CoverTypeRepository { get; }
        IProductRepository  ProductRepository { get; }
        ICompanyRepository CompanyRepository { get; }
        IApplicationUserRepository ApplicationUserRepository { get; }
        IShoppingCartRepository ShoppingCartRepository { get; }
        IOrderHeaderRepository OrderHeaderRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }

        void Save();
    }
}
