using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gallery.Database;

public class GalleryContext(DbContextOptions<GalleryContext> options) : IdentityDbContext<User, Role, Guid>(options)
{
    public DbSet<Image> Images { get; set; }
}
