using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MainMusicStore.DataAccess.IMainRepository;
using MainMusicStore.Models.DbModels;
using MainMusicStore.Models.ViewModels;
using MainMusicStore.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Stripe;

namespace MainMusicStore.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(IUnitOfWork uow, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _uow = uow;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        [BindProperty]
        public ShoppingCartVm ShoppingCartVm { get; set; }


        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVm = new ShoppingCartVm()
            {
                OrderHeader = new OrderHeader(),
                ListCart = _uow.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == claims.Value, includeProperties: "Product")
            };

            ShoppingCartVm.OrderHeader.OrderTotal = 0;
            ShoppingCartVm.OrderHeader.ApplicationUser = _uow.ApplicationUserRepository
                                                        .GetFirstOrDefault(u => u.Id == claims.Value, includeProperties: "Company");

            foreach (var cart in ShoppingCartVm.ListCart)
            {
                cart.Price = ProjectConstant.GetPriceBaseOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVm.OrderHeader.OrderTotal += (cart.Price * cart.Count);
                cart.Product.Description = ProjectConstant.ConvertToRawHtml(cart.Product.Description);

                if (cart.Product.Description.Length > 50)
                {
                    cart.Product.Description = cart.Product.Description.Substring(0, 49) + "....";
                }
            }

            return View(ShoppingCartVm);
        }

        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _uow.ApplicationUserRepository.GetFirstOrDefault(u => u.Id == claims.Value);

            if (user == null)
                ModelState.AddModelError(string.Empty, "Verification email is empty!");

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            ModelState.AddModelError(string.Empty, "verification emil sent.Please check your email!");
            return RedirectToAction("Index");
        }


        public IActionResult Plus(int id)
        {
            try
            {
                var cart = _uow.ShoppingCartRepository.GetFirstOrDefault(x => x.Id == id, includeProperties: "Product");

                if (cart == null)
                    return Json(false);
                //return RedirectToAction("Index");

                cart.Count += 1;
                cart.Price = ProjectConstant.GetPriceBaseOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);

                _uow.Save();
                //var allShoppingCart = _uow.ShoppingCart.GetAll();

                return Json(true);
                //return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        public IActionResult Minus(int cartId)
        {
            var cart = _uow.ShoppingCartRepository.GetFirstOrDefault(x => x.Id == cartId, includeProperties: "Product");
            if (cart.Count == 1)
            {
                var cnt = _uow.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                _uow.ShoppingCartRepository.Remove(cart);
                _uow.Save();
                HttpContext.Session.SetInt32(ProjectConstant.ShoppingCart, cnt - 1);
            }
            else
            {
                cart.Count -= 1;
                cart.Price = ProjectConstant.GetPriceBaseOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                _uow.Save();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int cartId)
        {
            var cart = _uow.ShoppingCartRepository.GetFirstOrDefault(x => x.Id == cartId, includeProperties: "Product");

            var cnt = _uow.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            _uow.ShoppingCartRepository.Remove(cart);
            _uow.Save();
            HttpContext.Session.SetInt32(ProjectConstant.ShoppingCart, cnt - 1);

            return RedirectToAction("Index");
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVm = new ShoppingCartVm()
            {
                OrderHeader = new OrderHeader(),
                ListCart = _uow.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == claims.Value, includeProperties: "Product")
            };

            ShoppingCartVm.OrderHeader.ApplicationUser = _uow.ApplicationUserRepository
                                                        .GetFirstOrDefault(u => u.Id == claims.Value, includeProperties: "Company");

            foreach (var item in ShoppingCartVm.ListCart)
            {
                item.Price = ProjectConstant.GetPriceBaseOnQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
                ShoppingCartVm.OrderHeader.OrderTotal += (item.Price * item.Count);
            }

            ShoppingCartVm.OrderHeader.Name = ShoppingCartVm.OrderHeader.ApplicationUser.Name;
            ShoppingCartVm.OrderHeader.PhoneNumber = ShoppingCartVm.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVm.OrderHeader.StreetAddress = ShoppingCartVm.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVm.OrderHeader.City = ShoppingCartVm.OrderHeader.ApplicationUser.City;
            ShoppingCartVm.OrderHeader.State = ShoppingCartVm.OrderHeader.ApplicationUser.State;
            ShoppingCartVm.OrderHeader.PostCode = ShoppingCartVm.OrderHeader.ApplicationUser.PostCode;

            return View(ShoppingCartVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummmaryPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVm.OrderHeader.ApplicationUser = _uow.ApplicationUserRepository.GetFirstOrDefault(a => a.Id == claims.Value, includeProperties: "Company");

            ShoppingCartVm.ListCart = _uow.ShoppingCartRepository.GetAll(s => s.ApplicationUserId == claims.Value, includeProperties: "Product");

            ShoppingCartVm.OrderHeader.PaymentStatus = ProjectConstant.PaymentStatusPending;
            ShoppingCartVm.OrderHeader.OrderStatus = ProjectConstant.StatusPending;
            ShoppingCartVm.OrderHeader.ApplicationUserId = claims.Value;
            ShoppingCartVm.OrderHeader.OrderDate = DateTime.Now;

            _uow.OrderHeaderRepository.Add(ShoppingCartVm.OrderHeader);
            _uow.Save();

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            foreach (var orderDetail in ShoppingCartVm.ListCart)
            {
                orderDetail.Price = ProjectConstant.GetPriceBaseOnQuantity(orderDetail.Count, orderDetail.Product.Price, orderDetail.Product.Price50, orderDetail.Product.Price100);

                OrderDetails oDetails = new OrderDetails()
                {
                    ProductId = orderDetail.ProductId,
                    OrderId = ShoppingCartVm.OrderHeader.Id,
                    Price = orderDetail.Price,
                    Count = orderDetail.Count
                };
                ShoppingCartVm.OrderHeader.OrderTotal += oDetails.Count * oDetails.Price;
                _uow.OrderDetailRepository.Add(oDetails);
            }
            _uow.ShoppingCartRepository.RemoveRange(ShoppingCartVm.ListCart);
            _uow.Save();
            HttpContext.Session.SetInt32(ProjectConstant.ShoppingCart, 0);

            if (stripeToken == null)
            {
                ShoppingCartVm.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
                ShoppingCartVm.OrderHeader.PaymentStatus = ProjectConstant.PaymentStatusDelayedPayment;
                ShoppingCartVm.OrderHeader.OrderStatus = ProjectConstant.StatusApproved;
            }
            else
            {
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(ShoppingCartVm.OrderHeader.OrderTotal * 100),
                    Currency = "usd",
                    Description = "Order Id : " + ShoppingCartVm.OrderHeader.Id,
                    Source = stripeToken
                };

                var service = new ChargeService();
                Charge charge = service.Create(options);

                if (charge.BalanceTransactionId == null)
                    ShoppingCartVm.OrderHeader.PaymentStatus = ProjectConstant.PaymentStatusRejected;
                else
                    ShoppingCartVm.OrderHeader.TransactionId = charge.BalanceTransactionId;

                if (charge.Status.ToLower()=="succeeded")
                {
                    ShoppingCartVm.OrderHeader.PaymentStatus = ProjectConstant.PaymentStatusApproved;
                    ShoppingCartVm.OrderHeader.OrderStatus = ProjectConstant.StatusApproved;
                    ShoppingCartVm.OrderHeader.PaymentDate = DateTime.Now;
                }
            }
            _uow.Save();
            return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVm.OrderHeader.Id });
        }


        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }
    }
}