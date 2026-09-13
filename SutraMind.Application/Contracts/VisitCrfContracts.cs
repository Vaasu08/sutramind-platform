using SutraMind.Domain.Enums;

namespace SutraMind.Application.Contracts;

public sealed record ScheduleVisitRequest(Guid ParticipantId, int VisitNumber, DateOnly ScheduledDate, Guid? InvestigatorId);
public sealed record SaveCrfRequest(Guid VisitId, DateOnly? VisitDate, Guid InvestigatorId, int? SystolicBp, int? DiastolicBp, int? PulseBpm, decimal? WeightKg, decimal? TemperatureC, string? AgniCode, string? BalaCode, string? Symptoms, Guid? MedicineId, string? Dose, string? Frequency, string? Compliance, string? Remarks, CrfStatus CompletionStatus);
