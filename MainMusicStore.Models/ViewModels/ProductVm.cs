using System.Collections.Generic;
using MainMusicStore.Models.DbModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MainMusicStore.Models.ViewModels
{
    public class ProductVm
    {
        public Product Product{ get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public IEnumerable<SelectListItem> CoverTypeList { get; set; }
    }
}
