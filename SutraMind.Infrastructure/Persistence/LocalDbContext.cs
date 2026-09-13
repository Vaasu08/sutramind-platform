using Microsoft.EntityFrameworkCore;
using SutraMind.Domain.Entities;

namespace SutraMind.Infrastructure.Persistence;

public sealed class LocalDbContext(DbContextOptions<LocalDbContext> options) : DbContext(options)
{
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<OutboxOperation> OutboxOperations => Set<OutboxOperation>();
    public DbSet<LocalSyncState> SyncStates => Set<LocalSyncState>();
    public DbSet<ConflictRecord> ConflictRecords => Set<ConflictRecord>();
    public DbSet<Study> Studies => Set<Study>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<StudyMembership> StudyMemberships => Set<StudyMembership>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Protocol> Protocols => Set<Protocol>();
    public DbSet<EthicsReview> EthicsReviews => Set<EthicsReview>();
    public DbSet<Medicine> Medicines => Set<Medicine>();
    public DbSet<MasterTerm> MasterTerms => Set<MasterTerm>();
    public DbSet<AyurvedaBaseline> AyurvedaBaselines => Set<AyurvedaBaseline>();
    public DbSet<Visit> Visits => Set<Visit>();
    public DbSet<Crf> Crfs => Set<Crf>();
    public DbSet<DataQuery> Queries => Set<DataQuery>();
    public DbSet<Milestone> Milestones => Set<Milestone>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Participant>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.HasIndex(item => new { item.StudyId, item.ParticipantCode }).IsUnique();
            entity.HasIndex(item => new { item.StudyId, item.RandomizationId }).IsUnique().HasFilter("RandomizationId IS NOT NULL");
            entity.Property(item => item.SyncState).HasConversion<string>();
            entity.Property(item => item.Status).HasConversion<string>();
        });

        modelBuilder.Entity<OutboxOperation>(entity =>
        {
            entity.HasKey(item => item.OperationId);
            entity.Property(item => item.Operation).HasConversion<string>();
            entity.Property(item => item.State).HasConversion<string>();
            entity.HasIndex(item => new { item.State, item.NextAttemptAtUtc });
        });

        modelBuilder.Entity<LocalSyncState>().HasKey(item => item.DeviceId);
        modelBuilder.Entity<ConflictRecord>().HasKey(item => item.ConflictId);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(type => typeof(SyncEntity).IsAssignableFrom(type.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType).Property<DateTimeOffset>(nameof(SyncEntity.CreatedAtUtc));
            modelBuilder.Entity(entityType.ClrType).Property<DateTimeOffset>(nameof(SyncEntity.UpdatedAtUtc));
        }
    }
}
