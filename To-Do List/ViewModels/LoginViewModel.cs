using System.ComponentModel.DataAnnotations;

namespace To_Do_List.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email field is empty")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password field is empty")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}
