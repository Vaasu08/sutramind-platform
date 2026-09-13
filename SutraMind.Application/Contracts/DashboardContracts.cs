namespace SutraMind.Application.Contracts;

public sealed record DashboardSummary(int EnrolledParticipants, int SampleTarget, int CompletedVisits, int ScheduledVisits, decimal AdherencePercent, int OpenQueries, string EthicsStatus, IReadOnlyDictionary<string, int> PrakritiDistribution, IReadOnlyDictionary<string, int> VyadhiDistribution);
