using System.Linq;
using MainMusicStore.DataAccess.Data;
using MainMusicStore.DataAccess.IMainRepository;
using MainMusicStore.Models.DbModels;

namespace MainMusicStore.DataAccess.MainRepository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly MainMusicStoreDbContext _mainMusicStoreDbContext;

        public CategoryRepository(MainMusicStoreDbContext mainMusicStoreDbContext)
            : base(mainMusicStoreDbContext)
        {
            _mainMusicStoreDbContext = mainMusicStoreDbContext;
        }

        public void Update(Category category)
        {
            var categoryData = _mainMusicStoreDbContext.Categories.FirstOrDefault(x => x.Id == category.Id);
            if (categoryData != null)
            {
                categoryData.CategoryName = category.CategoryName;
            }
            _mainMusicStoreDbContext.SaveChanges();
        }
    }
}
