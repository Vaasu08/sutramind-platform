namespace SutraMind.Domain.Entities;

public sealed class ConflictRecord
{
    public Guid ConflictId { get; init; } = Guid.NewGuid();
    public required string EntityType { get; init; }
    public required Guid EntityId { get; init; }
    public required string LocalPayloadJson { get; init; }
    public required string ServerPayloadJson { get; init; }
    public required string BasePayloadJson { get; init; }
    public long? ServerVersion { get; init; }
    public DateTimeOffset DetectedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public required string Resolution { get; set; }
    public Guid? ResolvedBy { get; set; }
    public DateTimeOffset? ResolvedAtUtc { get; set; }
    public string? ResolutionReason { get; set; }
}
