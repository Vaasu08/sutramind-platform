namespace SutraMind.Domain.Entities;

public sealed class LocalSyncState
{
    public required Guid DeviceId { get; init; }
    public string? LastServerCursor { get; set; }
    public DateTimeOffset? LastSuccessAtUtc { get; set; }
    public DateTimeOffset? LastAttemptAtUtc { get; set; }
    public required string ConnectionState { get; set; }
}
