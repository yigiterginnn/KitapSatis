using System.Diagnostics;
using KitapSatis.Web.Data.Repositories;
using KitapSatis.Web.Entities;
using KitapSatis.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KitapSatis.Web.Controllers;

public class HomeController : Controller
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Category> _categoryRepository;

    public HomeController(IRepository<Product> productRepository, IRepository<Category> categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    // Ana sayfa: ürün listesi + kategori filtresi + arama + sıralama
    public async Task<IActionResult> Index(int? categoryId, string? search, string? sort)
    {
        IQueryable<Product> query = _productRepository.Query().Include(p => p.Category);

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) || p.Author.Contains(search));

        query = sort switch
        {
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            _ => query.OrderByDescending(p => p.CreatedDate)
        };

        ViewBag.Categories = await _categoryRepository.GetAllAsync();
        ViewBag.SelectedCategoryId = categoryId;
        ViewBag.Search = search;
        ViewBag.Sort = sort;

        var products = await query.ToListAsync();
        return View(products);
    }

    // Ürün detay sayfası
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productRepository.Query()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        return View(product);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}