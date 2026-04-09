using System.ComponentModel.DataAnnotations;

namespace Marketeer.Models;

public class AdminProductViewModel
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 999999)]
    public decimal Price { get; set; }

    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int AvailableQuantity { get; set; }

    [Range(0, int.MaxValue)]
    public int ReservedQuantity { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}
