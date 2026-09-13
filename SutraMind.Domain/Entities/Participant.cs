using SutraMind.Domain.Enums;

namespace SutraMind.Domain.Entities;

public sealed class Participant : SyncEntity
{
    public required string ParticipantCode { get; init; }
    public required Guid StudyId { get; init; }
    public required Guid SiteId { get; init; }
    public required string Name { get; init; }
    public int Age { get; init; }
    public required string Gender { get; init; }
    public required string ModernDiagnosis { get; init; }
    public required string VyadhiCode { get; init; }
    public int? DiseaseDurationMonths { get; init; }
    public string? RandomizationId { get; init; }
    public DateOnly EnrollmentDate { get; init; }
    public ParticipantStatus Status { get; set; } = ParticipantStatus.Draft;
    public Guid CreatedBy { get; init; }
}
