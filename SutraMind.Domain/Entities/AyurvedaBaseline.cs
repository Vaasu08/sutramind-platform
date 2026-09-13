namespace SutraMind.Domain.Entities;

public sealed class AyurvedaBaseline : SyncEntity
{
    public required Guid ParticipantId { get; set; }
    public required string PrakritiCode { get; set; }
    public string? VikritiNotes { get; set; }
    public required string AgniCode { get; set; }
    public required string BalaCode { get; set; }
    public required string SatvaCode { get; set; }
    public Guid RecordedBy { get; set; }
    public DateTimeOffset RecordedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
