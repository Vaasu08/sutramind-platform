using SutraMind.Domain.Enums;

namespace SutraMind.Domain.Entities;

public abstract class SyncEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public SyncState SyncState { get; set; } = SyncState.Pending;
    public long? ServerVersion { get; set; }
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public Guid? LastModifiedBy { get; set; }
    public Guid? SourceDeviceId { get; set; }
}
