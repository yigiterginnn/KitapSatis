using KitapSatis.Api.Dtos;
using KitapSatis.Data.Entities;
using KitapSatis.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace KitapSatis.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly IRepository<Category> _categoryRepository;

    public CategoriesController(IRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    // GET /api/categories
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryRepository.GetAllAsync();
        var result = categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        });
        return Ok(result);
    }

    // GET /api/categories/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        return Ok(new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        });
    }

    // POST /api/categories
    [HttpPost]
    public async Task<IActionResult> Create(CategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description
        };

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveAsync();

        dto.Id = category.Id;
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, dto);
    }

    // PUT /api/categories/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CategoryDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        category.Name = dto.Name;
        category.Description = dto.Description;

        _categoryRepository.Update(category);
        await _categoryRepository.SaveAsync();
        return NoContent();
    }

    // DELETE /api/categories/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        _categoryRepository.Delete(category);
        await _categoryRepository.SaveAsync();
        return NoContent();
    }
}