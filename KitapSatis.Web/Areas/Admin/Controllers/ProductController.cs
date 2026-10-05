using KitapSatis.Web.Data.Repositories;
using KitapSatis.Web.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KitapSatis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductController : Controller
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Category> _categoryRepository;

    public ProductController(IRepository<Product> productRepository, IRepository<Category> categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    // LİSTELE (kategorisiyle birlikte)
    public async Task<IActionResult> Index()
    {
        var products = await _productRepository.Query()
            .Include(p => p.Category)
            .ToListAsync();
        return View(products);
    }

    // EKLE - formu göster
    public async Task<IActionResult> Create()
    {
        await LoadCategoriesAsync();
        return View();
    }

    // EKLE - kaydet
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync(product.CategoryId);
            return View(product);
        }

        await _productRepository.AddAsync(product);
        await _productRepository.SaveAsync();
        TempData["Message"] = "Ürün eklendi.";
        return RedirectToAction(nameof(Index));
    }

    // GÜNCELLE - formu dolu göster
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        await LoadCategoriesAsync(product.CategoryId);
        return View(product);
    }

    // GÜNCELLE - kaydet
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Product product)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync(product.CategoryId);
            return View(product);
        }

        var existing = await _productRepository.GetByIdAsync(product.Id);
        if (existing == null)
            return NotFound();

        existing.Name = product.Name;
        existing.Author = product.Author;
        existing.Description = product.Description;
        existing.Price = product.Price;
        existing.Stock = product.Stock;
        existing.ImageUrl = product.ImageUrl;
        existing.CategoryId = product.CategoryId;

        _productRepository.Update(existing);
        await _productRepository.SaveAsync();
        TempData["Message"] = "Ürün güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    // SİL - onay sayfası
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productRepository.Query()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
            return NotFound();

        return View(product);
    }

    // SİL - onaylandı
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        _productRepository.Delete(product);
        await _productRepository.SaveAsync();
        TempData["Message"] = "Ürün silindi.";
        return RedirectToAction(nameof(Index));
    }

    // Kategori listesini dropdown için hazırlar
    private async Task LoadCategoriesAsync(int? selectedId = null)
    {
        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedId);
    }
}