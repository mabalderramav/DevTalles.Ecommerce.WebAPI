using AutoMapper;
using DevTalles.Ecommerce.WebAPI.Constants;
using DevTalles.Ecommerce.WebAPI.Models;
using DevTalles.Ecommerce.WebAPI.Models.Dtos.Products;
using DevTalles.Ecommerce.WebAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace DevTalles.Ecommerce.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors(PolicyName.AllowSpecificOrigin)]
    [Authorize(Roles = "Admin")]
    public class ProductsController(
        IProductRepository productRepository,
        IMapper mapper,
        ICategoryRepository categoryRepository) : ControllerBase
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetProducts()
        {
            var products = _productRepository.GetProducts();
            var productsDto = _mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }

        [HttpGet("{productId:int}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetProduct(int productId)
        {
            var product = _productRepository.GetProduct(productId);
            if (product == null) return NotFound($"Product with ID {productId} not found.");
            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateProduct([FromBody] CreateProductDto? createProductDto)
        {
            if (createProductDto == null) return BadRequest(ModelState);
            if (_productRepository.ProductExists(createProductDto.Name))
            {
                ModelState.AddModelError("CustomError",
                    $"Product with name {createProductDto.Name} already exists.");
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.CategoryExists(createProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError",
                    $"Category with ID {createProductDto.CategoryId} does not exist.");
                return BadRequest(ModelState);
            }

            var product = _mapper.Map<Product>(createProductDto);
            if (_productRepository.CreateProduct(product))
            {
                var createProduct = _productRepository.GetProduct(product.ProductId);
                var productDto = _mapper.Map<ProductDto>(createProduct);
                return CreatedAtRoute("GetProduct", new { productId = product.ProductId }, productDto);
            }

            ModelState.AddModelError("CustomError",
                $"Something went wrong when saving the product {createProductDto.Name}.");
            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }

        [HttpGet("SearchProductByCategory/{categoryId:int}", Name = "GetProductsByCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetProductsByCategory(int categoryId)
        {
            var products = _productRepository.GetProductsByCategory(categoryId);
            if (products.Count == 0) return NotFound($"No products found for category ID {categoryId}.");
            var productsDto = _mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }

        [HttpGet("SearchProductByNameOrDescription/{searchTerm}", Name = "SearchProductsByNameOrDescription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult SearchProductsByNameOrDescription(string searchTerm)
        {
            var products = _productRepository.SearchProducts(searchTerm);
            if (products.Count == 0) return NotFound($"No products found for the search term '{searchTerm}'.");
            var productsDto = _mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }

        [HttpPatch("buyProduct/{productName}/{quantity:int}", Name = "BuyProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult BuyProduct(string productName, int quantity)
        {
            if (string.IsNullOrWhiteSpace(productName) || quantity <= 0)
                return BadRequest("Product name must be provided and quantity must be greater than zero.");

            var foundProduct = _productRepository.ProductExists(productName);
            if (!foundProduct)
                return NotFound($"Product with name '{productName}' not found.");

            if (!_productRepository.BuyProduct(productName, quantity))
            {
                ModelState.AddModelError("CustomError",
                    $"Unable to purchase product '{productName}' with quantity {quantity}. It may be out of stock or insufficient stock available.");
                return BadRequest(ModelState);
            }

            return Ok($"Product '{productName}' purchased successfully with quantity {quantity}.");
        }

        [HttpPut("{productId:int}", Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateProduct(int productId, [FromBody] UpdateProductDto? updateProductDto)
        {
            if (updateProductDto == null) return BadRequest(ModelState);
            if (!_productRepository.ProductExists(productId))
            {
                ModelState.AddModelError("CustomError", $"Product with ID {productId} does not exist.");
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.CategoryExists(updateProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError",
                    $"Category with ID {updateProductDto.CategoryId} does not exist.");
                return BadRequest(ModelState);
            }

            var product = _mapper.Map<Product>(updateProductDto);
            product.ProductId = productId;
            if (_productRepository.UpdateProduct(product))
                return NoContent();

            ModelState.AddModelError("CustomError",
                $"Something went wrong when updating the product {updateProductDto.Name}.");
            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }

        [HttpDelete("{productId:int}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteProduct(int productId)
        {
            if (productId <= 0)
            {
                ModelState.AddModelError("CustomError", "Product ID must be greater than zero.");
                return BadRequest(ModelState);
            }

            if (!_productRepository.ProductExists(productId))
            {
                ModelState.AddModelError("CustomError", $"Product with ID {productId} does not exist.");
                return NotFound(ModelState);
            }

            var product = _productRepository.GetProduct(productId);
            if (product == null) return NotFound($"Product with ID {productId} not found.");
            if (_productRepository.DeleteProduct(product))
                return NoContent();

            ModelState.AddModelError("CustomError",
                $"Something went wrong when deleting the product with ID {productId}.");
            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }
    }
}