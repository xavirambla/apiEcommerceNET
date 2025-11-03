using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Models.Dtos.Responses;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using Mapster;
// using AutoMapper; // Migrado a Mapster
using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ApiEcommerce.Controllers
{
    [Authorize( Roles ="Admin")]    
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersionNeutral]       // sirve para todas las versiones
    public class ProductsController : ControllerBase
    {

        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        // private readonly IMapper _mapper; // Eliminado por migración a Mapster

        // ✅ Constructor dentro de la clase
        // public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper)
        // {
        //     _productRepository = productRepository;
        //     _categoryRepository = categoryRepository;
        //     _mapper = mapper;
        // }
        public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }
        
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProducts()
        {
            var products = _productRepository.GetProducts();
            /*
            var productsDto = new List<ProductDto>();

                        foreach (var product in products)
                        {
                            productsDto.Add(_mapper.Map<ProductDto>(product));
                        }
            */
            //var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
            var productsDto = products.Adapt<List<ProductDto>>();
            return Ok(productsDto);
        }




        [AllowAnonymous]
        [HttpGet("{productId:int}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProduct(int productId)
        {
            var product = _productRepository.GetProduct(productId);
            if (product == null)
            {
                return NotFound($"El producto con Id {productId} no existe.");
            }

            //var productDto = _mapper.Map<ProductDto>(product);
            var productDto = product.Adapt<ProductDto>();
            return Ok(productDto);
        }


        [AllowAnonymous]
        [HttpGet("Paged", Name = "GetProductsInPage")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProductsInPage([FromQuery] int pageNumber=1, [FromQuery] int pageSize=5)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("El número de página y el tamaño de página deben ser mayores que cero.");
            }
            var totalProducts = _productRepository.GetTotalProductsCount();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            if (pageNumber > totalPages)
            {
                return NotFound($"La página {pageNumber} no existe. El número total de páginas es {totalPages}.");
            }

            var products = _productRepository.GetProductsInPages(pageNumber, pageSize);
            var productDto = products.Adapt<List<ProductDto>>();
            if (products.Count == 0)
            {
                return NotFound($"No existen productos para la página {pageNumber} con tamaño de página {pageSize}.");
            }
            /*
            var productsDto = new List<ProductDto>();

                        foreach (var product in products)
                        {
                            productsDto.Add(_mapper.Map<ProductDto>(product));
                        }
            */

            var paginationResponse = new PaginationResponse<ProductDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                Items = productDto
            };
            return Ok( paginationResponse );
        }





        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // se cambia a fromform pq las imágenes se envían como form-data
        // public IActionResult CreateProduct([FromBody] CreateProductDto createProductDto)
        public IActionResult CreateProduct([FromForm] CreateProductDto createProductDto)
        {

            if (createProductDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_productRepository.ProductExists(createProductDto.Name))
            {
                ModelState.AddModelError("CustomError", "El producto ya existe.");
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.CategoryExists(createProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", $"La categoría con el {createProductDto.CategoryId} no existe.");
                return BadRequest(ModelState);
            }
            var product = createProductDto.Adapt<Product>();
            product.Category = null; // <- Esto es clave para evitar el error de IDENTITY_INSERT
            if (createProductDto.Image != null)
            {
                UploadProductImage(createProductDto, product);
                
                
            }
            else
            {
                // sacara imagen de muesta https://placehold.co/
                product.ImgUrl = "https://placehold.co/300x300";
            }

            
            // agregando la imagen

            if (!_productRepository.CreateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al guardar el producto {product.Name}");
                return StatusCode(500, ModelState);  // otra manera distinta de hacer el badrequest                
            }
            var createdProduct = _productRepository.GetProduct(product.ProductId);
            var productoDto = createdProduct.Adapt<ProductDto>();            
            //var productDto = _mapper.Map<ProductDto>(createProduct);

            return CreatedAtRoute("GetProduct", new { productId = product.ProductId }, productoDto);
        }



        [HttpGet("searchProductByCategory{categoryId:int}", Name = "GetProductsForCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProductsForCategory(int categoryId)
        {

            var products = _productRepository.GetProductsForCategory(categoryId);
            if (products.Count == 0)
            {
                return NotFound($"No existen productos para la categoría con Id {categoryId}.");
            }
            /*
            var productsDto = new List<ProductDto>();

                        foreach (var product in products)
                        {
                            productsDto.Add(_mapper.Map<ProductDto>(product));
                        }
            */
        //    var productsDto = _mapper.Map<List<ProductDto>>(products);
            var productsDto = products.Adapt<List<ProductDto>>();
            return Ok(productsDto);
        }

        [HttpGet("searchProductByNameDescription/{searchTerm}", Name = "SearchProducts")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult SearchProducts(string searchTerm)
        {

            var products = _productRepository.SearchProducts(searchTerm);
            if (products.Count == 0)
            {
                return NotFound($"No existen productos con el nombre o descripción '{searchTerm}'.");
            }
            /*
            var productsDto = new List<ProductDto>();

                        foreach (var product in products)
                        {
                            productsDto.Add(_mapper.Map<ProductDto>(product));
                        }
            */
            //            var productsDto = _mapper.Map<List<ProductDto>>(products);
            var productsDto = products.Adapt<List<ProductDto>>();
            return Ok(productsDto);
        }



        [HttpPatch("buyProduct/{name}/{quantity:int}", Name = "BuyProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult BuyProduct(string name, int quantity)
        {
            if (string.IsNullOrWhiteSpace(name) || quantity <= 0)
            {
                return BadRequest("El nombre de producto o la cantidad no son válidos.");
            }
            var foundProduct = _productRepository.ProductExists(name);
            if (!foundProduct)
            {
                return NotFound($"No existe ningún producto con el nombre '{name}'.");
            }
            if (!_productRepository.BuyProduct(name, quantity))
            {
                ModelState.AddModelError("CustomError", $"No hay suficiente stock del producto '{name}' para completar la compra.");
                return BadRequest(ModelState);
            }
            var units = quantity == 1 ? "unidad" : "unidades";
            return Ok($"La compra de {quantity} {units} del producto '{name}' se ha realizado con éxito.");

        }

        [HttpPut("{productId:int}", Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateProduct(int productId, [FromForm] UpdateProductDto updateProductDto)
        {

            if (updateProductDto == null)
            {
                return BadRequest(ModelState);
            }

            if (!_productRepository.ProductExists(productId))
//            if (!_productRepository.ProductExists(updateProductDto.Name))
            {
                ModelState.AddModelError("CustomError", "El producto no existe.");
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.CategoryExists(updateProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", $"La categoría con el {updateProductDto.CategoryId} no existe.");
                return BadRequest(ModelState);
            }

            //var product = _mapper.Map<Product>(updateProductDto);
            var product = updateProductDto.Adapt<Product>();
            product.ProductId = productId;
            
            // agregando la imagen
            if (updateProductDto.Image != null)
            {
                //creamos un nombre único para este archivo
                UploadProductImage(updateProductDto, product);

            }
            else
            {
                // sacara imagen de muesta https://placehold.co/
                product.ImgUrl = "https://placehold.co/300x300";
            }



            if (!_productRepository.UpdateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al actualizar el producto {product.Name}");
                return StatusCode(500, ModelState);  // otra manera distinta de hacer el badrequest                
            }
            return NoContent();
        }

        private void UploadProductImage(dynamic productDto, Product product)
        {
            string fileName = product.ProductId + Guid.NewGuid().ToString() + Path.GetExtension(productDto.Image.FileName);
            var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductsOmages");
            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }
            var filePath = Path.Combine(imagesFolder, fileName);
            FileInfo fileInfo = new FileInfo(filePath);

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                productDto.Image.CopyTo(fileStream);
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
            product.ImgUrl = $"{baseUrl}/ProductsImages/{fileName}";
            product.ImgUrlLocal = filePath;
        }

        [HttpDelete("{productId:int}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteProduct(int productId)
        {
            if (productId <= 0)
            {
                return BadRequest(ModelState);
            }

            var product = _productRepository.GetProduct(productId);
            if (product == null)
            {
                return NotFound($"El producto con Id {productId} no existe.");
            }
            if (!_productRepository.DeleteProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al eliminar el producto {product.Name}");
                return StatusCode(500, ModelState);  // otra manera distinta de hacer el badrequest                
            }
            
            return NoContent();
        }



    }
}