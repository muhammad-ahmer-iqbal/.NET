using System.ComponentModel.DataAnnotations;

namespace MyFirstApp.ViewModels
{
    public class CustomerLoginViewModel
    {
        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
