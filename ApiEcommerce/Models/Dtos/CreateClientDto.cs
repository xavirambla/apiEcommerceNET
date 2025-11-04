using System.Collections.Generic;

namespace ApiEcommerce.Models.Dtos
{
    public class CreateClientDto
    {
        public string Name { get; set; }
        public List<CreatePhoneDto> Phones { get; set; }
        public List<CreateAddressDto> Addresses { get; set; }
    }
}