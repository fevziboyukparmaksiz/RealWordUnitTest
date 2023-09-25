using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RealWordUnitTest.Web.Controllers;
using RealWordUnitTest.Web.Models;
using RealWordUnitTest.Web.Repository;

namespace RealWordUnitTest.Test
{
    public class ProductControllerTest
    {
        private readonly IMock<IRepository<Product>> _mockRepository;
        private readonly ProductsController _productsController;
        private List<Product> _products;
        public ProductControllerTest()
        {
            _mockRepository = new Mock<IRepository<Product>>();
            _productsController = new ProductsController(_mockRepository.Object);
            _products = new List<Product>()
            {
                new() { Id = 1, Name = "Kalem", Price = 100, Stock = 50, Color = "Kırmızı" } ,
                new() { Id = 2, Name = "Kitap", Price = 200, Stock = 40, Color = "Mavi" }
            };
        }

        [Fact]
        public async void Index_ActionExecution_ReturnView()
        {
            var result = await _productsController.Index();

            Assert.IsType<ViewResult>(result);
        }



    }
}
