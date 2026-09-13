using SutraMind.Domain.Enums;

namespace SutraMind.Domain.Entities;

public sealed class EthicsReview : SyncEntity
{
    public required Guid StudyId { get; set; }
    public required string IecNumber { get; set; }
    public EthicsStatus Status { get; set; } = EthicsStatus.Pending;
    public DateOnly? ApprovalDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public string? Remarks { get; set; }
    public Guid? UpdatedBy { get; set; }
}
