using System.Security.Claims;
using KitapSatis.Data.Repositories;
using KitapSatis.Data.Entities;
using KitapSatis.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KitapSatis.Web.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<CartItem> _cartRepository;
    private readonly IRepository<Product> _productRepository;

    public OrderController(
        IRepository<Order> orderRepository,
        IRepository<CartItem> cartRepository,
        IRepository<Product> productRepository)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    private async Task<List<CartItem>> GetCartItemsAsync(string userId)
    {
        return await _cartRepository.Query()
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    // Ödeme formunu göster
    public async Task<IActionResult> Checkout()
    {
        var cartItems = await GetCartItemsAsync(GetUserId());
        if (!cartItems.Any())
            return RedirectToAction("Index", "Cart");

        ViewBag.CartItems = cartItems;
        return View(new CheckoutViewModel());
    }

    // Siparişi oluştur
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        var userId = GetUserId();
        var cartItems = await GetCartItemsAsync(userId);
        if (!cartItems.Any())
            return RedirectToAction("Index", "Cart");

        foreach (var item in cartItems)
        {
            if (item.Quantity > item.Product!.Stock)
                ModelState.AddModelError("", $"{item.Product.Name} için yeterli stok yok (kalan: {item.Product.Stock}).");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.CartItems = cartItems;
            return View(model);
        }

        var order = new Order
        {
            UserId = userId,
            FullName = model.FullName,
            Phone = model.Phone,
            Address = model.Address,
            Status = "Hazırlanıyor",
            TotalPrice = cartItems.Sum(i => i.Product!.Price * i.Quantity)
        };

        foreach (var item in cartItems)
        {
            order.OrderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.Product!.Price
            });

            item.Product.Stock -= item.Quantity;
            _productRepository.Update(item.Product);

            _cartRepository.Delete(item);
        }

        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveAsync();

        return RedirectToAction(nameof(Success), new { id = order.Id });
    }

    // "Siparişin alındı" sayfası
    public async Task<IActionResult> Success(int id)
    {
        var userId = GetUserId();
        var order = (await _orderRepository.WhereAsync(o => o.Id == id && o.UserId == userId))
            .FirstOrDefault();

        if (order == null)
            return NotFound();

        return View(order);
    }

    // Siparişlerim
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        var orders = await _orderRepository.Query()
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedDate)
            .ToListAsync();
        return View(orders);
    }

    // Sipariş detayı
    public async Task<IActionResult> Details(int id)
    {
        var userId = GetUserId();
        var order = await _orderRepository.Query()
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

        if (order == null)
            return NotFound();

        return View(order);
    }
}