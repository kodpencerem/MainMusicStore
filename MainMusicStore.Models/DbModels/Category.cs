using System.ComponentModel.DataAnnotations;

namespace MainMusicStore.Models.DbModels
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "You cannot leave this field blank!")]
        [StringLength(250 , MinimumLength = 3, ErrorMessage = "You can write a minimum of 3 characters and a maximum of 250 characters in this field !")]
        public string CategoryName { get; set; }

    }
}
