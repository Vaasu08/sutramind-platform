using SutraMind.Domain.Enums;

namespace SutraMind.Domain.Entities;

public sealed class Milestone : SyncEntity
{
    public required Guid StudyId { get; set; }
    public required string Name { get; set; }
    public DateOnly TargetDate { get; set; }
    public int? TargetCount { get; set; }
    public int? ActualCount { get; set; }
    public required string Status { get; set; }
}
