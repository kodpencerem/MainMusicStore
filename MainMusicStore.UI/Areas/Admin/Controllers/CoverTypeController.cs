using Dapper;
using MainMusicStore.DataAccess.IMainRepository;
using MainMusicStore.Models.DbModels;
using MainMusicStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainMusicStore.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ProjectConstant.RoleAdmin)]
    public class CoverTypeController : Controller
    {
        #region Variables

        private readonly IUnitOfWork _unitOfWork;

        #endregion Variables

        #region Constraction

        public CoverTypeController(IUnitOfWork unitOfWork)
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
            //var deleteData = _uow.CoverType.Get(id);
            //if (deleteData == null)
            //    return Json(new { success = false,message ="Data Not Found!"});

            //_uow.CoverType.Remove(deleteData);
            //_uow.Save();
            //return Json(new { success = true ,message ="Delete Operation Successfully"});
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@Id", id);

            var deleteData = _unitOfWork.SpCallRepository.OneRecord<CoverType>(ProjectConstant.ProcCoverTypeGet, dynamicParameters);
            if (deleteData == null)
                return Json(new { success = false, message = ProjectConstant.ResultNotFound });

            _unitOfWork.SpCallRepository.Execute(ProjectConstant.ProcCoverTypeDelete, dynamicParameters);
            _unitOfWork.Save();
            return Json(new { success = true, message = ProjectConstant.ResultSuccess});
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
            CoverType coverType = new CoverType();
            if (id == null)
            {
                //This for Create
                return View(coverType);
            }

            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@Id",id);
            coverType = _unitOfWork.SpCallRepository.OneRecord<CoverType>(ProjectConstant.ProcCoverTypeGet, dynamicParameters);

            //cat = _uow.CoverType.Get((int)id);
            if (coverType != null)
                return View(coverType);
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Name", coverType.Name);
                if (coverType.Id == 0)
                {
                    //Create
                    //_uow.CoverType.Add(CoverType);
                    _unitOfWork.SpCallRepository.Execute(ProjectConstant.ProcCoverTypeCreate, dynamicParameters);
                }
                else
                {
                    //Update
                    dynamicParameters.Add("@Id", coverType.Id);
                    //_uow.CoverType.Update(CoverType);
                    _unitOfWork.SpCallRepository.Execute(ProjectConstant.ProcCoverTypeUpdate, dynamicParameters);
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(coverType);
        }

        #endregion Actions

        #region API CALLS

        public IActionResult GetAll()
        {
            //var allObj = _uow.CoverType.GetAll();
            var allCoverTypes = _unitOfWork.SpCallRepository.List<CoverType>(ProjectConstant.ProcCoverTypeGetAll, null);
            return Json(new { data = allCoverTypes });
        }

        #endregion API CALLS
    }
}