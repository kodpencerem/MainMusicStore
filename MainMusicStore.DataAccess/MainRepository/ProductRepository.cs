using MainMusicStore.DataAccess.Data;
using MainMusicStore.DataAccess.IMainRepository;
using MainMusicStore.Models.DbModels;
using System.Linq;

namespace MainMusicStore.DataAccess.MainRepository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly MainMusicStoreDbContext _mainMusicStoreDbContext;

        public ProductRepository(MainMusicStoreDbContext mainMusicStoreDbContext)
            : base(mainMusicStoreDbContext)
        {
            _mainMusicStoreDbContext = mainMusicStoreDbContext;
        }

        public void Update(Product product)
        {
            var productData = _mainMusicStoreDbContext.Products.FirstOrDefault(x => x.Id == product.Id);
            if (productData != null)
            {
                if (product.ImageUrl != null)
                {
                    productData.ImageUrl = product.ImageUrl;
                }
                productData.Isbn = product.Isbn;
                productData.Price = product.Price;
                productData.Price50 = product.Price50;
                productData.Price100 = product.Price100;
                productData.ListPrice = product.ListPrice;
                productData.Title = product.Title;
                productData.Description = product.Description;
                productData.CategoryId = product.CategoryId;
                productData.CoverTypeId = product.CoverTypeId;
                productData.Author = product.Author;
            }
        }
    }
}
