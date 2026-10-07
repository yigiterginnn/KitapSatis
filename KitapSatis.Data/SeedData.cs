using KitapSatis.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KitapSatis.Data;

public static class SeedData
{
    public static async Task CreateAdminAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        var adminEmail = "admin@kitapsatis.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin123!");
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
            await userManager.AddToRoleAsync(admin, "Admin");
    }

    public static async Task SeedBooksAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // 1) Kategoriler (yoksa ekle)
        var categoryNames = new[] { "Roman", "Bilim Kurgu", "Tarih", "Kişisel Gelişim", "Klasikler" };
        foreach (var name in categoryNames)
        {
            if (!await context.Categories.AnyAsync(c => c.Name == name))
                context.Categories.Add(new Category { Name = name });
        }
        await context.SaveChangesAsync();

        var categories = (await context.Categories.ToListAsync())
            .GroupBy(c => c.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

        // 2) Kitaplar (yoksa ekle)
        var books = new List<(string Name, string Author, string Category, decimal Price, int Stock, string Isbn, string Description)>
        {
            ("Simyacı", "Paulo Coelho", "Roman", 165.00m, 15, "9780062315007",
                "Hazinesini bulmak için yola çıkan genç bir çobanın, yolculuk boyunca kendini keşfetmesini anlatan dünyaca ünlü roman."),
            ("Bülbülü Öldürmek", "Harper Lee", "Roman", 210.00m, 10, "9780061120084",
                "Küçük bir kasabada geçen, adalet ve önyargı üzerine bir çocuğun gözünden anlatılan unutulmaz bir hikaye."),
            ("1984", "George Orwell", "Bilim Kurgu", 185.00m, 20, "9780451524935",
                "Her şeyin gözetlendiği bir gelecekte, düşünmenin bile suç sayıldığı bir dünyayı anlatan distopya klasiği."),
            ("Dune", "Frank Herbert", "Bilim Kurgu", 395.00m, 8, "9780441172719",
                "Çöl gezegeni Arrakis'te güç, siyaset ve hayatta kalma mücadelesini anlatan bilim kurgunun en büyük destanlarından biri."),
            ("Fahrenheit 451", "Ray Bradbury", "Bilim Kurgu", 175.00m, 12, "9781451673319",
                "Kitapların yasaklandığı ve yakıldığı bir toplumda, bir itfaiyecinin sorgulamaya başlamasını anlatan roman."),
            ("Sapiens", "Yuval Noah Harari", "Tarih", 340.00m, 14, "9780062316097",
                "İnsan türünün taş devrinden günümüze nasıl dünyaya hakim olduğunu anlatan, sürükleyici bir tarih kitabı."),
            ("Tüfek, Mikrop ve Çelik", "Jared Diamond", "Tarih", 320.00m, 6, "9780393317558",
                "Toplumların neden farklı hızlarda geliştiğini coğrafya ve doğa üzerinden açıklayan ödüllü bir inceleme."),
            ("Atomik Alışkanlıklar", "James Clear", "Kişisel Gelişim", 230.00m, 25, "9780735211292",
                "Küçük değişikliklerin zamanla büyük sonuçlar doğurduğunu gösteren, iyi alışkanlık edinmenin pratik rehberi."),
            ("Düşün ve Zengin Ol", "Napoleon Hill", "Kişisel Gelişim", 145.00m, 18, "9781585424337",
                "Başarılı insanların ortak düşünce alışkanlıklarını inceleyen, kişisel gelişim türünün en bilinen kitaplarından."),
            ("Suç ve Ceza", "Fyodor Dostoyevski", "Klasikler", 195.00m, 11, "9780140449136",
                "İşlediği suçun ardından vicdanıyla hesaplaşan genç bir öğrencinin psikolojisini anlatan dünya klasiği."),
            ("Sefiller", "Victor Hugo", "Klasikler", 285.00m, 7, "9780140444308",
                "Jean Valjean'ın hayatı üzerinden adalet, merhamet ve insanlık üzerine yazılmış büyük bir klasik."),
            ("Küçük Prens", "Antoine de Saint-Exupéry", "Klasikler", 95.00m, 30, "9780156012195",
                "Başka bir gezegenden gelen küçük bir prensin gözünden sevgi, dostluk ve büyümeyi anlatan zamansız bir masal.")
        };

        foreach (var b in books)
        {
            if (!await context.Products.AnyAsync(p => p.Name == b.Name))
            {
                context.Products.Add(new Product
                {
                    Name = b.Name,
                    Author = b.Author,
                    Description = b.Description,
                    Price = b.Price,
                    Stock = b.Stock,
                    ImageUrl = $"https://covers.openlibrary.org/b/isbn/{b.Isbn}-L.jpg",
                    CategoryId = categories[b.Category]
                });
            }
        }
        await context.SaveChangesAsync();
    }
}