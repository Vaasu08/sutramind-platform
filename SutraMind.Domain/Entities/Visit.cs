using SutraMind.Domain.Enums;

namespace SutraMind.Domain.Entities;

public sealed class Visit : SyncEntity
{
    public required Guid ParticipantId { get; set; }
    public int VisitNumber { get; set; }
    public DateOnly? VisitDate { get; set; }
    public DateOnly? ScheduledDate { get; set; }
    public DateOnly? NextVisitDate { get; set; }
    public VisitStatus Status { get; set; } = VisitStatus.Scheduled;
    public Guid? InvestigatorId { get; set; }
    public Guid CreatedBy { get; set; }
}
