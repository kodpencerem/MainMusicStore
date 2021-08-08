using System;

namespace MainMusicStore.DataAccess.IMainRepository
{
    public interface IUnitOfWork : IDisposable
    {

        ICategoryRepository CategoryRepository { get; }
        ISpCallRepository SpCallRepository { get; }
        ICoverTypeRepository CoverTypeRepository { get; }

        void Save();
    }
}
