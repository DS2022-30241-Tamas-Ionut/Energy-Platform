using Assignment1.Business;
using Assignment1.Business.Interfaces;
using Assignment1.Models;
using Assignment1.Repository;
using Assignment1.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
  .AddRoles<IdentityRole>()
  .AddEntityFrameworkStores<EnergyUtilityDbContext>();

builder.Services.AddScoped<IGenericRepository, GenericRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IEnergyConsumptionService, EnergyConsumptionService>();
builder.Services.AddSingleton<IReceiveMessageService, ReceiveMessageService>();

builder.Services.AddDbContext<EnergyUtilityDbContext>(
        options => options.UseSqlServer(builder.Configuration.GetConnectionString("EnergyUtility"), sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure();
        }));

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/";
    options.AccessDeniedPath = "/";
    options.SlidingExpiration = true;
});

var app = builder.Build();

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
app.UseWebSockets();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

var services = app.Services.CreateScope().ServiceProvider;

CreateUserRoles(services).Wait();
CreateStartupUsers(services);

var receiveMessages = services.GetService<IReceiveMessageService>();
receiveMessages.ReceiveMessage();

app.Run();

async Task CreateUserRoles(IServiceProvider serviceProvider)
{
    var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    //Adding Admin Role
    var roleCheck = await RoleManager.RoleExistsAsync("Admin");
    if (!roleCheck)
    {
        //create the roles and seed them to the database
        await RoleManager.CreateAsync(new IdentityRole("Admin"));
    }

    roleCheck = await RoleManager.RoleExistsAsync("User");
    if (!roleCheck)
    {
        //create the roles and seed them to the database
        await RoleManager.CreateAsync(new IdentityRole("User"));
    }
}

void CreateStartupUsers(IServiceProvider serviceProvider)
{
    var userMgr = serviceProvider.GetRequiredService<UserManager<User>>();
    var users = userMgr.Users;
    if (!users.Any(x => x.UserName == "admin@webdotnet.com"))
    {
        var user = new User { UserName = "admin@webdotnet.com", Address = "Zorilor", FirstName="admin", LastName="admin" };
        userMgr.CreateAsync(user, "P@ssw0rd").Wait();
        var registeredUser = userMgr.FindByNameAsync(user.UserName).Result;
        userMgr.AddToRoleAsync(registeredUser, "Admin").Wait();
    }

    if (!users.Any(x => x.UserName == "none"))
    {
        var user = new User { UserName = "none", Address = "", FirstName = "", LastName = "" };
        userMgr.CreateAsync(user, "P@ssw0rd").Wait();
        var registeredUser = userMgr.FindByNameAsync(user.UserName).Result;
        userMgr.AddToRoleAsync(registeredUser, "User").Wait();
    }
}