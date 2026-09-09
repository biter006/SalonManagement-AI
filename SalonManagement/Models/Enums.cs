using System.ComponentModel.DataAnnotations;

namespace SalonManagement.Models;

public enum AppointmentStatus
{
    [Display(Name = "Chờ xác nhận")]
    Pending,
    [Display(Name = "Đã xác nhận")]
    Confirmed,
    [Display(Name = "Đã hoàn thành")]
    Completed,
    [Display(Name = "Đã hủy")]
    Cancelled,
    [Display(Name = "Khách vắng mặt")]
    NoShow
}

public enum StylistStatus
{
    [Display(Name = "Đang làm việc")]
    Active,
    [Display(Name = "Đang nghỉ")]
    OnLeave,
    [Display(Name = "Ngừng làm việc")]
    Inactive
}

public enum ScheduleStatus
{
    [Display(Name = "Làm việc")]
    Working,
    [Display(Name = "Ngày nghỉ")]
    DayOff
}

public enum PaymentMethod
{
    [Display(Name = "Tiền mặt")]
    Cash,
    [Display(Name = "Chuyển khoản")]
    BankTransfer,
    [Display(Name = "Thẻ")]
    Card
}

public enum PaymentStatus
{
    [Display(Name = "Chưa thanh toán")]
    Unpaid,
    [Display(Name = "Đã thanh toán")]
    Paid,
    [Display(Name = "Đã hoàn tiền")]
    Refunded
}

public enum MessageType
{
    [Display(Name = "Nhắc lịch")]
    Reminder,
    [Display(Name = "Cảm ơn sau dịch vụ")]
    ThankYou,
    [Display(Name = "Chăm sóc khách cũ")]
    FollowUp,
    [Display(Name = "Mời khách quay lại")]
    ReturnInvitation
}
