using KitapSatis.Web.Data.Repositories;
using KitapSatis.Web.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KitapSatis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class OrderController : Controller
{
    private readonly IRepository<Order> _orderRepository;

    public OrderController(IRepository<Order> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    // Tüm siparişler (en yeni en üstte)
    public async Task<IActionResult> Index()
    {
        var orders = await _orderRepository.Query()
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.CreatedDate)
            .ToListAsync();
        return View(orders);
    }

    // Sipariş detayı (ürünleriyle birlikte)
    public async Task<IActionResult> Details(int id)
    {
        var order = await _orderRepository.Query()
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return NotFound();

        return View(order);
    }

    // Sipariş durumunu güncelle
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
            return NotFound();

        order.Status = status;
        _orderRepository.Update(order);
        await _orderRepository.SaveAsync();

        TempData["Message"] = "Sipariş durumu güncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }
}