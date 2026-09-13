namespace SutraMind.Domain.Entities;

public sealed class MasterTerm : SyncEntity
{
    public required string Category { get; set; }
    public required string Code { get; set; }
    public required string LabelEn { get; set; }
    public required string LabelHi { get; set; }
    public string? ModernMappingEn { get; set; }
    public bool Active { get; set; } = true;
    public int SortOrder { get; set; }
}
