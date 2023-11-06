﻿using BilgeShop.Business.Dtos;
using BilgeShop.Business.Services;
using BilgeShop.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BilgeShop.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IActionResult List()
        {
            var categoryDtoList = _categoryService.GetCategories();

            var viewModel = categoryDtoList.Select(x => new CategoryListViewModel()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return View(viewModel);
        }

        [HttpGet] // link/url'den tetiklenir.
        public IActionResult New()
        {
            return View("Form", new CategoryFormViewModel());
            // Eğer ekleme ve güncelleme için aynı formu kullanacaksanız. Formu ekleme kısmında boş bir model ile açın.
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
           var categoryDto =  _categoryService.GetCategory(id);

            var viewModel = new CategoryFormViewModel()
            {
                Id = categoryDto.Id,
                Name = categoryDto.Name,
                Description = categoryDto.Description
            };

            return View("Form", viewModel);
        }


        [HttpPost] // form'dan tetiklenir.
        public IActionResult Save(CategoryFormViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View("Form", formData);
            }

            if(formData.Id == 0) // EKLEME İŞLEMİ
            {
                var addCategoryDto = new AddCategoryDto()
                {
                    Name = formData.Name.Trim(),
                    Description = formData.Description
                };

              var result =  _categoryService.AddCategory(addCategoryDto);

                if (result)
                {
                    return RedirectToAction("List");
                }
                else
                {
                    ViewBag.ErrorMessage = "Bu isimde bir kategori zaten mevcut.";
                    return View("Form", formData);
                }
                
            }
            else // GÜNCELLEME İŞLEMİ
            {

                var updateCategoryDto = new UpdateCategoryDto()
                {
                    Id = formData.Id,
                    Name = formData.Name,
                    Description = formData.Description
                };

                _categoryService.UpdateCategory(updateCategoryDto);


                return RedirectToAction("List");
            }


        }



        public IActionResult Delete(int id)
        {
            _categoryService.DeleteCategory(id);

            return RedirectToAction("List");
        }
    }
}
