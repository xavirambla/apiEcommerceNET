using ApiEcommerce.Data;
using ApiEcommerce.Models;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiEcommerce.Repository
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;
        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Client> AddClientAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return client;
        }

        public async Task<Client?> GetClientByIdAsync(int id)
        {
            return await _context.Clients
                .Include(c => c.Phones)
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _context.Clients
                .Include(c => c.Phones)
                .Include(c => c.Addresses)
                .ToListAsync();
        }

        public async Task<Client?> UpdateClientAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
            return client;
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return false;
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Phone?> AddPhoneAsync(int clientId, Phone phone)
        {
            var client = await _context.Clients.Include(c => c.Phones).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client == null) return null;
            if (phone.IsDefault)
            {
                foreach (var p in client.Phones)
                    p.IsDefault = false;
            }
            client.Phones.Add(phone);
            await _context.SaveChangesAsync();
            return phone;
        }

        public async Task<bool> SetDefaultPhoneAsync(int clientId, int phoneId)
        {
            var client = await _context.Clients.Include(c => c.Phones).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client == null) return false;
            foreach (var p in client.Phones)
                p.IsDefault = (p.Id == phoneId);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePhoneAsync(int clientId, int phoneId)
        {
            var client = await _context.Clients.Include(c => c.Phones).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client == null) return false;
            var phone = client.Phones.FirstOrDefault(p => p.Id == phoneId);
            if (phone == null) return false;
            client.Phones.Remove(phone);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Address?> AddAddressAsync(int clientId, Address address)
        {
            var client = await _context.Clients.Include(c => c.Addresses).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client == null) return null;
            if (address.IsDefault)
            {
                foreach (var a in client.Addresses)
                    a.IsDefault = false;
            }
            client.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<bool> SetDefaultAddressAsync(int clientId, int addressId)
        {
            var client = await _context.Clients.Include(c => c.Addresses).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client == null) return false;
            foreach (var a in client.Addresses)
                a.IsDefault = (a.Id == addressId);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAddressAsync(int clientId, int addressId)
        {
            var client = await _context.Clients.Include(c => c.Addresses).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client == null) return false;
            var address = client.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null) return false;
            client.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}