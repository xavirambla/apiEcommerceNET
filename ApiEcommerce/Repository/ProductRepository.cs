using ApiEcommerce.Models;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _db;

    public ProductRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public ICollection<Product> GetProducts()
    {
        return _db.Products.OrderBy(c => c.Name).ToList();
    }

    public ICollection<Product> GetProductsInPages(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return new List<Product>();
        }

        return _db.Products
            .OrderBy(p => p.ProductId)
            .Skip((pageNumber - 1) * pageSize) // saltamos los registros de las páginas anteriores
            .Take(pageSize)   // cogemos solo pageSize registros           
            .ToList();
    }

    public int GetTotalProductsCount()
    {
        return _db.Products.Count();
    }



    public Product? GetProduct(int productId)
    {
        if (productId <= 0)
        {
            return null;
        }

        //return _db.Products.FirstOrDefault(p => p.ProductId == productId);
        return _db.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductId == productId);
    }   

    public ICollection<Product> GetProductsForCategory(int categoryId)
    {
        if (categoryId <= 0)
        {
            return new List<Product>();
        }
        return _db.Products.Include(p=>p.Category).Where(p => p.CategoryId == categoryId).ToList();

    }

    public ICollection<Product> SearchProducts(string searchTerm)
    {
        IQueryable<Product> query = _db.Products;
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchTermLower = searchTerm.ToLower().Trim();
            query = query.Include(p => p.Category)
            .Where(p => p.Name.ToLower().Trim().Contains( searchTermLower ) || 
             p.Description.ToLower().Trim().Contains( searchTermLower ) 
            );
        }
        return query.OrderBy(p => p.Name).ToList();
    }

    public bool BuyProduct(string name, int quantity)
    {
        if (string.IsNullOrWhiteSpace(name) || quantity <= 0)
        {
            return false;
        }

        var product = _db.Products.FirstOrDefault(p => p.Name.ToLower().Trim() == name.ToLower().Trim());



        if (product == null || product.Stock < quantity)
        {
            return false;
        }
        
        product.Stock -= quantity;
        _db.Products.Update(product);
        return Save();
    }




    public bool ProductExists(int id)
    {
        if (id <= 0)
        {
            return false;
        }
        return _db.Products.Any(p => p.ProductId == id);

    }



    public bool ProductExists(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }
        return _db.Products.Any(p => p.Name.ToLower().Trim() == name.ToLower().Trim());
    }



    public bool CreateProduct(Product product)
    {
        if (product == null)
        {
            return false;
        }

        product.CreationDate = DateTime.Now;
        product.UpdateDate = DateTime.Now;        
        _db.Products.Add(product);
        return Save();
    }


    public bool UpdateProduct(Product product)
    {
        if(product == null)
        {
            return false;
        }

        product.UpdateDate = DateTime.Now;
        _db.Products.Update(product);
        return Save();

    }
    
    public bool DeleteProduct(Product product)
    {  
     if (product == null)
        {
            return false;
        }
        _db.Products.Remove( product);
        return Save();
    }


    public bool Save()
    {
//        return _db.SaveChanges()>=0 ? true: false;
        return _db.SaveChanges()>=0 ;// es lo mismo que la anterior  línea
    }




}

