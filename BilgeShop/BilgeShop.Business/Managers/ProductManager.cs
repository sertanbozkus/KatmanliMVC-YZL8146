using BilgeShop.Business.Dtos;
using BilgeShop.Business.Services;
using BilgeShop.Data.Entities;
using BilgeShop.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilgeShop.Business.Managers
{
    public class ProductManager : IProductService
    {
        private readonly IRepository<ProductEntity> _productRepository;
        public ProductManager(IRepository<ProductEntity> productRepository)
        {
            _productRepository = productRepository;
        }

        public void AddProduct(AddProductDto addProductDto)
        {
            var entity = new ProductEntity()
            {
                Name = addProductDto.Name,
                Description = addProductDto.Description,
                UnitPrice = addProductDto.UnitPrice,
                UnitsInStock = addProductDto.UnitsInStock,
                CategoryId = addProductDto.CategoryId,
                ImagePath = addProductDto.ImagePath
            };

            _productRepository.Add(entity);
        }

        public void DeleteProduct(int id)
        {
            _productRepository.Delete(id);
        }

        public UpdateProductDto GetProductById(int id)
        {
            var entity = _productRepository.GetById(id);

            var updateProductDto = new UpdateProductDto()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                UnitsInStock = entity.UnitsInStock,
                UnitPrice = entity.UnitPrice,
                CategoryId = entity.CategoryId,
                ImagePath = entity.ImagePath
            };

            return updateProductDto;
        }

        public List<ListProductDto> GetProducts()
        {
            var productEntities = _productRepository.GetAll().OrderBy(x => x.Category.Name).ThenBy(x => x.Name);
            // Önce kategori adına sonra ürün adına göre sırala

            var productDtoList = productEntities.Select(x => new ListProductDto()
            {
                Id = x.Id,
                Name = x.Name,
                UnitPrice = x.UnitPrice,
                UnitsInStock = x.UnitsInStock,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name,
                ImagePath = x.ImagePath
            }).ToList();

            return productDtoList;
        }

        public List<ListProductDto> GetProductsByCategoryId(int? categoryId)
        {
            if(categoryId.HasValue) // is not null
            {
                var productEntities = _productRepository.GetAll(x => x.CategoryId == categoryId).OrderBy(x => x.Name);
                // gönderdiğim categoryId ile categoryId verisi eşleşenleri isimlerine göre sıralayarak getir.

                var productDtos = productEntities.Select(x => new ListProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UnitPrice = x.UnitPrice,
                    UnitsInStock = x.UnitsInStock,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name,
                    ImagePath = x.ImagePath
                }).ToList();

                return productDtos;

            }
            else
            {
                return GetProducts(); // CategoryId gönderilmez ise yapılacak işlemler GetProducts ile aynı olduğundan direkt o metoda yönlendiriyorum.
            }


        }

        public void UpdateProduct(UpdateProductDto updateProductDto)
        {
            var entity = _productRepository.GetById(updateProductDto.Id);
            // Id ile eşleşen nesnenin tamamını çekiyorum.

            entity.Name = updateProductDto.Name;
            entity.Description = updateProductDto.Description;
            entity.UnitPrice = updateProductDto.UnitPrice;
            entity.UnitsInStock = updateProductDto.UnitsInStock;
            entity.CategoryId = updateProductDto.CategoryId;

            if(updateProductDto.ImagePath != "")
            entity.ImagePath = updateProductDto.ImagePath;
            // Bu If'i yazmazsam, updateProductDto ile View'den gelen null olan ImagePath bilgisi, veritabanındaki görsel adresinin üzerine yazılır. Böylelikle elimizde olan görseli kaybetmiş oluruz. ISTERSEN IF'I YORUM SATIRI YAPIP BIR DENE !

            _productRepository.Update(entity);
        }


        

    }
}
