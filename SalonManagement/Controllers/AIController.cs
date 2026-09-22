using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using SalonManagement.Data;
using SalonManagement.Models;
using SalonManagement.Services;
using SalonManagement.ViewModels;

namespace SalonManagement.Controllers;

[Authorize(Roles = "Admin,Receptionist,Stylist")]
public class AIController : Controller
{
    private const string ChatSessionKey = "SalonManagement.AI.ChatTurns";
    private readonly ApplicationDbContext _context;
    private readonly IAIService _aiService;
    private readonly ILogger<AIController> _logger;
    public AIController(ApplicationDbContext context, IAIService aiService, ILogger<AIController> logger) { _context = context; _aiService = aiService; _logger = logger; }

    public IActionResult Index() => View();
    public async Task<IActionResult> Recommendation(int? customerId)
    {
        var model = new AIRecommendationViewModel { CustomerId = customerId ?? 0 };
        await PopulateCustomersAsync(model); return View(model);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Recommendation(AIRecommendationViewModel model)
    {
        if (!ModelState.IsValid) { await PopulateCustomersAsync(model); return View(model); }
        var customer = await _context.Customers.FindAsync(model.CustomerId);
        if (customer is null) { ModelState.AddModelError(nameof(model.CustomerId), "Không tìm thấy khách hàng."); await PopulateCustomersAsync(model); return View(model); }
        try
        {
            var history = await HistoryAsync(customer.Id);
            var activeServices = await _context.SalonServices.Where(x => x.Status).ToListAsync();
            var profile = new SalonConsultationProfile
            {
                CustomerNeed = model.InputText,
                HairCondition = model.HairCondition,
                DesiredStyle = model.DesiredStyle,
                Budget = model.Budget,
                MaintenancePreference = model.MaintenancePreference
            };
            model.SuggestedServices = SalonConsultationAdvisor.Build(activeServices, history, profile)
                .Select(x => new AIServiceSuggestionViewModel
                {
                    Name = x.Service.Name,
                    Price = x.Service.Price,
                    DurationMinutes = x.Service.DurationMinutes,
                    Reason = x.Reason
                }).ToList();
            var result = await _aiService.RecommendAsync(customer, history, activeServices, profile.ToPromptContext(), model.PromptVersion);
            if (!result.Succeeded) ModelState.AddModelError(string.Empty, result.Error!);
            else { model.Result = result.Text; model.UsedFallback = result.UsedFallback; _context.AIRecommendations.Add(new AIRecommendation { CustomerId = customer.Id, InputText = model.InputText, Recommendation = result.Text }); await _context.SaveChangesAsync(); }
        }
        catch (Exception ex) { _logger.LogError(ex, "AI recommendation failed for customer {CustomerId}", model.CustomerId); ModelState.AddModelError(string.Empty, "AI hiện chưa sẵn sàng. Vui lòng thử lại sau."); }
        await PopulateCustomersAsync(model); return View(model);
    }
    public async Task<IActionResult> Chat(int? customerId)
    {
        var model = new AIChatViewModel
        {
            CustomerId = customerId ?? 0,
            Turns = GetChatTurns().Select(x => new AIChatTurnViewModel(x.Role, x.Text, x.UsedFallback)).ToList()
        };
        await PopulateCustomersAsync(model);
        return View(model);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Chat(AIChatViewModel model)
    {
        var turns = GetChatTurns();
        if (!ModelState.IsValid)
        {
            model.Turns = turns.Select(x => new AIChatTurnViewModel(x.Role, x.Text, x.UsedFallback)).ToList();
            await PopulateCustomersAsync(model);
            return View(model);
        }
        var customer = await _context.Customers.FindAsync(model.CustomerId);
        if (customer is null)
        {
            ModelState.AddModelError(nameof(model.CustomerId), "Không tìm thấy khách hàng.");
            model.Turns = turns.Select(x => new AIChatTurnViewModel(x.Role, x.Text, x.UsedFallback)).ToList();
            await PopulateCustomersAsync(model);
            return View(model);
        }
        try
        {
            var result = await _aiService.ChatAsync(customer, await HistoryAsync(customer.Id),
                await _context.SalonServices.Where(x => x.Status).ToListAsync(), turns, model.Message);
            if (!result.Succeeded) ModelState.AddModelError(string.Empty, result.Error!);
            else
            {
                turns.Add(new AIConversationTurn("Nhân viên", model.Message));
                turns.Add(new AIConversationTurn("AI", result.Text, result.UsedFallback));
                turns = turns.TakeLast(8).ToList();
                SaveChatTurns(turns);
                model.Message = string.Empty;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI chat failed for customer {CustomerId}", model.CustomerId);
            ModelState.AddModelError(string.Empty, "Không thể tư vấn AI lúc này. Vui lòng thử lại sau.");
        }
        model.Turns = turns.Select(x => new AIChatTurnViewModel(x.Role, x.Text, x.UsedFallback)).ToList();
        await PopulateCustomersAsync(model);
        return View(model);
    }
    public async Task<IActionResult> Message(int? customerId, int? appointmentId)
    {
        var model = new AIMessageViewModel { CustomerId = customerId ?? 0, AppointmentId = appointmentId };
        await PopulateMessageOptionsAsync(model); return View(model);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Message(AIMessageViewModel model)
    {
        if (!ModelState.IsValid) { await PopulateMessageOptionsAsync(model); return View(model); }
        var customer = await _context.Customers.FindAsync(model.CustomerId);
        if (customer is null) { ModelState.AddModelError(nameof(model.CustomerId), "Không tìm thấy khách hàng."); await PopulateMessageOptionsAsync(model); return View(model); }
        var appointment = model.AppointmentId.HasValue ? await _context.Appointments.Include(x => x.Service).Include(x => x.Stylist).FirstOrDefaultAsync(x => x.Id == model.AppointmentId && x.CustomerId == model.CustomerId) : null;
        try
        {
            var result = await _aiService.GenerateMessageAsync(customer, appointment, model.MessageType);
            if (!result.Succeeded) ModelState.AddModelError(string.Empty, result.Error!);
            else { model.Result = result.Text; _context.AIGeneratedMessages.Add(new AIGeneratedMessage { CustomerId = customer.Id, AppointmentId = model.AppointmentId, MessageType = model.MessageType, GeneratedMessage = result.Text }); await _context.SaveChangesAsync(); }
        }
        catch (Exception ex) { _logger.LogError(ex, "AI message generation failed for customer {CustomerId}", model.CustomerId); ModelState.AddModelError(string.Empty, "Không thể tạo tin nhắn lúc này."); }
        await PopulateMessageOptionsAsync(model); return View(model);
    }
    public async Task<IActionResult> Summary(int? customerId)
    {
        var model = new AISummaryViewModel { CustomerId = customerId ?? 0 };
        await PopulateCustomersAsync(model); return View(model);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Summary(AISummaryViewModel model)
    {
        if (!ModelState.IsValid) { await PopulateCustomersAsync(model); return View(model); }
        var customer = await _context.Customers.FindAsync(model.CustomerId);
        if (customer is null) { ModelState.AddModelError(nameof(model.CustomerId), "Không tìm thấy khách hàng."); await PopulateCustomersAsync(model); return View(model); }
        try
        {
            var result = await _aiService.SummarizeHistoryAsync(customer, await HistoryAsync(customer.Id));
            if (!result.Succeeded) ModelState.AddModelError(string.Empty, result.Error!); else model.Result = result.Text;
        }
        catch (Exception ex) { _logger.LogError(ex, "AI summary failed for customer {CustomerId}", model.CustomerId); ModelState.AddModelError(string.Empty, "Không thể tóm tắt lịch sử lúc này."); }
        await PopulateCustomersAsync(model); return View(model);
    }
    private async Task<List<ServiceHistory>> HistoryAsync(int customerId) => await _context.ServiceHistories.Include(x => x.Service).Include(x => x.Stylist).Where(x => x.CustomerId == customerId).OrderByDescending(x => x.ServiceDate).ToListAsync();
    private async Task PopulateCustomersAsync(AIRecommendationViewModel model) => model.Customers = (await _context.Customers.OrderBy(x => x.FullName).ToListAsync()).Select(x => new SelectListItem(x.FullName, x.Id.ToString()));
    private async Task PopulateCustomersAsync(AISummaryViewModel model) => model.Customers = (await _context.Customers.OrderBy(x => x.FullName).ToListAsync()).Select(x => new SelectListItem(x.FullName, x.Id.ToString()));
    private async Task PopulateCustomersAsync(AIChatViewModel model) => model.Customers = (await _context.Customers.OrderBy(x => x.FullName).ToListAsync()).Select(x => new SelectListItem(x.FullName, x.Id.ToString()));
    private async Task PopulateMessageOptionsAsync(AIMessageViewModel model)
    {
        model.Customers = (await _context.Customers.OrderBy(x => x.FullName).ToListAsync()).Select(x => new SelectListItem(x.FullName, x.Id.ToString()));
        model.Appointments = (await _context.Appointments.Include(x => x.Service).Where(x => !model.CustomerId.Equals(0) && x.CustomerId == model.CustomerId).OrderByDescending(x => x.AppointmentDate).ToListAsync()).Select(x => new SelectListItem($"{x.AppointmentDate:dd/MM/yyyy} - {x.Service!.Name}", x.Id.ToString()));
    }
    private List<AIConversationTurn> GetChatTurns()
    {
        var json = HttpContext.Session.GetString(ChatSessionKey);
        if (string.IsNullOrWhiteSpace(json)) return [];
        try { return JsonSerializer.Deserialize<List<AIConversationTurn>>(json) ?? []; }
        catch (JsonException) { return []; }
    }
    private void SaveChatTurns(List<AIConversationTurn> turns) => HttpContext.Session.SetString(ChatSessionKey, JsonSerializer.Serialize(turns));
}
