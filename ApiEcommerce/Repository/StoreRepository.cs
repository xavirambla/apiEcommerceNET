using ApiEcommerce.Repository.IRepository;

public class StoreRepository : IStoreRepository
{
    private readonly ApplicationDbContext _db;

    public StoreRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public bool StoreExists(string name)
    {
        return _db.Categories.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public bool StoreExists(int id)
    {
        return _db.Stores.Any(c => c.Id == id);
        
    }

    public bool CreateStore(Store store)
    {
        store.CreationDate = DateTime.Now;
        _db.Stores.Add(store);
        return Save();
    }

    public bool DeleteStore(Store store)
    {
        _db.Stores.Remove(store);
        return Save();
    }

    public ICollection<Store> GetStores()
    {
        return _db.Stores.OrderBy(c => c.Name).ToList();
    }

    public Store? GetStore(int storeId)
    {
        return _db.Stores.FirstOrDefault(c => c.Id == storeId) ;
    }

    public bool Save()
    {
        return _db.SaveChanges()>=0 ? true: false;
    }

    public bool UpdateStore(Store store)
    {
        store.CreationDate = DateTime.Now;
        _db.Stores.Update(store);
        return Save();
        
    }
}