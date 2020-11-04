using System.ComponentModel.DataAnnotations;

namespace aura.flowers.Models
{
    public class ContactUsViewModel
    {
        [StringLength(100)]
        [Required(ErrorMessage = "name-required")]
        public string Name { get; set; }

        [EmailAddress]
        [StringLength(100)]
        [Required(ErrorMessage = "email-required")]
        public string Email { get; set; }

        [StringLength(3000)]
        [Required(ErrorMessage = "message-required")]
        public string Message { get; set; }

        public int SelectedProductId { get; set; }
    }
}
