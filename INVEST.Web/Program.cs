using INVEST.Application;
using INVEST.Application.Acoes.Abstractions;
using INVEST.Infrastructure;
using INVEST.Infrastructure.Integrations.CompanyLogos;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var cs = builder.Configuration.GetConnectionString("DB_INVEST");

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddExceptionHandler<INVEST.Web.Middlewares.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHttpClient<CompanyLogoFunctionClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Azure:CompanyLogo:BaseUrl"]!);
    client.DefaultRequestHeaders.Add("x-functions-key",
        builder.Configuration["Azure:CompanyLogo:FunctionKey"]!);
});

builder.Services.AddTransient<ICompanyLogoProvider>(sp =>
{
    var inner = sp.GetRequiredService<CompanyLogoFunctionClient>();
    var cache = sp.GetRequiredService<IMemoryCache>();
    return new CachedCompanyLogoProvider(inner, cache);
});

builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");

// aplicar migrations apenas em Dev
if (app.Environment.IsDevelopment())
{
    app.Services.ApplyMigrations();
}

app.Services.SeedDatabase();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();