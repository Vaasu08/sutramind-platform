using SutraMind.Domain.Enums;

namespace SutraMind.Application.Contracts;

public sealed record CreateStudyRequest(string StudyCode, string Title, string Institution, int SampleSize, DateOnly StartDate, DateOnly EndDate, ProtocolInput Protocol, EthicsInput Ethics);
public sealed record ProtocolInput(string Version, string ModernDiagnosis, string VyadhiCode, Guid? MedicineId, string DosageFormCode, string AnupanaCode, int TreatmentDurationDays);
public sealed record EthicsInput(string IecNumber, EthicsStatus Status);
public sealed record StudySummary(Guid Id, string StudyCode, string Title, string Institution, int SampleSize, StudyStatus Status, DateOnly StartDate, DateOnly EndDate);
