using System.ComponentModel.DataAnnotations;

namespace MainMusicStore.Models.DbModels
{
    public class CoverType
    {
        [Key]
        public int Id { get; set; }


        [Display(Name = "Cover Type")]
        [Required(ErrorMessage = "You cannot leave this field blank!")]
        [StringLength(50 , MinimumLength = 3, ErrorMessage = "You can write a minimum of 3 characters and a maximum of 50 characters in this field !")]
        public string Name { get; set; }


    }
}
