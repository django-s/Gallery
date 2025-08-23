using Gallery.Database;
using Gallery.Models.Image;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gallery.Controllers;

[Authorize(Roles = "Admin")]
public sealed class ImageController : Controller
{
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateImageViewModel model, [FromServices] Configuration configuration,
        [FromServices] IDbContextFactory<GalleryContext> dbContextFactory, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string extension = Path.GetExtension(model.ImageFile.FileName);
        string name = $"{Guid.NewGuid()}{extension}";
        string filePath = Path.Combine(configuration.ImageDirectory, name);

        if (model.ImageFile.Length > 0)
        {
            await using FileStream stream = new(filePath, FileMode.Create);
            await model.ImageFile.CopyToAsync(stream, cancellationToken);
        }

        await using GalleryContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        context.Images.Add(new Image { Name = model.Name, Year = model.Year, Url = name, FilePath = filePath });

        await context.SaveChangesAsync(cancellationToken);

        return RedirectToAction(nameof(Create));
    }

    public async Task<IActionResult> Delete([FromServices] IDbContextFactory<GalleryContext> dbContextFactory,
        CancellationToken cancellationToken)
    {
        await using GalleryContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        List<Image> images = await context.Images.ToListAsync(cancellationToken);

        DeleteImagesViewModel model = new()
        {
            Images = images,
            DoDelete = images.Select(_ => false).ToList(),
            ImageIds = images.Select(x => x.Id).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete([FromServices] IDbContextFactory<GalleryContext> dbContextFactory,
        DeleteImagesViewModel model, CancellationToken cancellationToken)
    {
        if (model.ImageIds.Count != model.DoDelete.Count)
        {
            return RedirectToAction(nameof(Delete));
        }

        await using GalleryContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        List<Guid> imageIdsToDelete = model.ImageIds.Zip(model.DoDelete, Tuple.Create).Where(x => x.Item2)
            .Select(x => x.Item1).ToList();
        List<Image> imagesToDelete = await context.Images.Where(x => imageIdsToDelete.Contains(x.Id))
            .ToListAsync(cancellationToken);

        foreach (Image image in imagesToDelete)
        {
            System.IO.File.Delete(image.FilePath);
        }

        context.Images.RemoveRange(imagesToDelete);

        await context.SaveChangesAsync(cancellationToken);

        return RedirectToAction(nameof(Delete));
    }
}
