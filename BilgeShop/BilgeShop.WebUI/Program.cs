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
// IRepository tipinde bir new'leme yap�ld���nda (Dependency Injection ile) - SqlRepository kopyas� olu�tur.
// AddScoped -> Her istek i�in yeni bir kopya.

builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();
builder.Services.AddScoped<IProductService, ProductManager>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = new PathString("/");
    options.LogoutPath = new PathString("/");
    options.AccessDeniedPath = new PathString("/");
    // giri� - ��k�� - eri�im reddi durumlar�nda ana sayfaya y�nlendiriyor.
});


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
// Auth i�lemleri yap�yorsan, �stteki 2 sat�r yaz�lmal�. Yoksa hata vermez fakat oturum a�amaz, yetkilendirme sorgulayamaz.

// area route - default route'un �ncesinde yaz�lmal�.

app.UseStaticFiles(); // wwwroot kullan�m� i�in.

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{Controller=Dashboard}/{Action=Index}/{id?}"
    );

app.MapDefaultControllerRoute();


app.Run();
