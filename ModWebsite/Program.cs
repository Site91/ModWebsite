using Mod.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using System.Text.Json.Serialization;
using Mod.DataAccess.Repository.IRepository;
using Mod.DataAccess.Repository;
using Mod.DataAccess.DbInitializer;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"), options => options.EnableRetryOnFailure()
        )
    );

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false).AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<IdentityDbContext, ApplicationDbContext>(); //Added to try fix issues
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
//builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddMvc(); //Added to try fixes
builder.Services.AddFeatureManagement();
builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
});
builder.Services.AddAuthorization(options =>
{
    ////options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("EmployeeNumber"));
    //options.AddPolicy(SD.Roles[SD.RoleEnum.Manager][0], policy => policy.RequireRole(SD.Role_Manager));
    //options.AddPolicy(SD.Roles[SD.RoleEnum.Admin][0], policy => policy.RequireRole(new string[] { SD.Role_Manager, SD.Role_Admin }));
    //options.AddPolicy(SD.Roles[SD.RoleEnum.Mod][0], policy => policy.RequireRole(new string[] { SD.Role_Manager, SD.Role_Admin, SD.Role_Mod }));
    //options.AddPolicy(SD.Roles[SD.RoleEnum.JrMod][0], policy => policy.RequireRole(new string[] { SD.Role_Manager, SD.Role_Admin, SD.Role_Mod, SD.Role_JrMod }));
    //options.AddPolicy(SD.Roles[SD.RoleEnum.User_Dev][0], policy => policy.RequireRole(new string[] { SD.Role_Manager, SD.Role_Admin, SD.Role_Mod, SD.Role_JrMod, SD.Role_User_Dev }));
    //options.AddPolicy(SD.Roles[SD.RoleEnum.User_Indi][0], policy => policy.RequireRole(new string[] { SD.Role_Manager, SD.Role_Admin, SD.Role_Mod, SD.Role_JrMod, SD.Role_User_Dev, SD.Role_User_Indi }));
    //options.AddPolicy(SD.Roles[SD.RoleEnum.Trusted][0], policy => policy.RequireRole(new string[] { SD.Role_Trusted }));
});
builder.Services.AddCertificateForwarding(options =>
{
    
});
builder.Services.ConfigureApplicationCookie(options => options.LoginPath = "/Identity/Account/LogIn");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");

SeedDatabase();

//var webSocketOptions = new WebSocketOptions
//{
//    KeepAliveInterval = TimeSpan.FromMinutes(2)
//};

app.UseHttpsRedirection();
app.UseStaticFiles();
//app.UseWebSockets(webSocketOptions);

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Home}/{controller=Home}/{action=Index}/{id?}");

app.Run();

void SeedDatabase()
{
    //using (var scope = app.Services.CreateScope())
    //{
    //    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    //    dbInitializer.Initialize();
    //}
}
