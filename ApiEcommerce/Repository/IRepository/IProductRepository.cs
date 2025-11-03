/*
=============
🏆 Ejercicio 
=============
*/
// 1. Crear una interfaz llamada IProductRepository.
//
// 2. Incluir los siguientes métodos en la interfaz:
//
//    - GetProducts
//        → Devuelve todos los productos
//          en ICollection del tipo Product.
//
//    - GetProductsForCategory
//        → Recibe un categoryId y devuelve los productos
//          de esa categoría en ICollection del tipo Product.
//
//    - SearchProduct
//        → Recibe un nombre y devuelve los productos
//          que coincidan en ICollection del tipo Product.
//
//    - GetProduct
//        → Recibe un id y 
//          devuelve un solo objeto Product
//          o null si no se encuentra.
//
//    - BuyProduct
//        → Recibe el nombre del producto y una cantidad,
//          y devuelve un bool indicando si la compra fue exitosa.
//
//    - ProductExists (por id)
//        → Recibe un id y devuelve un bool
//          indicando si existe el producto.
//
//    - ProductExists (por nombre)
//        → Recibe un nombre y devuelve un bool
//          indicando si existe el producto.
//
//    - CreateProduct
//        → Recibe un objeto Product 
//          y devuelve un bool indicando si la creación fue exitosa.
//
//    - UpdateProduct
//        → Recibe un objeto Product
//          y devuelve un bool indicando si la actualización fue exitosa.
//
//    - DeleteProduct
//        → Recibe un objeto Product
//          y devuelve un bool indicando si la eliminación fue exitosa.
//
//    - Save
//        → Devuelve un bool indicando
//          si los cambios se guardaron correctamente.
using ApiEcommerce.Models;

public interface IProductRepository
{
    // Tu código aquí



    public ICollection<Product> GetProducts();

    public ICollection<Product> GetProductsInPages(int pageNumber, int pageSize);
    int GetTotalProductsCount();



    public ICollection<Product> GetProductsForCategory(int categoryId);


    public ICollection<Product> SearchProducts(string searchTerm);

    public Product? GetProduct(int productId);

    public bool BuyProduct(string name, int quantity);


    
    public bool ProductExists(int id);




    public bool ProductExists(string name);



    public bool CreateProduct(Product product);



    public bool UpdateProduct(Product product);


    public bool DeleteProduct(Product product);


    public bool Save();


}




