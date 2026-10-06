using System.ComponentModel.DataAnnotations;

namespace KitapSatis.Web.Models;

public class CheckoutViewModel
{
    // Teslimat bilgileri
    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Telefon 10-11 haneli olmalı (sadece rakam).")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Adres zorunludur.")]
    public string Address { get; set; } = string.Empty;

    // Ödeme bilgileri (demo, kaydedilmez)
    [Required(ErrorMessage = "Kart üzerindeki isim zorunludur.")]
    public string CardHolder { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kart numarası zorunludur.")]
    [RegularExpression(@"^\d{16}$", ErrorMessage = "Kart numarası 16 haneli olmalı.")]
    public string CardNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Son kullanma tarihi zorunludur.")]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "AA/YY formatında girin (örn. 08/28).")]
    public string ExpiryDate { get; set; } = string.Empty;

    [Required(ErrorMessage = "CVV zorunludur.")]
    [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV 3 haneli olmalı.")]
    public string Cvv { get; set; } = string.Empty;
}