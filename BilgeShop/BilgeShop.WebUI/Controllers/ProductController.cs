using Microsoft.AspNetCore.Mvc;

namespace BilgeShop.WebUI.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Detail(int id)
        {
            return View();
        }
    }
}
