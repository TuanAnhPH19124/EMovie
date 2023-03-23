using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EMovie.Models
{
    public class ApplicaitonUser : IdentityUser
    {
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
    }
}
