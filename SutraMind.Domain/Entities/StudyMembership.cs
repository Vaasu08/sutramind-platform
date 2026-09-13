namespace SutraMind.Domain.Entities;

public sealed class StudyMembership : SyncEntity
{
    public required Guid StudyId { get; set; }
    public required Guid UserId { get; set; }
    public Guid? SiteId { get; set; }
    public required string RoleInStudy { get; set; }
    public bool IsActive { get; set; } = true;
}
