using Microsoft.AspNetCore.Identity;
using MyFirstApp.Models;
using Microsoft.EntityFrameworkCore;
using MyFirstApp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPasswordHasher<Customer>, PasswordHasher<Customer>>();
#region Session Service Configuration
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
#endregion


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

#region Apply Database Migrations Automatically
await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}
#endregion

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

#region Session Middleware
app.UseSession();
#endregion

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "customer",
    pattern: "customer-info",
    defaults: new
    {
        controller = "Customer",
        action = "Index",
    });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
