using BilgeShop.Business.Managers;
using BilgeShop.Business.Services;
using BilgeShop.Data.Context;
using BilgeShop.Data.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<BilgeShopContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped(typeof(IRepository<>), typeof(SqlRepository<>));
// IRepository tipinde bir new'leme yapýldýðýnda (Dependency Injection ile) - SqlRepository kopyasý oluþtur.
// AddScoped -> Her istek için yeni bir kopya.

builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();
builder.Services.AddScoped<IProductService, ProductManager>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = new PathString("/");
    options.LogoutPath = new PathString("/");
    options.AccessDeniedPath = new PathString("/");
    // giriþ - çýkýþ - eriþim reddi durumlarýnda ana sayfaya yönlendiriyor.
});


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
// Auth iþlemleri yapýyorsan, üstteki 2 satýr yazýlmalý. Yoksa hata vermez fakat oturum açamaz, yetkilendirme sorgulayamaz.

// area route - default route'un öncesinde yazýlmalý.

app.UseStaticFiles(); // wwwroot kullanýmý için.

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{Controller=Dashboard}/{Action=Index}/{id?}"
    );

app.MapDefaultControllerRoute();


app.Run();
