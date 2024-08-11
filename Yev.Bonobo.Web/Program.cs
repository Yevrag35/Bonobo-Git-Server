using AttributeDI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Yev.Bonobo.Database;
using Yev.Bonobo.Middleware;
using Yev.Bonobo.Startup;

var builder = AppStartup.CreateWebBuilder(args);

// Add services to the container.
builder.Services
    .AddAttributedServices(options =>
    {
        bool isDev = builder.Environment.IsDevelopment();
        options.AllowDuplicateServiceRegistrations = !isDev;
        options.Configuration = builder.Configuration;
        options.IgnoreMultipleDynamicRegistrations = !isDev;
        options.ThrowOnMissingDynamicRegistrationMethod = isDev;
    })
    .AddDbContext<GitDbContext>(options =>
    {
        string conString = builder.Configuration.GetConnectionString("SQLite") ?? string.Empty;
        conString = conString.Replace("|DataDirectory|", builder.Environment.ContentRootPath);

        options.EnableSensitiveDataLogging()
               .EnableDetailedErrors()
               .UseSqlite(conString);
    });

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services
    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetRequiredSection("AzureAD"));
    //.EnableTokenAcquisitionToCallDownstreamApi(["user.read"]);
    //.AddMicrosoftGraph(builder.Configuration.GetRequiredSection("AzureAD"))
    //.AddInMemoryTokenCaches(options =>
    //{
    //    options.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
    //});

builder.Services
    .AddAuthorizationBuilder()
        .AddDefaultPolicy("Readers", b =>
        {
            b.RequireAuthenticatedUser()
             .RequireClaim(ClaimConstants.Roles, "git.read");
        });

builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        bool isDev = builder.Environment.IsDevelopment();
        options.AllowInputFormatterExceptionMessages = isDev;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = isDev;
    });

builder.Services
    .AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();

builder.Services
    .AddRazorPages()
    .AddMicrosoftIdentityUI();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbCtx = scope.ServiceProvider.GetRequiredService<GitDbContext>();
    await dbCtx.Database.MigrateAsync(app.Lifetime.ApplicationStopping).ConfigureAwait(false);
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

app.UseAuthorization();

app.UseDbUserMiddleware();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
