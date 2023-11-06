using BilgeShop.Business.Dtos;
using BilgeShop.Business.Services;
using BilgeShop.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BilgeShop.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _environment; // wwwroot yolunu yakalamak için kullanılacak olan metotları içeriyor.

        public ProductController(IProductService productService, ICategoryService categoryService, IWebHostEnvironment environment)
        {
            _productService = productService;
            _categoryService = categoryService;
            _environment = environment;

        }
        public IActionResult List()
        {

            var productDtoList = _productService.GetProducts();

            var viewModel = productDtoList.Select(x => new ProductListViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                UnitPrice = x.UnitPrice,
                UnitsInStock = x.UnitsInStock,
                ImagePath = x.ImagePath
            }).ToList();


            return View(viewModel);
        }

        [HttpGet]
        public IActionResult New()
        {
            ViewBag.Categories = _categoryService.GetCategories();
            return View("Form", new ProductFormViewModel());
            // EKLEME VE GÜNCELLEME AYNI FORM ÜZERİNDEN YAPILACAKSA , BOŞ MODEL İLE AÇMAYI UNUTMA - YOKSA FORMDAN ID -> Null GÖNDERİLİYOR SANIP VALIDATION'A TAKILIR.
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var updateProductDto = _productService.GetProductById(id);

            var viewModel = new ProductFormViewModel()
            {
                Id = updateProductDto.Id,
                Name = updateProductDto.Name,
                Description = updateProductDto.Description,
                UnitsInStock = updateProductDto.UnitsInStock,
                UnitPrice = updateProductDto.UnitPrice,
                CategoryId = updateProductDto.CategoryId
            };

            ViewBag.ImagePath = updateProductDto.ImagePath;

            ViewBag.Categories = _categoryService.GetCategories();
            return View("Form", viewModel);
        }


        [HttpPost]
        public IActionResult Save(ProductFormViewModel formData)
        {

            if(!ModelState.IsValid)
            {
                ViewBag.Categories = _categoryService.GetCategories();
                return View("Form", formData);
            }

            var newFileName = "";

            if(formData.File is not null) // dosya yüklenmek isteniyorsa
            {

                var allowedFileTypes = new string[] { "image/jpeg", "image/jpg", "image/png", "image/jfif" }; // izin verdiğim dosya tipleri.

                var allowedFileExtensions = new string[] { ".jpg", ".jpeg", ".png", ".jfif" }; // izin verdiğim dosya uzantıları.

                var fileContentType = formData.File.ContentType;
                // dosyanın içeriği.

                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(formData.File.FileName);
                // dosyanın uzantısız ismi.

                var fileExtension = Path.GetExtension(formData.File.FileName);
                // dosyanın uzantısı.

                // Dosya tipi istediklerimden biri değilse veya uzantı istediklerimden biri değilse
            if(!allowedFileTypes.Contains(fileContentType) ||
                    !allowedFileExtensions.Contains(fileExtension))
                {

                    // HATA MESAJI GÖSTER 
                    ViewBag.Categories = _categoryService.GetCategories();
                    return View("Form", formData);

                }


                newFileName = fileNameWithoutExtension + "-" + Guid.NewGuid() + fileExtension;
                // Aynı isimde iki dosya yüklendiğinde hata vermesin diye, birbiriyle asla eşleşmeyecek şekilde her dosya adına unique bir metin ilavesi (guid) yapılır.


                var folderPath = Path.Combine("images", "products");
                // images/products

                // _environment.WebRootPath -> wwwroot'a kadar olan kısım.

                var wwwrootFolderPath = Path.Combine(_environment.WebRootPath, folderPath);
                // .../wwwroot/images/products

                var wwwrootFilePath = Path.Combine(wwwrootFolderPath, newFileName);
                // .../wwwroot/images/products/urunGorseli-1231321daw.jpg

                Directory.CreateDirectory(wwwrootFolderPath); // Eğer images/products klasörleri yoksa, oluştur.

                using(var filestream = new FileStream(wwwrootFilePath, FileMode.Create))
                {
                    formData.File.CopyTo(filestream);
                } // asıl dosya kopyalamanın yapıldığı kısım.

                // using içerisinde new'lenen filestream nesnesi scope boyunca yaşar, scope bitiminde silinir.
            }


            if(formData.Id == 0 ) // EKLEME
            {
                var addProductDto = new AddProductDto()
                {
                    Name = formData.Name.Trim(),
                    Description = formData.Description,
                    UnitPrice = formData.UnitPrice,
                    UnitsInStock = formData.UnitsInStock,
                    CategoryId = formData.CategoryId,
                    ImagePath = newFileName

                };

                _productService.AddProduct(addProductDto);
                return RedirectToAction("List");

            }
            else // GÜNCELLEME
            {
                var updateProductDto = new UpdateProductDto()
                {
                    Id = formData.Id,
                    Name = formData.Name,
                    Description = formData.Description,
                    UnitPrice = formData.UnitPrice,
                    UnitsInStock = formData.UnitsInStock,
                    CategoryId = formData.CategoryId,
                    ImagePath = newFileName
                };

                _productService.UpdateProduct(updateProductDto);
                return RedirectToAction("List");




            }

        }

       
        public IActionResult Delete(int id)
        {
            _productService.DeleteProduct(id);

            return RedirectToAction("List");
        }
    }
}
