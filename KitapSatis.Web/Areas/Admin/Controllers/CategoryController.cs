using KitapSatis.Data.Repositories;
using KitapSatis.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitapSatis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoryController : Controller
{
    private readonly IRepository<Category> _categoryRepository;

    public CategoryController(IRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    // LİSTELE
    public async Task<IActionResult> Index()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return View(categories);
    }

    // EKLE - formu göster
    public IActionResult Create()
    {
        return View();
    }

    // EKLE - formdan gelen veriyi kaydet
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (!ModelState.IsValid)
            return View(category);

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveAsync();
        TempData["Message"] = "Kategori eklendi.";
        return RedirectToAction(nameof(Index));
    }

    // GÜNCELLE - formu dolu göster
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        return View(category);
    }

    // GÜNCELLE - değişiklikleri kaydet
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Category category)
    {
        if (!ModelState.IsValid)
            return View(category);

        var existing = await _categoryRepository.GetByIdAsync(category.Id);
        if (existing == null)
            return NotFound();

        existing.Name = category.Name;
        existing.Description = category.Description;

        _categoryRepository.Update(existing);
        await _categoryRepository.SaveAsync();
        TempData["Message"] = "Kategori güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    // SİL - onay sayfası
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        return View(category);
    }

    // SİL - onaylandı, sil
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        _categoryRepository.Delete(category);
        await _categoryRepository.SaveAsync();
        TempData["Message"] = "Kategori silindi.";
        return RedirectToAction(nameof(Index));
    }
}