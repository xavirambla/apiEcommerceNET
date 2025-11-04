using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models
{
    public class Client
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public ICollection<Phone> Phones { get; set; } = new List<Phone>();
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }

    public class Phone
    {
        public int Id { get; set; }
        [Required]
        public string Number { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
    }

    public class Address
    {
        public int Id { get; set; }
        [Required]
        public string Street { get; set; } = string.Empty;
        [Required]
        public string City { get; set; } = string.Empty;
        [Required]
        public string State { get; set; } = string.Empty;
        [Required]
        public string ZipCode { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}