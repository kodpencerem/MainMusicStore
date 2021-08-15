using MainMusicStore.DataAccess.IMainRepository;
using MainMusicStore.Models.DbModels;
using MainMusicStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainMusicStore.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ProjectConstant.RoleAdmin)]
    public class CategoryController : Controller
    {
        #region Variables

        private readonly IUnitOfWork _unitOfWork;

        #endregion Variables

        #region Constraction

        public CategoryController(IUnitOfWork unitOfWork)
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
            var deleteData = _unitOfWork.CategoryRepository.Get(id);
            if (deleteData == null)
                return Json(new { success = false, message = ProjectConstant.ResultNotFound });

            _unitOfWork.CategoryRepository.Remove(deleteData);
            _unitOfWork.Save();
            return Json(new { success = true, message = ProjectConstant.ResultSuccess });
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
            Category category = new Category();
            if (id == null)
            {
                //This for Create
                return View(category);
            }

            category = _unitOfWork.CategoryRepository.Get((int)id);
            if (category != null)
            {
                //This for Update
                return View(category);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    //Create
                    _unitOfWork.CategoryRepository.Add(category);
                }
                else
                {
                    //Update
                    _unitOfWork.CategoryRepository.Update(category);
                }

                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        #endregion Actions

        #region API CALLS

        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.CategoryRepository.GetAll();
            return Json(new { data = allObj });
        }

        #endregion API CALLS
    }
}