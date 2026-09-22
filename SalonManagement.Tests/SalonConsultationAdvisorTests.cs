using SalonManagement.Models;
using SalonManagement.Services;

namespace SalonManagement.Tests;

public class SalonConsultationAdvisorTests
{
    [Fact]
    public void Build_PrioritizesRecoveryService_ForDamagedHairWithinBudget()
    {
        var services = new[]
        {
            new SalonService { Id = 1, Name = "Cắt tóc nữ", Price = 180000, DurationMinutes = 45, Status = true },
            new SalonService { Id = 2, Name = "Hấp phục hồi keratin", Price = 450000, DurationMinutes = 75, Status = true },
            new SalonService { Id = 3, Name = "Nhuộm thời trang", Price = 850000, DurationMinutes = 120, Status = true }
        };
        var profile = new SalonConsultationProfile
        {
            CustomerNeed = "Tóc khô xơ sau khi tẩy, muốn phục hồi",
            HairCondition = "Khô, xơ hoặc hư tổn",
            DesiredStyle = "Phục hồi tóc",
            Budget = 500000,
            MaintenancePreference = "Có thể chăm sóc cơ bản"
        };

        var results = SalonConsultationAdvisor.Build(services, [], profile);

        Assert.Equal("Hấp phục hồi keratin", results.First().Service.Name);
        Assert.Contains("phục hồi", results.First().Reason, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Build_PrioritizesColorService_WhenCustomerWantsToChangeColor()
    {
        var services = new[]
        {
            new SalonService { Id = 1, Name = "Phục hồi keratin", Price = 650000, DurationMinutes = 120, Status = true },
            new SalonService { Id = 2, Name = "Nhuộm tóc", Price = 850000, DurationMinutes = 150, Status = true }
        };
        var history = new[]
        {
            new ServiceHistory { ServiceId = 1, ServiceDate = DateTime.Today },
            new ServiceHistory { ServiceId = 1, ServiceDate = DateTime.Today.AddDays(-30) },
            new ServiceHistory { ServiceId = 1, ServiceDate = DateTime.Today.AddDays(-60) }
        };

        var results = SalonConsultationAdvisor.Build(services, history, new SalonConsultationProfile
        {
            CustomerNeed = "Khách muốn thay đổi màu sắc, lên màu mới",
            HairCondition = "Tóc khỏe, bình thường",
            DesiredStyle = "Đổi màu/nhuộm tóc",
            MaintenancePreference = "Có thể chăm sóc cơ bản"
        });

        Assert.Equal("Nhuộm tóc", results.First().Service.Name);
    }
}
