using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace DnD_Character_Sheet_Creator.Models
{
    public class AppUser : IdentityUser
    {
        public int PlayerId { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 11)]
        [RegularExpression("^[0-9]*$")]
        public string OIB { get; set; } = string.Empty;

        [Required]
        [StringLength(13, MinimumLength = 13)]
        [RegularExpression("^[0-9]*$")]
        public string JMBG { get; set; } = string.Empty;
    }
}