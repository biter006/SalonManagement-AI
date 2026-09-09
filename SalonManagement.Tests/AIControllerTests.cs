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
        var result = await controller.Recommendation(new AIRecommendationViewModel { CustomerId = customer.Id, InputText = "Nhu cầu test" });
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
        await controller.Recommendation(new AIRecommendationViewModel { CustomerId = customer.Id, InputText = "Nhu cầu test" });
        Assert.Empty(context.AIRecommendations);
    }

    [Fact]
    public async Task Recommendation_AIException_IsHandledWithoutDatabaseChange()
    {
        await using var context = Context();
        var customer = await AddCustomerAsync(context);
        var controller = new AIController(context, new ThrowingAIService(), NullLogger<AIController>.Instance);
        var result = await controller.Recommendation(new AIRecommendationViewModel { CustomerId = customer.Id, InputText = "Nhu cầu test" });
        var view = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ViewData.ModelState.IsValid);
        Assert.Empty(context.AIRecommendations);
    }

    private static ApplicationDbContext Context() => new(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
    private static async Task<Customer> AddCustomerAsync(ApplicationDbContext context)
    {
        var customer = new Customer { FullName = "AI Test", Phone = "0900555555" };
        context.Customers.Add(customer); context.SalonServices.Add(new SalonService { Name = "Cắt test", Price = 100000, DurationMinutes = 60 }); await context.SaveChangesAsync(); return customer;
    }

    private sealed class StubAIService : IAIService
    {
        private readonly AITextResult _result;
        public StubAIService(AITextResult result) => _result = result;
        public Task<AITextResult> RecommendAsync(Customer customer, IReadOnlyCollection<ServiceHistory> history, IReadOnlyCollection<SalonService> services, string need, CancellationToken cancellationToken = default) => Task.FromResult(_result);
        public Task<AITextResult> GenerateMessageAsync(Customer customer, Appointment? appointment, MessageType type, CancellationToken cancellationToken = default) => Task.FromResult(_result);
        public Task<AITextResult> SummarizeHistoryAsync(Customer customer, IReadOnlyCollection<ServiceHistory> history, CancellationToken cancellationToken = default) => Task.FromResult(_result);
    }
    private sealed class ThrowingAIService : IAIService
    {
        public Task<AITextResult> RecommendAsync(Customer customer, IReadOnlyCollection<ServiceHistory> history, IReadOnlyCollection<SalonService> services, string need, CancellationToken cancellationToken = default) => throw new HttpRequestException("Test error");
        public Task<AITextResult> GenerateMessageAsync(Customer customer, Appointment? appointment, MessageType type, CancellationToken cancellationToken = default) => throw new HttpRequestException("Test error");
        public Task<AITextResult> SummarizeHistoryAsync(Customer customer, IReadOnlyCollection<ServiceHistory> history, CancellationToken cancellationToken = default) => throw new HttpRequestException("Test error");
    }
}
