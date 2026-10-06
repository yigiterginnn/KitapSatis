using System.ComponentModel.DataAnnotations;

namespace KitapSatis.Api.Dtos;

public class CategoryDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kategori adı zorunludur.")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}