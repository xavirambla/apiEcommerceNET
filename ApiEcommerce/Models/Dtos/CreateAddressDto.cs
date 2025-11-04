namespace ApiEcommerce.Models.Dtos
{
    public class CreateAddressDto
    {
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
        public bool IsDefault { get; set; }
    }
}