using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class UserViewModels
    {
    }

    public class ListUserViewModel : ApplicationUser
    {
        public ListUserViewModel()
        {

        }
        public ListUserViewModel(ApplicationUser mainData)
        {
            this.UserName = mainData.UserName;
            this.Avatar = mainData.Avatar;
            this.FirstName = mainData.FirstName;
            this.LastName = mainData.LastName;
            this.Email = mainData.Email;
            this.listRoles = new List<string>();
        }
        public List<string> listRoles { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage =
            "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage =
            "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        // New Fields added to extend Application User class:

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        // Return a pre-poulated instance of AppliationUser:
        public ApplicationUser GetUser()
        {
            var user = new ApplicationUser()
            {
                FirstName = this.FirstName,
                LastName = this.LastName,
                Email = this.Email,
                UserName = this.Email
            };
            return user;
        }
    }
}