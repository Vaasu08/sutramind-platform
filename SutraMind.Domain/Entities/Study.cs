using SutraMind.Domain.Enums;

namespace SutraMind.Domain.Entities;

public sealed class Study : SyncEntity
{
    public required string StudyCode { get; set; }
    public required string Title { get; set; }
    public string? ShortTitle { get; set; }
    public required Guid PrincipalInvestigatorId { get; set; }
    public required string Institution { get; set; }
    public string? TrialPhase { get; set; }
    public int SampleSize { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public StudyStatus Status { get; set; } = StudyStatus.Draft;
    public string? CtriNumber { get; set; }
}
