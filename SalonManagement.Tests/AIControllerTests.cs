using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SalonManagement.Controllers;
using SalonManagement.Data;
using SalonManagement.Models;
using SalonManagement.Services;
using SalonManagement.ViewModels;

namespace SalonManagement.Tests;

public class AIControllerTests
{
    [Fact]
    public async Task Recommendation_UsesMockResponse_AndPersistsResult()
    {
        await using var context = Context();
        var customer = await AddCustomerAsync(context);
        var controller = new AIController(context, new StubAIService(new AITextResult(true, "Gợi ý test")), NullLogger<AIController>.Instance);
        var result = await controller.Recommendation(ModelFor(customer.Id));
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AIRecommendationViewModel>(view.Model);
        Assert.Equal("Gợi ý test", model.Result);
        Assert.Single(context.AIRecommendations);
    }

    [Fact]
    public async Task Recommendation_EmptyAIResponse_DoesNotPersist()
    {
        await using var context = Context();
        var customer = await AddCustomerAsync(context);
        var controller = new AIController(context, new StubAIService(new AITextResult(false, string.Empty, "AI trả về rỗng")), NullLogger<AIController>.Instance);
        await controller.Recommendation(ModelFor(customer.Id));
        Assert.Empty(context.AIRecommendations);
    }

    [Fact]
    public async Task Recommendation_AIException_IsHandledWithoutDatabaseChange()
    {
        await using var context = Context();
        var customer = await AddCustomerAsync(context);
        var controller = new AIController(context, new ThrowingAIService(), NullLogger<AIController>.Instance);
        var result = await controller.Recommendation(ModelFor(customer.Id));
        var view = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ViewData.ModelState.IsValid);
        Assert.Empty(context.AIRecommendations);
    }

    [Fact]
    public async Task Recommendation_StylistCannotAccessUnassignedCustomer()
    {
        await using var context = Context();
        var customer = await AddCustomerAsync(context);
        var controller = StylistController(context, "stylist@test.local");

        var result = await controller.Recommendation(ModelFor(customer.Id));

        Assert.IsType<ForbidResult>(result);
        Assert.Empty(context.AIRecommendations);
    }

    [Fact]
    public async Task Recommendation_StylistOnlySeesAssignedCustomers()
    {
        await using var context = Context();
        var assignedCustomer = await AddCustomerAsync(context);
        _ = await AddCustomerAsync(context);
        await AssignCustomerToStylistAsync(context, assignedCustomer, "stylist@test.local");
        var controller = StylistController(context, "stylist@test.local");

        var result = await controller.Recommendation((int?)null);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AIRecommendationViewModel>(view.Model);
        var option = Assert.Single(model.Customers);
        Assert.Equal(assignedCustomer.Id.ToString(), option.Value);
    }

    private static ApplicationDbContext Context() => new(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
    private static async Task<Customer> AddCustomerAsync(ApplicationDbContext context)
    {
        var customer = new Customer { FullName = "AI Test", Phone = "0900555555" };
        context.Customers.Add(customer); context.SalonServices.Add(new SalonService { Name = "Cắt test", Price = 100000, DurationMinutes = 60 }); await context.SaveChangesAsync(); return customer;
    }
    private static async Task AssignCustomerToStylistAsync(ApplicationDbContext context, Customer customer, string email)
    {
        var stylist = new Stylist { FullName = "Thợ test", Phone = "0900666666", Email = email, Specialization = "Cắt tóc" };
        var service = await context.SalonServices.FirstAsync();
        context.Appointments.Add(new Appointment
        {
            CustomerId = customer.Id,
            Stylist = stylist,
            ServiceId = service.Id,
            AppointmentDate = DateTime.Today,
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10)
        });
        await context.SaveChangesAsync();
    }
    private static AIController StylistController(ApplicationDbContext context, string email)
    {
        var controller = new AIController(context, new StubAIService(new AITextResult(true, "Gợi ý test")), NullLogger<AIController>.Instance);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    [new Claim(ClaimTypes.Name, email), new Claim(ClaimTypes.Role, "Stylist")], "Test"))
            }
        };
        return controller;
    }
    private static AIRecommendationViewModel ModelFor(int customerId) => new()
    {
        CustomerId = customerId,
        InputText = "Nhu cầu test",
        HairCondition = "Tóc khỏe, bình thường",
        DesiredStyle = "Tóc gọn, dễ chăm sóc",
        MaintenancePreference = "Ít thời gian chăm sóc"
    };

    private sealed class StubAIService : IAIService
    {
        private readonly AITextResult _result;
        public StubAIService(AITextResult result) => _result = result;
        public Task<AITextResult> RecommendAsync(Customer customer, IReadOnlyCollection<ServiceHistory> history, IReadOnlyCollection<SalonService> services, string need, CancellationToken cancellationToken = default) => Task.FromResult(_result);
        public Task<AITextResult> ChatAsync(Customer customer, IReadOnlyCollection<ServiceHistory> history, IReadOnlyCollection<SalonService> services, IReadOnlyCollection<AIConversationTurn> conversation, string message, CancellationToken cancellationToken = default) => Task.FromResult(_result);
        public Task<AITextResult> GenerateMessageAsync(Customer customer, Appointment? appointment, MessageType type, CancellationToken cancellationToken = default) => Task.FromResult(_result);
        public Task<AITextResult> SummarizeHistoryAsync(Customer customer, IReadOnlyCollection<ServiceHistory> history, CancellationToken cancellationToken = default) => Task.FromResult(_result);
    }
    private sealed class ThrowingAIService : IAIService
    {
        public Task<AITextResult> RecommendAsync(Customer customer, IReadOnlyCollection<ServiceHistory> history, IReadOnlyCollection<SalonService> services, string need, CancellationToken cancellationToken = default) => throw new HttpRequestException("Test error");
        public Task<AITextResult> ChatAsync(Customer customer, IReadOnlyCollection<ServiceHistory> history, IReadOnlyCollection<SalonService> services, IReadOnlyCollection<AIConversationTurn> conversation, string message, CancellationToken cancellationToken = default) => throw new HttpRequestException("Test error");
        public Task<AITextResult> GenerateMessageAsync(Customer customer, Appointment? appointment, MessageType type, CancellationToken cancellationToken = default) => throw new HttpRequestException("Test error");
        public Task<AITextResult> SummarizeHistoryAsync(Customer customer, IReadOnlyCollection<ServiceHistory> history, CancellationToken cancellationToken = default) => throw new HttpRequestException("Test error");
    }
}
