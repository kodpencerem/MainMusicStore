using System;
using System.Linq;
using MainMusicStore.DataAccess.Data;
using MainMusicStore.DataAccess.IMainRepository;
using MainMusicStore.Models.DbModels;
using MainMusicStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MainMusicStore.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ProjectConstant.RoleAdmin)]
    public class UserController : Controller
    {
        #region Variables

        private readonly MainMusicStoreDbContext _mainMusicStoreDbContext;

        #endregion Variables

        #region Constraction

        public UserController(IUnitOfWork unitOfWork, MainMusicStoreDbContext mainMusicStoreDbContext)
        {
            _mainMusicStoreDbContext = mainMusicStoreDbContext;
        }

        #endregion Constraction

        #region Actions

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var data = _mainMusicStoreDbContext.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (data == null)
                return Json(new { success = false, message = "Error while locking/unlocking" });

            if (data.LockoutEnd != null && data.LockoutEnd > DateTime.Now)
                data.LockoutEnd = DateTime.Now;
            else
                data.LockoutEnd = DateTime.Now.AddYears(10);

            _mainMusicStoreDbContext.SaveChanges();
            return Json(new { success = true, message = "Operation Successfully" });
        }



        #endregion Actions

        #region API CALLS

        public IActionResult GetAll()
        {
            var userList = _mainMusicStoreDbContext.ApplicationUsers.Include(c => c.Company).ToList();
            var userRole = _mainMusicStoreDbContext.UserRoles.ToList();
            var roles = _mainMusicStoreDbContext.Roles.ToList();

            foreach (var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id)?.RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId)?.Name;

                user.Company ??= new Company()
                {
                    Name = string.Empty
                };
            }
            return Json(new { data = userList });
        }

        #endregion API CALLS
    }
}