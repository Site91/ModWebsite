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
using Mod.Util;

using Microsoft.AspNetCore.Http;
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using React.AspNet;

var builder = WebApplication.CreateBuilder(args);

//print tunnel
Console.WriteLine($"Tunnel URL: {Environment.
            GetEnvironmentVariable("VS_TUNNEL_URL")}");
Console.WriteLine($"API project tunnel URL: {Environment.
    GetEnvironmentVariable("VS_TUNNEL_URL_MyWebApi")}");

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
builder.Services.AddScoped<WebSocketUtil>();
//builder.Services.AddSingleton<IEmailSender, EmailSender>();

//REACT
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddReact();

// Make sure a JS engine is registered, or you will get an error!
builder.Services.AddJsEngineSwitcher(options => options.DefaultEngineName = ChakraCoreJsEngine.EngineName)
  .AddChakraCore();

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
    app.UseHttpsRedirection();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");

SeedDatabase();

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

//REACT
app.UseReact(config =>
{
    // If you want to use server-side rendering of React components,
    // add all the necessary JavaScript files here. This includes
    // your components as well as all of their dependencies.
    // See http://reactjs.net/ for more information. Example:
    //config
    //    .AddScript("~/js/First.jsx")
    //    .AddScript("~/js/Second.jsx");

    // If you use an external build too (for example, Babel, Webpack,
    // Browserify or Gulp), you can improve performance by disabling
    // ReactJS.NET's version of Babel and loading the pre-transpiled
    // scripts. Example:
    //config
    //    .SetLoadBabel(false)
    //    .AddScriptWithoutTransform("~/Scripts/bundle.server.js");
});

app.UseStaticFiles();
app.UseWebSockets(webSocketOptions);

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
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}
