using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RealWordUnitTest.Web.Controllers;
using RealWordUnitTest.Web.Models;
using RealWordUnitTest.Web.Repository;

namespace RealWordUnitTest.Test
{
    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepository;
        private readonly ProductsController _productsController;
        private readonly List<Product> _products;
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

        [Fact]
        public async void Index_ActionExecution_ReturnProductList()
        {
            _mockRepository.Setup(repo => repo.GetAll()).ReturnsAsync(_products);

            var result = await _productsController.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

            Assert.Equal<int>(2, productList.Count());
        }

        [Fact]
        public async void Details_IdisNull_ReturnRedirectToIndexAction()
        {
            var result = await _productsController.Details(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void Details_IdInvalid_ReturnNotFound()
        {
            Product product = null;
            _mockRepository.Setup(repo => repo.GetById(0))!.ReturnsAsync(product);

            var result = await _productsController.Details(0);

            var notFound = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, notFound.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public async void Details_ValidId_ReturnProduct(int productId)
        {
            var product = _products.Find(x => x.Id == productId);
            _mockRepository.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _productsController.Details(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        [Fact]
        public void Create_ActionExecution_ReturnView()
        {
            var result = _productsController.Create();

            Assert.IsType<ViewResult>(result);

        }

        [Fact]
        public async void CreatePOST_InvalidModelState_ReturnView()
        {
            _productsController.ModelState.AddModelError("Name", "Name alanı boş olamaz");

            var result = await _productsController.Create(_products.First());

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }

        [Fact]
        public async void CreatePOST_ValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await _productsController.Create(_products.First());

            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirectToAction.ActionName);
        }

        [Fact]
        public async void CreatePOST_ValidModelState_CreateMethodExecution()
        {
            Product newProduct = null;

            _mockRepository.Setup(repo => repo.Create(It.IsAny<Product>())).Callback<Product>(x => newProduct = x);

            var result = await _productsController.Create(_products.First());

            _mockRepository.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once);

            Assert.Equal(_products.First().Id, newProduct.Id);
        }

        [Fact]
        public async void CreatePOST_InvalidModelState_NeverCreateMethodExecution()
        {
            _productsController.ModelState.AddModelError("Name", "Name alanı boş olamaz");

            var result = await _productsController.Create(_products.First());

            _mockRepository.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Never);

        }

        [Fact]
        public async void Edit_IdisNull_ReturnRedirectToIndexAction()
        {
            var result = await _productsController.Edit(null);

            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirectToAction.ActionName);
        }

        [Fact]
        public async void Edit_IdisInvalid_ReturnNotFound()
        {
            Product product = null;

            _mockRepository.Setup(repo => repo.GetById(3))!.ReturnsAsync(product);

            var result = await _productsController.Edit(3);

            var notFound = Assert.IsType<NotFoundResult>(result);

            Assert.Equal(404, notFound.StatusCode);
        }

        [Theory]
        [InlineData(2)]
        public async void Edit_ValidId_ReturnView(int productId)
        {
            var product = _products.First(x => x.Id == productId);
            _mockRepository.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _productsController.Edit(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            var productResult = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, productResult.Id);
            Assert.Equal(product.Name, productResult.Name);

        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_IdisNotEqualProduct_ReturnNotFound(int productId)
        {
            var result = _productsController.Edit(2, _products.First(x => x.Id == productId));

            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_InvalidModelState_ReturnView(int productId)
        {
            _productsController.ModelState.AddModelError("", "");

            var result = _productsController.Edit(productId, _products.Find(x => x.Id == productId));

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_ValidModelState_ReturnRedirectToIndexAction(int productId)
        {
            var result = _productsController.Edit(productId, _products.Find(x => x.Id == productId));

            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirectToAction.ActionName);
        }

        [Theory]
        [InlineData(1)]
        public async void EditPOST_ValidModelState_EditMethodExecution(int productId)
        {
            var product = _products.First(x => x.Id == productId);

            _mockRepository.Setup(repo => repo.Update(product));

            _productsController.Edit(productId, product);

            _mockRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async void Delete_IdIsNull_ReturnNotFound()
        {
            var result = await _productsController.Delete(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(0)]
        public async void Delete_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepository.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _productsController.Delete(productId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecutes_ReturnProduct(int productId)
        {
            var product = _products.First(x => x.Id == productId);

            _mockRepository.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _productsController.Delete(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsAssignableFrom<Product>(viewResult.Model);
        }
    }
}
