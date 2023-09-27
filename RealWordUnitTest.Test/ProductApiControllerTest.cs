using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RealWordUnitTest.Web.Controllers;
using RealWordUnitTest.Web.Models;
using RealWordUnitTest.Web.Repository;

namespace RealWordUnitTest.Test
{
    public class ProductApiControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepository;
        private readonly ProductsApiController _productsApiController;
        private List<Product> _products;

        public ProductApiControllerTest()
        {
            _mockRepository = new Mock<IRepository<Product>>();
            _productsApiController = new ProductsApiController(_mockRepository.Object);
            _products = new List<Product>() { new Product { Id = 1, Name = "Kalem", Price = 100, Stock = 50, Color = "Kırmızı" },
                new Product { Id = 2, Name = "Defter", Price = 200, Stock = 500, Color = "Mavi" }};
        }

        [Fact]
        public async void GetProduct_ActionExecutes_ReturnOkResultWithProduct()
        {
            _mockRepository.Setup(repo => repo.GetAll()).ReturnsAsync(_products);

            var result = await _productsApiController.GetProducts();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);

            Assert.Equal(2, returnProducts.ToList().Count);
        }

        [Theory]
        [InlineData(0)]
        public async void GetProduct_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepository.Setup(repo => repo.GetById(productId))!.ReturnsAsync(product);

            var result = await _productsApiController.GetProduct(productId);

            Assert.IsType<NotFoundResult>(result);

        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void GetProduct_IdValid_ReturnOkResult(int productId)
        {
            var product = _products.First(x => x.Id == productId);

            _mockRepository.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _productsApiController.GetProduct(productId);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnProduct = Assert.IsType<Product>(okResult.Value);

            Assert.Equal(productId, returnProduct.Id);
            Assert.Equal(product.Name, returnProduct.Name);

        }

        [Theory]
        [InlineData(1)]
        public void PutProduct_IdIsNotEqualProduct_ReturnBadRequestResult(int productId)
        {
            var product = _products.First(x => x.Id == productId);

            var result = _productsApiController.PutProduct(2, product);

            Assert.IsType<BadRequestResult>(result);

        }

        [Theory]
        [InlineData(1)]
        public void PutProduct_ActionExecutes_ReturnNoContent(int productId)
        {
            var product = _products.First(x => x.Id == productId);
            _mockRepository.Setup(repo => repo.Update(product));

            var result = _productsApiController.PutProduct(productId, product);

            _mockRepository.Verify(repo => repo.Update(product), Times.Once);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async void PostProduct_ActionExecutes_ReturnCreatedAtAction()
        {
            var product = _products.First();

            _mockRepository.Setup(repo => repo.Create(product)).Returns(Task.CompletedTask);

            var result = await _productsApiController.PostProduct(product);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);

            _mockRepository.Verify(repo => repo.Create(product), Times.Once);

            Assert.Equal("GetProduct",createdAtActionResult.ActionName);
        }
    }
}
