using ApiEcommerce.Constants;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]    
    [ApiController]
    [Authorize( Roles ="Admin")]
    //    [EnableCors("AllowSpecificOrigin")]
//[EnableCors(PolicyNames.AllowSpecificOrigin)]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        // Mapster no requiere inyección de IMapper
        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [AllowAnonymous]   //permite ver las categorías a todo el mundo
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Obsolete("Este método está obsoleto. Use GetCategoriesOrderById de la versión 2.0 en su lugar.")]
        //[EnableCors("AllowSpecificOrigin")]
        public IActionResult GetCategories()
        {
            var categories = _categoryRepository.GetCategories();
            var categoriesDto = categories.Adapt<List<CategoryDto>>();
            return Ok(categoriesDto);
        }



        [AllowAnonymous]   //permite ver las categorías a todo el mundo
        [HttpGet("{id:int}", Name = "GetCategory")]
//        [ResponseCache(Duration = 10)]  // cachea la respuesta durante 60 segundos
        [ResponseCache(CacheProfileName =CacheProfiles.Default10)]  // cachea la respuesta durante 60 segundos
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategory(int id)
        {
            System.Console.WriteLine($"Obteniendo categoría con Id: {id} a las {DateTime.Now}");
            var category = _categoryRepository.GetCategory(id);
            System.Console.WriteLine($"Respuesta con el Id : {id } : {category?.Name ?? "No encontrada"}");
            if (category == null)
            {
                return NotFound($"La categoría con Id {id} no existe.");
            }

            var categoryDto = category.Adapt<CategoryDto>();
            return Ok(categoryDto);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {

            if (createCategoryDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_categoryRepository.CategoryExists(createCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "La categoría ya existe.");
                return BadRequest(ModelState);
            }

            var category = createCategoryDto.Adapt<Category>();

            if (!_categoryRepository.CreateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al guardar la categoría {category.Name}");
                return StatusCode(500, ModelState);  // otra manera distinta de hacer el badrequest                
            }

            return CreatedAtRoute("GetCategory", new { id = category.Id }, category);
        }



        [HttpPatch("{id:int}", Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCategory(int id, [FromBody] CreateCategoryDto updateCategoryDto)
        {


            if (updateCategoryDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_categoryRepository.CategoryExists(updateCategoryDto.Name))
            {
                return NotFound($"La categoría ya existe.");
            }



            var category = updateCategoryDto.Adapt<Category>();
            category.Id = id;

            if (!_categoryRepository.UpdateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al actualizar la categoría {category.Name}");
                return StatusCode(500, ModelState);  // otra manera distinta de hacer el badrequest                
            }

            return NoContent();
        }


        [HttpDelete("{id:int}", Name = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCategory(int id)
        {


            if (!_categoryRepository.CategoryExists(id))
            {
                return NotFound($"La categoría con Id {id} no existe.");
            }



            var category = _categoryRepository.GetCategory(id);
            if (category == null)
            {
                return NotFound($"La categoría con Id {id} no existe.");
            }

            if (!_categoryRepository.DeleteCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al eliminar la categoría {category.Name}");
                return StatusCode(500, ModelState);  // otra manera distinta de hacer el badrequest                
            }
            return NoContent();
        }

    }
}

