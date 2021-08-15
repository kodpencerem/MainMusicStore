using System;
using System.IO;
using System.Linq;
using MainMusicStore.DataAccess.IMainRepository;
using MainMusicStore.Models.DbModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MainMusicStore.Models.ViewModels;
using MainMusicStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MainMusicStore.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ProjectConstant.RoleAdmin)]
    public class ProductController : Controller
    {
        #region Variables

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        #endregion Variables

        #region Constraction

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
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
            var deleteData = _unitOfWork.ProductRepository.Get(id);
            if (deleteData == null)
                return Json(new { success = false, message = ProjectConstant.ResultNotFound });

            string webRootPath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, deleteData.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _unitOfWork.ProductRepository.Remove(deleteData);
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
            ProductVm productVm = new ProductVm()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(i=> new SelectListItem
                {
                    Text = i.CategoryName,
                    Value = i.Id.ToString()
                })
                ,

                 CoverTypeList= _unitOfWork.CoverTypeRepository.GetAll().Select(i=> new SelectListItem
                {
                Text = i.Name,
                Value = i.Id.ToString()
            })
            };
            if (id==null)
            {
                return View(productVm);
            }
            else
            {
                productVm.Product = _unitOfWork.ProductRepository.Get(id.GetValueOrDefault());
                if (productVm.Product == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(productVm);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVm productVm)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;

                var files = HttpContext.Request.Form.Files;
                if (files.Count>0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\products");
                    var extenstion = Path.GetExtension(files[0].FileName);

                    if (productVm.Product.ImageUrl != null)
                    {
                        var imageUrl = productVm.Product.ImageUrl;
                        var imagePath = Path.Combine(webRootPath, productVm.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extenstion), FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams);
                    }

                    productVm.Product.ImageUrl = @"\images\products\" + fileName + extenstion;

                    var pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                    using (var fileStreams = new FileStream(Path.Combine(pathRoot, fileName + extenstion), FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams);
                    }
                }

                else
                {
                    if (productVm.Product.Id != 0)
                    {
                        var productData = _unitOfWork.ProductRepository.Get(productVm.Product.Id);
                        productVm.Product.ImageUrl = productData.ImageUrl;
                    }
                }


                if (productVm.Product.Id == 0)
                {
                    //Create
                    _unitOfWork.ProductRepository.Add(productVm.Product);
                }
                else
                {
                    //Update
                    _unitOfWork.ProductRepository.Update(productVm.Product);
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                productVm.CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(a => new SelectListItem
                {
                    Text = a.CategoryName,
                    Value = a.Id.ToString()
                });

                productVm.CoverTypeList = _unitOfWork.CoverTypeRepository.GetAll().Select(a => new SelectListItem
                {
                    Text = a.Name,
                    Value = a.Id.ToString()
                });

                if (productVm.Product.Id != 0)
                {
                    productVm.Product = _unitOfWork.ProductRepository.Get(productVm.Product.Id);
                }
            }
            return View(productVm);
        }

        #endregion Actions

        #region API CALLS

        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category");
            return Json(new { data = allObj });
        }

        #endregion API CALLS
    }
}




//Product product = new Product();
//if (id == null)
//{
//    //This for Create
//    return View(product);
//}

//product = _unitOfWork.ProductRepository.Get((int)id);
//if (product != null)
//{
//    //This for Update
//    return View(product);
//}

//return NotFound();