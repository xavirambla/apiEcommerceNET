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
    public class StoresController : ControllerBase
    {
        private readonly IStoreRepository _StoreRepository;
        // Mapster no requiere inyección de IMapper
        public StoresController(IStoreRepository StoreRepository)
        {
            _StoreRepository = StoreRepository;
        }



        [AllowAnonymous]   //permite ver las Tiendas a todo el mundo
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[EnableCors("AllowSpecificOrigin")]
        public IActionResult GetStores()
        {
            var Stores = _StoreRepository.GetStores();
            var StoresDto = Stores.Adapt<List<StoreDto>>();
            return Ok(StoresDto);
        }



        [AllowAnonymous]   //permite ver las Tiendas a todo el mundo
        [HttpGet("{id:int}", Name = "GetStore")]
//        [ResponseCache(Duration = 10)]  // cachea la respuesta durante 60 segundos
        [ResponseCache(CacheProfileName =CacheProfiles.Default10)]  // cachea la respuesta durante 60 segundos
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetStore(int id)
        {
            System.Console.WriteLine($"Obteniendo tienda con Id: {id} a las {DateTime.Now}");
            var Store = _StoreRepository.GetStore(id);
            System.Console.WriteLine($"Respuesta con el Id : {id } : {Store?.Name ?? "No encontrada"}");
            if (Store == null)
            {
                return NotFound($"La tienda con Id {id} no existe.");
            }

            var StoreDto = Store.Adapt<StoreDto>();
            return Ok(StoreDto);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateStore([FromBody] CreateStoreDto createStoreDto)
        {

            if (createStoreDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_StoreRepository.StoreExists(createStoreDto.Name))
            {
                ModelState.AddModelError("CustomError", "La tienda ya existe.");
                return BadRequest(ModelState);
            }

            var Store = createStoreDto.Adapt<Store>();

            if (!_StoreRepository.CreateStore(Store))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al guardar la tienda {Store.Name}");
                return StatusCode(500, ModelState);  // otra manera distinta de hacer el badrequest                
            }

            return CreatedAtRoute("GetStore", new { id = Store.Id }, Store);
        }



        [HttpPatch("{id:int}", Name = "UpdateStore")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateStore(int id, [FromBody] CreateStoreDto updateStoreDto)
        {


            if (updateStoreDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_StoreRepository.StoreExists(updateStoreDto.Name))
            {
                return NotFound($"La tienda ya existe.");
            }



            var Store = updateStoreDto.Adapt<Store>();
            Store.Id = id;

            if (!_StoreRepository.UpdateStore(Store))
            {
                if (!_StoreRepository.StoreExists(id))
                {
                    return NotFound($"La tienda con Id {id} no existe.");
                }
                else
                {
                    ModelState.AddModelError("CustomError", $"Algo salió mal al actualizar la tienda {Store.Name}");
                    return StatusCode(500, ModelState);  // otra manera distinta de hacer el badrequest                
                }
            }

            return NoContent();
        }


        [HttpDelete("{id:int}", Name = "DeleteStore")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteStore(int id)
        {


            if (!_StoreRepository.StoreExists(id))
            {
                return NotFound($"La tienda con Id {id} no existe.");
            }



            var Store = _StoreRepository.GetStore(id);
            if (Store == null)
            {
                return NotFound($"La tienda con Id {id} no existe.");
            }

            if (!_StoreRepository.DeleteStore(Store))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al eliminar la tienda {Store.Name}");
                return StatusCode(500, ModelState);  // otra manera distinta de hacer el badrequest                
            }
            return NoContent();
        }



        [AllowAnonymous]   //permite ver las tiendass a todo el mundo
        [HttpGet("orderedById")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[EnableCors("AllowSpecificOrigin")]
        public IActionResult GetStoresOrderById()
        {
            var stores = _StoreRepository.GetStores().OrderBy(c => c.Id);
            var storesDto = stores.Adapt<List<StoreDto>>();
            return Ok(storesDto);
        }



    }
}

