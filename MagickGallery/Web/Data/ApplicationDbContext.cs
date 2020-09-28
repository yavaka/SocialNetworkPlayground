namespace Web.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Web.Models;

    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {}
        
        public DbSet<MockUser> MockUsers { get; set; }

        public DbSet<OriginalImage> OriginalImages{ get; set; }
        public DbSet<ThumbnailImage> ThumbnailImages { get; set; }
        public DbSet<MediumImage> MediumImages { get; set; }
    }
}
