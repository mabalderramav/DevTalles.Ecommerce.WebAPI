using AutoMapper;
using DevTalles.Ecommerce.WebAPI.Models;
using DevTalles.Ecommerce.WebAPI.Models.Dtos.Category;
using DevTalles.Ecommerce.WebAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace DevTalles.Ecommerce.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ICategoryRepository categoryRepository, IMapper mapper) : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetCategories()
        {
            var categories = _categoryRepository.GetCategories();
            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories);
            return Ok(categoriesDto);
        }

        [HttpGet("{id:int}", Name = "GetCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCategory(int id)
        {
            var category = _categoryRepository.GetCategory(id);
            if (category == null) return NotFound($"Category with ID {id} not found.");
            var categoryDto = _mapper.Map<CategoryDto>(category);
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
            if (_categoryRepository.CategoryExists(createCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError",
                    $"Category with name {createCategoryDto.Name} already exists.");
                return BadRequest(ModelState);
            }

            var category = _mapper.Map<Category>(createCategoryDto);
            if (_categoryRepository.CreateCategory(category))
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
            if (!_categoryRepository.CategoryExists(id)) return NotFound($"Category with ID {id} not found.");
            if (updateCategoryDto == null) return BadRequest(ModelState);
            if (_categoryRepository.CategoryExists(updateCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError",
                    $"Category with name {updateCategoryDto.Name} already exists.");
                return BadRequest(ModelState);
            }

            var category = _mapper.Map<Category>(updateCategoryDto);
            category.Id = id;
            if (_categoryRepository.UpdateCategory(category)) return NoContent();
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
            if (!_categoryRepository.CategoryExists(id)) return NotFound($"Category with ID {id} not found.");
            var category = _categoryRepository.GetCategory(id);
            if (category == null) return NotFound($"Category with ID {id} not found.");
            if (_categoryRepository.DeleteCategory(category)) return NoContent();
            ModelState.AddModelError("CustomError",
                $"Something went wrong when deleting the category with ID {category.Id} and name {category.Name}.");
            return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
        }
    }
}