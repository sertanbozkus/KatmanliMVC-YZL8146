using BilgeShop.Business.Dtos;
using BilgeShop.Business.Services;
using BilgeShop.WebUI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace BilgeShop.WebUI.Controllers
{
    // Authentication - Authorization
    // (Kimlik Doğrulama - Yetkilendirme)
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("KayitOl")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("KayitOl")]
        public IActionResult Register(RegisterViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View(formData);
                // formData'yı parametre olarak geri göndererek, formda doldurulmuş yerleri sıfırlamamayı sağlıyoruz.
            }

            var addUserDto = new AddUserDto()
            {
                Email = formData.Email.Trim(),
                FirstName = formData.FirstName.Trim(),
                LastName = formData.LastName.Trim(),
                Password = formData.Password.Trim()
            };

            var result = _userService.AddUser(addUserDto);

            if (result.IsSucceed)
            {
                // işlem başarılı, ana sayfaya yönlendir.
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // işlem başarısızsa, Form View'e geri dön.
                ViewBag.ErrorMessage = result.Message;
                return View(formData);
            }
           
        }

        // Await keyword kullanılıyorsa -> Yani action içerisinde asenkron bir işlem yapılacaksa, ilgili acttion async Task<..> tanımlanmalıdır.
        public async Task<IActionResult> login(LoginViewModel formData)
        {

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }

            var loginDto = new LoginDto
            {
                Email = formData.Email,
                Password = formData.Password
            };

            var userInfo = _userService.LoginUser(loginDto);

            if(userInfo is null)
            {
                return RedirectToAction("Index", "Home");
                // İsteğe bağlı, uyarı mesajı verilebilir.
            }

            // Buraya kadar gelebildiyse kodlar, demek ki kişinin formdan gönderdiği email ve şifre ile DB üzerindeki kayıt eşleşmiş. Gerekli bilgileri alıp veritabanından buraya kadar getirmişiz (userInfo içerisinde). Artık oturum açılabilir.

            // Oturumda tutacağım her veri -> Claim
            // Bütün verilerin listesi -> List<Claim>

            var claims = new List<Claim>();

            claims.Add(new Claim("id", userInfo.Id.ToString()));
            claims.Add(new Claim("email", userInfo.Email));
            claims.Add(new Claim("firstName", userInfo.FirstName));
            claims.Add(new Claim("lastName", userInfo.LastName));
            claims.Add(new Claim("userType", userInfo.UserType.ToString()));

            // Yetkilendirme işlemi için özel olarak bir claim daha açmam gerekiyor.

            claims.Add(new Claim(ClaimTypes.Role, userInfo.UserType.ToString()));

            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            // Claims listesindeki verilerle bir oturum açılacağı için yukarıdaki identity nesnesini tanımladım.

            var autProperties = new AuthenticationProperties
            {
                AllowRefresh = true, // yenilenebilir oturum.
                ExpiresUtc = new DateTimeOffset(DateTime.Now.AddHours(48)) // oturum açıldıktan sonra 48 saat geçerli.
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity), autProperties);
            // await asenkronize (eşzamansız) yapılan birbirlerini bekleyerek çalışmalarını sağlıyor. ( Proje - Browser )

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(); // Oturumu kapatır.

            return RedirectToAction("Index", "Home");
        }
    }
}