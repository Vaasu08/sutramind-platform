namespace SutraMind.Domain.Entities;

public sealed class Medicine : SyncEntity
{
    public required string MedicineCode { get; set; }
    public required string AyurvedaName { get; set; }
    public required string DosageFormCode { get; set; }
    public string? WhoDrugCode { get; set; }
    public bool IsActive { get; set; } = true;
}
