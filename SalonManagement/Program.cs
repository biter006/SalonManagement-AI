using Microsoft.AspNetCore.Identity;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SalonManagement.Data;
using SalonManagement.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("Prompts/SalonPrompts.json", optional: false, reloadOnChange: true);
var vietnameseCulture = new CultureInfo("vi-VN");
CultureInfo.DefaultThreadCurrentCulture = vietnameseCulture;
CultureInfo.DefaultThreadCurrentUICulture = vietnameseCulture;
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.Configure<AIOptions>(builder.Configuration.GetSection("AI"));
builder.Services.Configure<SalonPromptsOptions>(builder.Configuration.GetSection("SalonPrompts"));
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();
if (app.Environment.IsDevelopment()) app.UseMigrationsEndPoint();
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(vietnameseCulture),
    SupportedCultures = new List<CultureInfo> { vietnameseCulture },
    SupportedUICultures = new List<CultureInfo> { vietnameseCulture }
});
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
try
{
    await DbInitializer.SeedAsync(app.Services, builder.Configuration, app.Logger);
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Could not initialize salon database. Check the connection string and migrations.");
}
app.Run();

public partial class Program { }
