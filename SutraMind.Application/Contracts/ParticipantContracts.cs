using SutraMind.Domain.Enums;

namespace SutraMind.Application.Contracts;

public sealed record EnrollParticipantRequest(string ParticipantCode, Guid StudyId, Guid SiteId, string Name, int Age, string Gender, string ModernDiagnosis, string VyadhiCode, int? DiseaseDurationMonths, string? RandomizationId, DateOnly EnrollmentDate, AyurvedaBaselineInput Baseline, Guid CreatedBy);
public sealed record AyurvedaBaselineInput(string PrakritiCode, string? VikritiNotes, string AgniCode, string BalaCode, string SatvaCode);
public sealed record ParticipantSummary(Guid Id, string ParticipantCode, Guid StudyId, Guid SiteId, int Age, string Gender, string ModernDiagnosis, string VyadhiCode, ParticipantStatus Status, SyncState SyncState);
