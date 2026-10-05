using KitapSatis.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KitapSatis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public UserController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    // Kullanıcı listesi
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var model = new List<UserViewModel>();

        foreach (var user in users)
        {
            model.Add(new UserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? "",
                IsAdmin = await _userManager.IsInRoleAsync(user, "Admin")
            });
        }

        return View(model);
    }

    // Admin yap / Adminliği kaldır
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAdmin(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        if (user.Email == User.Identity?.Name)
        {
            TempData["Error"] = "Kendi admin yetkini kaldıramazsın.";
            return RedirectToAction(nameof(Index));
        }

        if (await _userManager.IsInRoleAsync(user, "Admin"))
            await _userManager.RemoveFromRoleAsync(user, "Admin");
        else
            await _userManager.AddToRoleAsync(user, "Admin");

        TempData["Message"] = "Kullanıcı yetkisi güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    // Kullanıcı sil
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        if (user.Email == User.Identity?.Name)
        {
            TempData["Error"] = "Kendi hesabını silemezsin.";
            return RedirectToAction(nameof(Index));
        }

        await _userManager.DeleteAsync(user);
        TempData["Message"] = "Kullanıcı silindi.";
        return RedirectToAction(nameof(Index));
    }
}