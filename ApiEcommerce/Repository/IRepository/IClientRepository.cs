using ApiEcommerce.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiEcommerce.Repository.IRepository
{
    public interface IClientRepository
    {
        Task<Client> AddClientAsync(Client client);
        Task<Client?> GetClientByIdAsync(int id);
        Task<IEnumerable<Client>> GetAllClientsAsync();
        Task<Client?> UpdateClientAsync(Client client);
        Task<bool> DeleteClientAsync(int id);
        // Phones
        Task<Phone?> AddPhoneAsync(int clientId, Phone phone);
        Task<bool> SetDefaultPhoneAsync(int clientId, int phoneId);
        Task<bool> DeletePhoneAsync(int clientId, int phoneId);
        // Addresses
        Task<Address?> AddAddressAsync(int clientId, Address address);
        Task<bool> SetDefaultAddressAsync(int clientId, int addressId);
        Task<bool> DeleteAddressAsync(int clientId, int addressId);
    }
}