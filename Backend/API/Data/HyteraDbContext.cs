using HyteraAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HyteraAPI.Data;

public class HyteraDbContext : DbContext
{
    public HyteraDbContext(DbContextOptions<HyteraDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<GameScore> GameScores { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<VoiceSet> VoiceSets { get; set; }
    public DbSet<AppVersion> AppVersions { get; set; }
    public DbSet<AppRoc> AppRocs { get; set; }
    public DbSet<GptPrompt> GptPrompts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.BPCode).HasMaxLength(50);
            entity.Property(e => e.BPName).HasMaxLength(255);
        });

        // Item configuration
        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("Items");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ItemCode).HasMaxLength(50);
            entity.Property(e => e.ItemName).HasMaxLength(255);
            entity.Property(e => e.ItemType).HasMaxLength(50);
            entity.Property(e => e.MSRP).HasColumnType("decimal(18,2)");
            entity.Property(e => e.DealerPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ItemImageUrl).HasMaxLength(500);
            entity.Property(e => e.ItemLinkUrl).HasMaxLength(500);
        });

        // GameScore configuration
        modelBuilder.Entity<GameScore>(entity =>
        {
            entity.ToTable("GameScores");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AppId).HasMaxLength(50);
            entity.Property(e => e.FunTimeId).HasMaxLength(50);
            entity.Property(e => e.Score12).HasMaxLength(20);
            entity.Property(e => e.Score34).HasMaxLength(20);
            entity.Property(e => e.GPSLat).HasMaxLength(50);
            entity.Property(e => e.GPSLng).HasMaxLength(50);
        });

        // Asset configuration
        modelBuilder.Entity<Asset>(entity =>
        {
            entity.ToTable("Assets");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ShowName).HasMaxLength(255);
            entity.Property(e => e.PhysicalPath).HasMaxLength(500);
            entity.Property(e => e.PhysicalName).HasMaxLength(255);
            entity.Property(e => e.ContentType).HasMaxLength(100);
        });

        // Language configuration
        modelBuilder.Entity<Language>(entity =>
        {
            entity.ToTable("Languages");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LanguageCode).HasMaxLength(20);
            entity.Property(e => e.LanguageName).HasMaxLength(100);
            entity.Property(e => e.IndexFileUrl).HasMaxLength(500);
        });

        // VoiceSet configuration
        modelBuilder.Entity<VoiceSet>(entity =>
        {
            entity.ToTable("VoiceSets");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.VoiceSetCode).HasMaxLength(50);
            entity.Property(e => e.VoiceSetName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Version).HasMaxLength(20);
            entity.Property(e => e.LanguageCode).HasMaxLength(20);
        });

        // AppVersion configuration
        modelBuilder.Entity<AppVersion>(entity =>
        {
            entity.ToTable("AppVersions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AppId).HasMaxLength(50);
            entity.Property(e => e.OS).HasMaxLength(50);
            entity.Property(e => e.Version).HasMaxLength(20);
        });

        // AppRoc configuration
        modelBuilder.Entity<AppRoc>(entity =>
        {
            entity.ToTable("AppRocs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AppId).HasMaxLength(50);
            entity.Property(e => e.FunTimeId).HasMaxLength(50);
            entity.Property(e => e.RocId).HasMaxLength(50);
        });

        // GptPrompt configuration
        modelBuilder.Entity<GptPrompt>(entity =>
        {
            entity.ToTable("GptPrompts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QueryType).HasMaxLength(50);
            entity.Property(e => e.GptModel).HasMaxLength(50);
        });
    }
}
