using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalonManagement.Controllers;
using SalonManagement.Data;
using SalonManagement.ViewModels;

namespace SalonManagement.Tests;

public class UsersControllerTests
{
    [Fact]
    public async Task Index_LoadsUsersAndDisplaysTheirRoles()
    {
        await using var provider = CreateProvider();
        using var scope = provider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        Assert.True((await roleManager.CreateAsync(new IdentityRole(DbInitializer.AdminRole))).Succeeded);
        var user = new IdentityUser { UserName = "admin.test@salon.local", Email = "admin.test@salon.local" };
        Assert.True((await userManager.CreateAsync(user, "Salon@123")).Succeeded);
        Assert.True((await userManager.AddToRoleAsync(user, DbInitializer.AdminRole)).Succeeded);

        var controller = new UsersController(userManager, roleManager);
        var result = await controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<UserRowViewModel>>(view.Model);
        var row = Assert.Single(model);
        Assert.Equal("admin.test@salon.local", row.Email);
        Assert.Equal("Quản trị viên", row.Role);
    }

    private static ServiceProvider CreateProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase($"UsersControllerTests-{Guid.NewGuid()}"));
        services.AddIdentityCore<IdentityUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        return services.BuildServiceProvider();
    }
}
