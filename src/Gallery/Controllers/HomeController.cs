using System.Diagnostics;
using Gallery.Database;
using Gallery.Models.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gallery.Controllers;

public sealed class HomeController(IDbContextFactory<GalleryContext> contextFactory) : Controller
{
    public async Task<IActionResult> IndexAsync(CancellationToken cancellationToken)
    {
        await using GalleryContext context = await contextFactory.CreateDbContextAsync(cancellationToken);
        List<Image> pictures = await context.Images.ToListAsync(cancellationToken);
        return View(new HomeViewModel { Pictures = pictures });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
