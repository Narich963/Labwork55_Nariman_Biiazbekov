using System.ComponentModel.DataAnnotations;

namespace To_Do_List.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Name field is empty")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Email field is empty")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password field is empty")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords are not equal")]
    public string ConfirmPassword { get; set; }
}
