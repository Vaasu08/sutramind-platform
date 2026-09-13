namespace SutraMind.Application.ReadModels;

public sealed record MetricCardData(string Value, string Label, string Detail, string Accent);
public sealed record VisitStageData(string Stage, string Progress, double Completion);
public sealed record ParticipantListItem(string Code, string Demographic, string Randomization, string Prakriti, string Agni, string Bala, string Status, string StatusColor);

public sealed record DashboardViewData(
    IReadOnlyList<MetricCardData> Metrics,
    IReadOnlyList<VisitStageData> VisitStages,
    IReadOnlyList<ParticipantListItem> Participants,
    string ParticipantSummary,
    int PendingOutboxCount);

public sealed record StudyListItem(Guid Id, string Label);
public sealed record StudyWorkspaceData(
    Guid StudyId,
    string StudyCode,
    string Title,
    string Institution,
    int SampleSize,
    int EnrolledCount,
    string ModernDiagnosis,
    string VyadhiCode,
    string MedDraNote,
    string IecNumber,
    string EthicsDecision,
    DateOnly? ApprovalDate,
    string InterventionName,
    string DosageFormCode,
    string AnupanaCode,
    string AnupanaLabel);

public sealed record VisitListItem(
    string ParticipantCode,
    int VisitNumber,
    string VisitLabel,
    DateOnly? ScheduledDate,
    string CrfStatus,
    string StatusColor);

public sealed record QueryListItem(
    Guid Id,
    string SubjectCode,
    string FieldName,
    string Message,
    string Status,
    string StatusColor,
    DateTimeOffset RaisedAtUtc);

public sealed record EthicsReviewData(
    string IecNumber,
    string Decision,
    DateOnly? ApprovalDate,
    DateOnly? ExpiryDate,
    string Remarks);

public sealed record MasterTermListItem(string Category, string Code, string LabelEn, string LabelHi, string? ModernMappingEn);
