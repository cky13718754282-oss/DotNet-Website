using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Geekspace.Models;
using Microsoft.AspNetCore.Identity;

namespace Geekspace.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<LearningResource> LearningResources { get; set; }
    public DbSet<ResourceComment> ResourceComments { get; set; }
    public DbSet<UserLearningProgress> UserLearningProgresses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserLearningProgress>()
            .HasIndex(progress => new { progress.UserId, progress.LearningResourceId })
            .IsUnique();

        builder.Entity<UserLearningProgress>()
            .HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(progress => progress.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserLearningProgress>()
            .HasOne(progress => progress.LearningResource)
            .WithMany()
            .HasForeignKey(progress => progress.LearningResourceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
