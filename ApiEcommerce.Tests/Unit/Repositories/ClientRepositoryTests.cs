using ApiEcommerce.Data;
using ApiEcommerce.Models;
using ApiEcommerce.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace ApiEcommerce.Tests.Unit.Repositories
{
    public class ClientRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly ClientRepository _repo;

        public ClientRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_Clients")
                .Options;
            _context = new ApplicationDbContext(options);
            _repo = new ClientRepository(_context);
        }

        [Fact]
        public async Task AddClientAsync_Adds_Client()
        {
            var client = new Client { Name = "RepoTest" };
            var result = await _repo.AddClientAsync(client);
            result.Id.Should().BeGreaterThan(0);
            (await _context.Clients.FindAsync(result.Id)).Should().NotBeNull();
        }

        [Fact]
        public async Task AddPhoneAsync_Sets_Default_Correctly()
        {
            var client = new Client { Name = "PhoneTest" };
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            var phone1 = new Phone { Number = "111", IsDefault = true };
            await _repo.AddPhoneAsync(client.Id, phone1);
            var phone2 = new Phone { Number = "222", IsDefault = true };
            await _repo.AddPhoneAsync(client.Id, phone2);

            var dbClient = await _repo.GetClientByIdAsync(client.Id);
            dbClient!.Phones.Count.Should().Be(2);
            dbClient.Phones.Count(p => p.IsDefault).Should().Be(1);
            dbClient.Phones.Should().ContainSingle(p => p.IsDefault && p.Number == "222");
        }

        [Fact]
        public async Task AddAddressAsync_Sets_Default_Correctly()
        {
            var client = new Client { Name = "AddrTest" };
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            var addr1 = new Address { Street = "C1", City = "C", State = "S", ZipCode = "1", IsDefault = true };
            await _repo.AddAddressAsync(client.Id, addr1);
            var addr2 = new Address { Street = "C2", City = "C", State = "S", ZipCode = "2", IsDefault = true };
            await _repo.AddAddressAsync(client.Id, addr2);

            var dbClient = await _repo.GetClientByIdAsync(client.Id);
            dbClient!.Addresses.Count.Should().Be(2);
            dbClient.Addresses.Count(a => a.IsDefault).Should().Be(1);
            dbClient.Addresses.Should().ContainSingle(a => a.IsDefault && a.Street == "C2");
        }
    }
}
