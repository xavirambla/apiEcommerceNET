using System.Collections.Generic;

namespace ApiEcommerce.Models.Dtos
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<PhoneDto>? Phones { get; set; }
        public List<AddressDto>? Addresses { get; set; }
    }
}