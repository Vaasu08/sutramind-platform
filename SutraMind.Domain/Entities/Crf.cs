using SutraMind.Domain.Enums;

namespace SutraMind.Domain.Entities;

public sealed class Crf : SyncEntity
{
    public required Guid VisitId { get; set; }
    public int? SystolicBp { get; set; }
    public int? DiastolicBp { get; set; }
    public int? PulseBpm { get; set; }
    public decimal? WeightKg { get; set; }
    public decimal? TemperatureC { get; set; }
    public string? AgniCode { get; set; }
    public string? BalaCode { get; set; }
    public string? Symptoms { get; set; }
    public Guid? MedicineId { get; set; }
    public string? Dose { get; set; }
    public string? Frequency { get; set; }
    public string? Compliance { get; set; }
    public string? Remarks { get; set; }
    public CrfStatus CompletionStatus { get; set; } = CrfStatus.Draft;
    public Guid? CompletedBy { get; set; }
    public DateTimeOffset? CompletedAtUtc { get; set; }
}
