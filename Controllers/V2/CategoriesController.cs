using Asp.Versioning;
using AutoMapper;
using DevTalles.Ecommerce.WebAPI.Constants;
using DevTalles.Ecommerce.WebAPI.Models;
using DevTalles.Ecommerce.WebAPI.Models.Dtos.Category;
using DevTalles.Ecommerce.WebAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace DevTalles.Ecommerce.WebAPI.Controllers.V2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    [EnableCors(PolicyName.AllowSpecificOrigin)]
    [Authorize(Roles = "Admin")]
    public class CategoriesController(ICategoryRepository categoryRepository, IMapper mapper) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetCategoriesOrderById()
        {
            var categories = categoryRepository.GetCategories().OrderBy(c => c.Id).ToList();
            var categoriesDto = mapper.Map<List<CategoryDto>>(categories);
            return Ok(categoriesDto);
        }

        [HttpGet("{id:int}", Name = "GetCategory")]
        // [ResponseCache(Duration = 60)]
        [ResponseCache(CacheProfileName = CacheProfiles.DefaultCacheProfile)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [EnableCors(PolicyName.AllowSpecificOrigin)]
        public IActionResult GetCategory(int id)
        {
            Console.WriteLine($"Received request for category with ID: {id} at {DateTime.UtcNow}");
            var category = categoryRepository.GetCategory(id);
            Console.WriteLine(
                $"Category retrieval result for ID {id}: {(category != null ? "Found" : "Not Found")} at {DateTime.UtcNow}");
            if (category == null) return NotFound($"Category with ID {id} not found.");
            var categoryDto = mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateCategory([FromBody] CreateCategoryDto? createCategoryDto)
        {
            if (createCategoryDto == null) return BadRequest(ModelState);
            if (categoryRepository.CategoryExists(createCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError",
                    $"Category with name {createCategoryDto.Name} already exists.");
                return BadRequest(ModelState);
            }

            var category = mapper.Map<Category>(createCategoryDto);
            if (categoryRepository.CreateCategory(category))
                return CreatedAtRoute("GetCategory", new { id = category.Id }, category);
            ModelState.AddModelError("CustomError",
                $"Something went wrong when saving the category {createCategoryDto.Name}.");
            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }

        [HttpPatch("{id:int}", Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCategory(int id, [FromBody] CreateCategoryDto? updateCategoryDto)
        {
            if (!categoryRepository.CategoryExists(id)) return NotFound($"Category with ID {id} not found.");
            if (updateCategoryDto == null) return BadRequest(ModelState);
            if (categoryRepository.CategoryExists(updateCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError",
                    $"Category with name {updateCategoryDto.Name} already exists.");
                return BadRequest(ModelState);
            }

            var category = mapper.Map<Category>(updateCategoryDto);
            category.Id = id;
            if (categoryRepository.UpdateCategory(category)) return NoContent();
            ModelState.AddModelError("CustomError",
                $"Something went wrong when updating the category {updateCategoryDto.Name}.");
            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }

        [HttpDelete("{id:int}", Name = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCategory(int id)
        {
            if (!categoryRepository.CategoryExists(id)) return NotFound($"Category with ID {id} not found.");
            var category = categoryRepository.GetCategory(id);
            if (category == null) return NotFound($"Category with ID {id} not found.");
            if (categoryRepository.DeleteCategory(category)) return NoContent();
            ModelState.AddModelError("CustomError",
                $"Something went wrong when deleting the category with ID {category.Id} and name {category.Name}.");
            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }
    }
}