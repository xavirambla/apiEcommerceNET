using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiEcommerce.Controllers
{
    

    [Authorize( Roles ="Admin")]    
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersionNeutral]       // sirve para todas las versiones
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClientDto>> CreateClient([FromBody] CreateClientDto dto)
        {
            var client = new Client
            {
                Name = dto.Name ?? string.Empty,
                Phones = dto.Phones?.Select(p => new Phone { Number = p.Number ?? string.Empty, IsDefault = p.IsDefault }).ToList() ?? new List<Phone>(),
                Addresses = dto.Addresses?.Select(a => new Address { Street = a.Street ?? string.Empty, City = a.City ?? string.Empty, State = a.State ?? string.Empty, ZipCode = a.ZipCode ?? string.Empty, IsDefault = a.IsDefault }).ToList() ?? new List<Address>()
            };
            var created = await _clientRepository.AddClientAsync(client);
            return CreatedAtAction(nameof(GetClient), new { id = created.Id }, created.Adapt<ClientDto>());
        }

    [AllowAnonymous]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientDto>> GetClient(int id)
        {
            var client = await _clientRepository.GetClientByIdAsync(id);
            if (client == null) return NotFound();
            return Ok(client.Adapt<ClientDto>());
        }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
        {
            var clients = await _clientRepository.GetAllClientsAsync();
            return Ok(clients.Adapt<IEnumerable<ClientDto>>());
        }

    [HttpPost("{clientId}/phones")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PhoneDto>> AddPhone(int clientId, [FromBody] CreatePhoneDto dto)
        {
            var phone = new Phone { Number = dto.Number ?? string.Empty, IsDefault = dto.IsDefault, ClientId = clientId };
            var result = await _clientRepository.AddPhoneAsync(clientId, phone);
            if (result == null) return NotFound();
            return Ok(result.Adapt<PhoneDto>());
        }

    [HttpPut("{clientId}/phones/{phoneId}/default")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetDefaultPhone(int clientId, int phoneId)
        {
            var ok = await _clientRepository.SetDefaultPhoneAsync(clientId, phoneId);
            if (!ok) return NotFound();
            return NoContent();
        }

    [HttpDelete("{clientId}/phones/{phoneId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePhone(int clientId, int phoneId)
        {
            var ok = await _clientRepository.DeletePhoneAsync(clientId, phoneId);
            if (!ok) return NotFound();
            return NoContent();
        }

    [HttpPost("{clientId}/addresses")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AddressDto>> AddAddress(int clientId, [FromBody] CreateAddressDto dto)
        {
            var address = new Address { Street = dto.Street ?? string.Empty, City = dto.City ?? string.Empty, State = dto.State ?? string.Empty, ZipCode = dto.ZipCode ?? string.Empty, IsDefault = dto.IsDefault, ClientId = clientId };
            var result = await _clientRepository.AddAddressAsync(clientId, address);
            if (result == null) return NotFound();
            return Ok(result.Adapt<AddressDto>());
        }

    [HttpPut("{clientId}/addresses/{addressId}/default")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetDefaultAddress(int clientId, int addressId)
        {
            var ok = await _clientRepository.SetDefaultAddressAsync(clientId, addressId);
            if (!ok) return NotFound();
            return NoContent();
        }

    [HttpDelete("{clientId}/addresses/{addressId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAddress(int clientId, int addressId)
        {
            var ok = await _clientRepository.DeleteAddressAsync(clientId, addressId);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}