using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using MainMusicStore.DataAccess.IMainRepository;
using MainMusicStore.Models.DbModels;
using MainMusicStore.Models.ViewModels;
using MainMusicStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace MainMusicStore.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _uow;

        [BindProperty]
        public OrderDetailsVm OrderDetailVm { get; set; }

        public OrderController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            OrderDetailVm = new OrderDetailsVm
            {
                OrderHeader = _uow.OrderHeaderRepository.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser"),
                OrderDetails = _uow.OrderDetailRepository.GetAll(o => o.OrderId == id, includeProperties: "Product")
            };

            return View(OrderDetailVm);
        }

        [Authorize(Roles = ProjectConstant.RoleAdmin + "," + ProjectConstant.RoleEmployee)]
        public IActionResult StartProcessing(int id)
        {
            OrderHeader orderHeader = _uow.OrderHeaderRepository.GetFirstOrDefault(u => u.Id == id);
            orderHeader.OrderStatus = ProjectConstant.StatusInProcess;
            _uow.Save();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = ProjectConstant.RoleAdmin + "," + ProjectConstant.RoleEmployee)]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _uow.OrderHeaderRepository.GetFirstOrDefault(u => u.Id == OrderDetailVm.OrderHeader.Id);
            orderHeader.TrackingNumber = OrderDetailVm.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderDetailVm.OrderHeader.Carrier;
            orderHeader.OrderStatus = ProjectConstant.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            _uow.Save();
            return RedirectToAction("Index");
        }


        [Authorize(Roles = ProjectConstant.RoleAdmin + "," + ProjectConstant.RoleEmployee)]
        public IActionResult CancelOrder(int id)
        {
            OrderHeader orderHeader = _uow.OrderHeaderRepository.GetFirstOrDefault(u => u.Id == id);

            if (orderHeader.PaymentStatus == ProjectConstant.StatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                    Reason = RefundReasons.RequestedByCustomer,
                    Charge = orderHeader.TransactionId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                orderHeader.OrderStatus = ProjectConstant.StatusRefund;
                orderHeader.PaymentStatus = ProjectConstant.StatusRefund;
            }
            else
            {
                orderHeader.OrderStatus = ProjectConstant.StatusCancelled;
                orderHeader.PaymentStatus = ProjectConstant.StatusCancelled;
            }
            _uow.Save();
            return RedirectToAction("Index");
        }

        public IActionResult UpdateOrderDetails()
        {
            var orderHEaderFromDb = _uow.OrderHeaderRepository.GetFirstOrDefault(u => u.Id == OrderDetailVm.OrderHeader.Id);
            orderHEaderFromDb.Name = OrderDetailVm.OrderHeader.Name;
            orderHEaderFromDb.PhoneNumber = OrderDetailVm.OrderHeader.PhoneNumber;
            orderHEaderFromDb.StreetAddress = OrderDetailVm.OrderHeader.StreetAddress;
            orderHEaderFromDb.City = OrderDetailVm.OrderHeader.City;
            orderHEaderFromDb.State = OrderDetailVm.OrderHeader.State;
            orderHEaderFromDb.PostCode = OrderDetailVm.OrderHeader.PostCode;
            if (OrderDetailVm.OrderHeader.Carrier != null)
            {
                orderHEaderFromDb.Carrier = OrderDetailVm.OrderHeader.Carrier;
            }
            if (OrderDetailVm.OrderHeader.TrackingNumber != null)
            {
                orderHEaderFromDb.TrackingNumber = OrderDetailVm.OrderHeader.TrackingNumber;
            }

            _uow.Save();
            TempData["Error"] = "Order Details Updated Successfully.";
            return RedirectToAction("Details", "Order", new { id = orderHEaderFromDb.Id });
        }

        #region APICALLS

        [HttpGet]
        public IActionResult GetOrderList(string status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> orderHeaderList;

            if (User.IsInRole(ProjectConstant.RoleAdmin) || User.IsInRole(ProjectConstant.RoleEmployee))
                orderHeaderList = _uow.OrderHeaderRepository.GetAll(includeProperties: "ApplicationUser");
            else
                orderHeaderList = _uow.OrderHeaderRepository.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");

            switch (status)
            {
                case "pending":
                    orderHeaderList = orderHeaderList.Where(o => o.PaymentStatus == ProjectConstant.PaymentStatusDelayedPayment);
                    break;

                case "inprocess":
                    orderHeaderList = orderHeaderList.Where(o =>
                                        o.OrderStatus == ProjectConstant.StatusApproved
                                        || o.OrderStatus == ProjectConstant.StatusInProcess
                                        || o.OrderStatus == ProjectConstant.StatusPending);
                    break;

                case "completed":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == ProjectConstant.StatusShipped);
                    break;

                case "rejected":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == ProjectConstant.StatusCancelled
                                                            || o.OrderStatus == ProjectConstant.StatusRefund
                                                            || o.OrderStatus == ProjectConstant.PaymentStatusRejected);
                    break;

                default:
                    break;
            }


            //orderHeaderList = _uow.OrderHeader.GetAll(includeProperties: "ApplicationUser");

            return Json(new { data = orderHeaderList });
        }

        #endregion

    }
}