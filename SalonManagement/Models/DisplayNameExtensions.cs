using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SalonManagement.Models;

public static class DisplayNameExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? value.ToString();
    }

    public static string GetRoleDisplayName(string role) => role switch
    {
        "Admin" => "Quản trị viên",
        "Receptionist" => "Lễ tân",
        "Stylist" => "Thợ làm tóc",
        _ => role
    };
}
