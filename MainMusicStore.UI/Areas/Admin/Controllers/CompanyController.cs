using MainMusicStore.DataAccess.IMainRepository;
using MainMusicStore.Models.DbModels;
using MainMusicStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainMusicStore.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ProjectConstant.RoleAdmin + "," + ProjectConstant.RoleEmployee)]
    public class CompanyController : Controller
    {
        #region Variables

        private readonly IUnitOfWork _unitOfWork;

        #endregion Variables

        #region Constraction

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Constraction

        #region Actions

        public IActionResult Index()
        {
            return View();
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var deleteData = _unitOfWork.CompanyRepository.Get(id);
            if (deleteData == null)
                return Json(new { success = false, message =  ProjectConstant.ResultNotFound });

            _unitOfWork.CompanyRepository.Remove(deleteData);
            _unitOfWork.Save();
            return Json(new { success = true, message=ProjectConstant.ResultSuccess });
        }

        /// <summary>
        /// Create Or Update Get Method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ///
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if (id == null)
            {
                //This for Create
                return View(company);
            }

            company = _unitOfWork.CompanyRepository.Get((int)id);
            if (company != null)
            {
                return View(company);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    //Create
                    _unitOfWork.CompanyRepository.Add(company);
                }
                else
                {
                    //Update
                    _unitOfWork.CompanyRepository.Update(company);
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(company);
        }

        #endregion Actions

        #region API CALLS

        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.CompanyRepository.GetAll();
            return Json(new { data = allObj });
        }

        #endregion API CALLS
    }
}