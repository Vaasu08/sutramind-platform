namespace SutraMind.Domain.Entities;

public sealed class Site : SyncEntity
{
    public required Guid StudyId { get; set; }
    public required string SiteCode { get; set; }
    public required string Name { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
}
