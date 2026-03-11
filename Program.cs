using Microsoft.EntityFrameworkCore;
using GoldPrice.Data;
using GoldPrice.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Database Context
builder.Services.AddDbContext<GoldPriceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GoldPriceDb")));

// Add HTTP Client for GoldAPI
builder.Services.AddHttpClient<IGoldApiService, GoldApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Gold}/{action=Index}/{id?}");

app.Run();
