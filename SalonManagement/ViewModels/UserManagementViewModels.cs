using System.ComponentModel.DataAnnotations;

namespace SalonManagement.ViewModels;

public class UserRowViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class CreateUserViewModel
{
    [Required, EmailAddress, Display(Name = "Email / tên đăng nhập")]
    public string Email { get; set; } = string.Empty;
    [Required, StringLength(100, MinimumLength = 8), DataType(DataType.Password), Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;
    [Required, Display(Name = "Vai trò")]
    public string Role { get; set; } = "Receptionist";
}
