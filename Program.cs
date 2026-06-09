using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WardSync.Components;
using WardSync.Components.Account;
using WardSync.Data;
using WardSync.Services;

var builder = WebApplication.CreateBuilder(args);
// ==================================================
// APPLICATION STARTUP CONFIGURATION
// Configures services and dependency injection
// for the WardSync application.
// ==================================================
// Add services to the container.

// Register Razor Components and enable interactive
// server-side rendering for Blazor components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure authentication and authorization services
builder.Services.AddCascadingAuthenticationState();
// Handles redirects during authentication workflows
builder.Services.AddScoped<IdentityRedirectManager>();
// Revalidates user authentication state throughout the session
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddScoped<WardSyncService>();
builder.Services.AddScoped<CalendarExportService>();

var app = builder.Build();

// Configure HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. 
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Added additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

await IdentitySeedData.SeedRolesAndUsersAsync(app.Services);

app.MapGet("/assignments/calendar/{id:int}", async (
    int id,
    WardSyncService wardSyncService,
    CalendarExportService calendarService) =>
{
    var assignment = await wardSyncService.GetAssignmentByIdAsync(id);

    if (assignment is null)
    {
        return Results.NotFound();
    }

    var calendarContent =
        calendarService.CreateAssignmentCalendarFile(assignment);

    var fileName =
        $"WardSync-Assignment-{assignment.Id}.ics";

    return Results.File(
        System.Text.Encoding.UTF8.GetBytes(calendarContent),
        "text/calendar",
        fileName);
});

app.Run();
