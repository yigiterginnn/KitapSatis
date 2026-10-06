using System.Security.Claims;
using KitapSatis.Web.Data.Repositories;
using KitapSatis.Web.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KitapSatis.Web.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly IRepository<CartItem> _cartRepository;
    private readonly IRepository<Product> _productRepository;

    public CartController(IRepository<CartItem> cartRepository, IRepository<Product> productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    // Giriş yapmış kullanıcının Id'si
    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // Sepeti göster
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        var items = await _cartRepository.Query()
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();
        return View(items);
    }

    // Sepete ekle
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            return NotFound();

        if (quantity < 1)
            quantity = 1;

        var userId = GetUserId();
        var item = (await _cartRepository.WhereAsync(c => c.UserId == userId && c.ProductId == productId))
            .FirstOrDefault();

        var newQuantity = (item?.Quantity ?? 0) + quantity;
        if (newQuantity > product.Stock)
        {
            TempData["Error"] = $"Stokta sadece {product.Stock} adet var.";
            return RedirectToAction("Details", "Home", new { id = productId });
        }

        if (item == null)
        {
            await _cartRepository.AddAsync(new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity
            });
        }
        else
        {
            item.Quantity = newQuantity;
            _cartRepository.Update(item);
        }

        await _cartRepository.SaveAsync();
        TempData["Message"] = $"{product.Name} sepete eklendi.";
        return RedirectToAction(nameof(Index));
    }

    // Adet güncelle
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateQuantity(int id, int quantity)
    {
        var userId = GetUserId();
        var item = await _cartRepository.Query()
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (item == null)
            return NotFound();

        if (quantity <= 0)
        {
            _cartRepository.Delete(item);
        }
        else if (quantity > item.Product!.Stock)
        {
            TempData["Error"] = $"Stokta sadece {item.Product.Stock} adet var.";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            item.Quantity = quantity;
            _cartRepository.Update(item);
        }

        await _cartRepository.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    // Sepetten çıkar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id)
    {
        var userId = GetUserId();
        var item = (await _cartRepository.WhereAsync(c => c.Id == id && c.UserId == userId))
            .FirstOrDefault();

        if (item == null)
            return NotFound();

        _cartRepository.Delete(item);
        await _cartRepository.SaveAsync();
        TempData["Message"] = "Ürün sepetten çıkarıldı.";
        return RedirectToAction(nameof(Index));
    }
}