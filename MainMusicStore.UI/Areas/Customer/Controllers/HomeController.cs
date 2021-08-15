using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using MainMusicStore.DataAccess.IMainRepository;
using MainMusicStore.Models.DbModels;
using MainMusicStore.UI.Models;
using MainMusicStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MainMusicStore.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category,CoverType");

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var shoppingCount = _unitOfWork.ShoppingCartRepository.GetAll(a => a.ApplicationUserId == claim.Value).ToList().Count();

                HttpContext.Session.SetInt32(ProjectConstant.ShoppingCart, shoppingCount);
            }
            return View(productList);
        }

        public IActionResult Details(int id)
        {
            var product = _unitOfWork.ProductRepository.GetFirstOrDefault(p => p.Id == id, includeProperties: "Category,CoverType");

            ShoppingCart cart = new ShoppingCart()
            {
                Product = product,
                ProductId = product.Id
            };
            return View(cart);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cartObj)
        {
            cartObj.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartObj.ApplicationUserId = claim.Value;

                ShoppingCart fromDb = _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(
                    s => s.ApplicationUserId == cartObj.ApplicationUserId
                    && s.ProductId == cartObj.ProductId,
                    includeProperties: "Product");

                if (fromDb == null)
                {
                    //Insert
                    _unitOfWork.ShoppingCartRepository.Add(cartObj);
                }
                else
                {
                    //Update
                    fromDb.Count += cartObj.Count;
                }

                _unitOfWork.Save();

                var shoppingCount = _unitOfWork.ShoppingCartRepository.GetAll(a => a.ApplicationUserId == cartObj.ApplicationUserId).ToList().Count();

                HttpContext.Session.SetInt32(ProjectConstant.ShoppingCart, shoppingCount);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var product = _unitOfWork.ProductRepository.GetFirstOrDefault(p => p.Id == cartObj.ProductId, includeProperties: "Category,CoverType");

                ShoppingCart cart = new ShoppingCart()
                {
                    Product = product,
                    ProductId = product.Id
                };
                return View(cart);
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
