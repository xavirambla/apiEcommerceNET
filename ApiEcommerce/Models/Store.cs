// https://learn.microsoft.com/es-mx/ef/core/modeling/entity-properties?tabs=data-annotations%2Cwith-nrt#primary-key
using System.ComponentModel.DataAnnotations;


public class Store
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage="Creation date is required")]
    public DateTime CreationDate { get; set; }


}
