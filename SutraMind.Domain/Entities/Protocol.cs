namespace SutraMind.Domain.Entities;

public sealed class Protocol : SyncEntity
{
    public required Guid StudyId { get; set; }
    public required string Version { get; set; }
    public bool IsActive { get; set; } = true;
    public required string ModernDiagnosis { get; set; }
    public required string VyadhiCode { get; set; }
    public required string InterventionName { get; set; }
    public Guid? MedicineId { get; set; }
    public required string DosageFormCode { get; set; }
    public required string AnupanaCode { get; set; }
    public int TreatmentDurationDays { get; set; }
    public string? Summary { get; set; }
    public Guid CreatedBy { get; set; }
}
