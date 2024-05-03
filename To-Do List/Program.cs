using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using To_Do_List.Models;
using To_Do_List.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services
    .AddDbContext<TasksContext>(options => options.UseNpgsql(connection))
    .AddIdentity<User, IdentityRole<int>>(opts =>
    {
        opts.Password.RequiredLength = 5;
        opts.Password.RequireDigit = false;
        opts.Password.RequireLowercase = false;
        opts.Password.RequireUppercase = false;
        opts.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<TasksContext>();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

    await AdminInitializer.SeedAdminUser(roleManager, userManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error has occured while seeding the database");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=MyTask}/{action=Index}/{id?}");

app.Run();
