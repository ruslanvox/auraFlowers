using System.ComponentModel.DataAnnotations;

namespace aura.flowers.Models.Account
{
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }
    }
}