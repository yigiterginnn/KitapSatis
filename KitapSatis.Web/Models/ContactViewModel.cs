using System.ComponentModel.DataAnnotations;

namespace KitapSatis.Web.Models;

public class ContactViewModel
{
    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mesaj zorunludur.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Mesaj en az 10 karakter olmalı.")]
    public string Message { get; set; } = string.Empty;
}