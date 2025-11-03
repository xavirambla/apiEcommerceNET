namespace ApiEcommerce.Repository.IRepository
{
    public interface IStoreRepository
    {
        ICollection<Store> GetStores();
        Store? GetStore(int storeId);

        bool StoreExists(string name);
        bool StoreExists(int id);
        bool CreateStore(Store store);
        bool UpdateStore(Store store);
        bool DeleteStore(Store store);

        bool Save();
        
    }
}