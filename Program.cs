using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResearchManagementSystem.Data;
using ResearchManagementSystem.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ResearchManagementSystem.Services;
using RemcSys.Data;
using RemcSys.Models;
using rscSys_final.Data;
using ResearchManagementSystem.Areas.CreSys.Data;
using ResearchManagementSystem.Areas.CreSys.Interfaces;
using ResearchManagementSystem.Areas.CreSys.Services;
using CrdlSys.Data;
using CrdlSys.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<RemcDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RemcDBContext") ?? throw new InvalidOperationException("Connection string 'RemcDBContext' not found.")));
builder.Services.AddDbContext<rscSysfinalDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("rscSysfinalDbContextConnection") ?? throw new InvalidOperationException("Connection string 'rscSysfinalDbContextConnection' not found.")));
builder.Services.AddDbContext<CreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CreDbContextConnection") ?? throw new InvalidOperationException("Connection string 'CreDbContextConnection' not found.")));
builder.Services.AddDbContext<CrdlDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CrdlDbContextConnection") ?? throw new InvalidOperationException("Connection string 'CrdlDbContextConnection' not found.")));
// Configure services
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                     ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//Dependencies Injection CRE
builder.Services.AddScoped<IEthicsEmailService, EthicsEmailService>();
builder.Services.AddScoped<IAllServices, AllServices>();
builder.Services.AddScoped<IPdfGenerationService, PdfGenerationService>();
builder.Services.AddHostedService<EvaluationReminderService>();


// Configure Identity services with unique email requirement
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultUI()
.AddDefaultTokenProviders();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ActionLoggerService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ReportService>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set timeout duration
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Enable session
app.UseSession();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Application start - seeding roles and SuperAdmin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Seed roles
        await ContextSeed.SeedRolesAsync(userManager, roleManager);
        // Seed SuperAdmin
        await ContextSeed.SeedSuperAdminAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<RemcDBContext>();

    var existingSettings = dbContext.REMC_Settings.FirstOrDefault(s => s.Id == "MainOption");
    if (existingSettings == null)
    {
        var settings = new Settings
        {
            Id = "MainOption",
            isMaintenance = false,
            isUFRApplication = false,
            isEFRApplication = false,
            isUFRLApplication = false,
            evaluatorNum = 5,
            daysEvaluation = 7,
            tuklasInvolvement = "Project Leader with Co-Proponents",
            lathalaInvolvement = "Project Leader with Co-Proponents"
        };

        dbContext.REMC_Settings.Add(settings);
        dbContext.SaveChanges();
    }
}

app.Run();
