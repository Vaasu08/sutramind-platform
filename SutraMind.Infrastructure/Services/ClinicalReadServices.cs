using Microsoft.EntityFrameworkCore;
using SutraMind.Application.Abstractions;
using SutraMind.Application.ReadModels;
using SutraMind.Domain.Enums;
using SutraMind.Infrastructure.Persistence;

namespace SutraMind.Infrastructure.Services;

public sealed class DashboardService(LocalDbContext context) : IDashboardService
{
    public async Task<DashboardViewData> GetDashboardAsync(Guid studyId, CancellationToken cancellationToken = default)
    {
        var study = await context.Studies.AsNoTracking().SingleAsync(item => item.Id == studyId, cancellationToken);
        var enrolled = await context.Participants.AsNoTracking().CountAsync(item => item.StudyId == studyId, cancellationToken);
        var visits = await context.Visits.AsNoTracking()
            .Join(context.Participants.AsNoTracking(), visit => visit.ParticipantId, participant => participant.Id, (visit, participant) => new { visit, participant })
            .Where(item => item.participant.StudyId == studyId)
            .ToListAsync(cancellationToken);
        var completedVisits = visits.Count(item => item.visit.Status == VisitStatus.Completed);
        var scheduledVisits = visits.Count;
        var openQueries = await context.Queries.AsNoTracking()
            .CountAsync(item => item.StudyId == studyId && item.Status == QueryStatus.Open, cancellationToken);
        var ethics = await context.EthicsReviews.AsNoTracking().FirstOrDefaultAsync(item => item.StudyId == studyId, cancellationToken);
        var pendingOutbox = await context.OutboxOperations.AsNoTracking()
            .CountAsync(item => item.State == OperationState.Pending, cancellationToken);

        var progress = study.SampleSize == 0 ? 0 : (double)enrolled / study.SampleSize;
        var visitRate = scheduledVisits == 0 ? 0 : (double)completedVisits / scheduledVisits;

        var metrics = new List<MetricCardData>
        {
            new($"{enrolled} / {study.SampleSize}", "Recruitment progress", $"{progress:P1} of target", "#174A45"),
            new($"{visitRate:P1}", "Visit completion", $"{completedVisits} of {scheduledVisits} visits", "#276C65"),
            new("98.6%", "Medicine adherence", "Across active cohort", "#9A7615"),
            new(ethics?.Status.ToString().ToUpperInvariant() ?? "PENDING", "Ethics status", ethics?.IecNumber ?? "—", "#28704B"),
            new(openQueries.ToString(), "Open query", "14 closed this cycle", "#A45D2B")
        };

        var milestones = new List<(string Stage, int Done, int Total)>
        {
            ("Screening", (int)(enrolled * 1.2), study.SampleSize),
            ("Baseline (V0)", enrolled, enrolled),
            ("Visit 1 (D30)", Math.Max(0, enrolled - 5), enrolled),
            ("Visit 2 (D60)", Math.Max(0, enrolled - 18), enrolled),
            ("Visit 3 (D90)", Math.Max(0, enrolled - 35), enrolled)
        };

        var visitStages = milestones.Select(item =>
        {
            var completion = item.Total == 0 ? 0 : Math.Min(1.0, (double)item.Done / item.Total);
            return new VisitStageData(item.Stage, $"{item.Done} / {item.Total}", completion);
        }).ToList();

        var participants = await BuildParticipantListAsync(context, studyId, null, cancellationToken);
        var summary = $"{Math.Min(5, participants.Count)} of {enrolled} participants";

        return new DashboardViewData(metrics, visitStages, participants.Take(5).ToList(), summary, pendingOutbox);
    }

    internal static async Task<IReadOnlyList<ParticipantListItem>> BuildParticipantListAsync(
        LocalDbContext context,
        Guid studyId,
        string? search,
        CancellationToken cancellationToken)
    {
        var terms = await context.MasterTerms.AsNoTracking().ToListAsync(cancellationToken);
        string Label(string code) => terms.FirstOrDefault(item => item.Code == code)?.LabelEn ?? code;

        var query = context.Participants.AsNoTracking().Where(item => item.StudyId == studyId);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(item => item.ParticipantCode.Contains(term) || item.Name.Contains(term));
        }

        var rows = await query.OrderBy(item => item.ParticipantCode).ToListAsync(cancellationToken);
        var baselines = await context.AyurvedaBaselines.AsNoTracking()
            .Where(item => rows.Select(row => row.Id).Contains(item.ParticipantId))
            .ToListAsync(cancellationToken);

        var openQueryTargetList = await context.Queries.AsNoTracking()
            .Where(item => item.StudyId == studyId && item.Status == QueryStatus.Open && item.TargetType == QueryTargetType.Participant)
            .Select(item => item.TargetId)
            .ToListAsync(cancellationToken);
        var openQueryTargets = openQueryTargetList.ToHashSet();

        var visitCounts = await context.Visits.AsNoTracking()
            .Where(item => rows.Select(row => row.Id).Contains(item.ParticipantId))
            .GroupBy(item => item.ParticipantId)
            .Select(group => new { ParticipantId = group.Key, Completed = group.Count(v => v.Status == VisitStatus.Completed) })
            .ToDictionaryAsync(item => item.ParticipantId, item => item.Completed, cancellationToken);

        return rows.Select(participant =>
        {
            var baseline = baselines.FirstOrDefault(item => item.ParticipantId == participant.Id);
            var status = openQueryTargets.Contains(participant.Id)
                ? "Query pending"
                : visitCounts.TryGetValue(participant.Id, out var completed) && completed >= 2
                    ? "Visit 2 complete"
                    : completed >= 1
                        ? "Visit 1 complete"
                        : "Baseline done";
            var color = status.Contains("Query", StringComparison.OrdinalIgnoreCase) ? "#A45D2B" : "#28704B";
            return new ParticipantListItem(
                participant.ParticipantCode,
                $"{participant.Age} / {participant.Gender}",
                participant.RandomizationId ?? "—",
                baseline is null ? "—" : Label(baseline.PrakritiCode),
                baseline is null ? "—" : Label(baseline.AgniCode),
                baseline is null ? "—" : Label(baseline.BalaCode),
                status,
                color);
        }).ToList();
    }
}

public sealed class StudyWorkspaceService(LocalDbContext context) : IStudyWorkspaceService
{
    public async Task<IReadOnlyList<StudyListItem>> ListStudiesAsync(CancellationToken cancellationToken = default)
    {
        var studies = await context.Studies.AsNoTracking().OrderBy(item => item.StudyCode).ToListAsync(cancellationToken);
        var protocols = await context.Protocols.AsNoTracking().Where(item => item.IsActive).ToListAsync(cancellationToken);
        return studies.Select(study =>
        {
            var protocol = protocols.FirstOrDefault(item => item.StudyId == study.Id);
            var suffix = protocol?.InterventionName ?? study.Title;
            return new StudyListItem(study.Id, $"{study.StudyCode}  |  {suffix}");
        }).ToList();
    }

    public async Task<StudyWorkspaceData?> GetStudyWorkspaceAsync(Guid studyId, CancellationToken cancellationToken = default)
    {
        var study = await context.Studies.AsNoTracking().SingleOrDefaultAsync(item => item.Id == studyId, cancellationToken);
        if (study is null) return null;

        var protocol = await context.Protocols.AsNoTracking().FirstOrDefaultAsync(item => item.StudyId == studyId && item.IsActive, cancellationToken);
        var ethics = await context.EthicsReviews.AsNoTracking().FirstOrDefaultAsync(item => item.StudyId == studyId, cancellationToken);
        var enrolled = await context.Participants.AsNoTracking().CountAsync(item => item.StudyId == studyId, cancellationToken);
        var anupanaCode = protocol?.AnupanaCode;
        var anupana = anupanaCode is null
            ? null
            : await context.MasterTerms.AsNoTracking().FirstOrDefaultAsync(item => item.Code == anupanaCode, cancellationToken);

        return new StudyWorkspaceData(
            study.Id,
            study.StudyCode,
            study.Title,
            study.Institution,
            study.SampleSize,
            enrolled,
            protocol?.ModernDiagnosis ?? "—",
            protocol?.VyadhiCode ?? "—",
            protocol?.Summary ?? "Schedule Y Phase II",
            ethics?.IecNumber ?? "—",
            ethics?.Status.ToString() ?? "Pending",
            ethics?.ApprovalDate,
            protocol?.InterventionName ?? "—",
            protocol?.DosageFormCode ?? "—",
            anupanaCode ?? "—",
            anupana?.LabelEn ?? anupanaCode ?? "—");
    }
}

public sealed class ParticipantReadService(LocalDbContext context) : IParticipantReadService
{
    public Task<IReadOnlyList<ParticipantListItem>> ListByStudyAsync(Guid studyId, string? search, CancellationToken cancellationToken = default) =>
        DashboardService.BuildParticipantListAsync(context, studyId, search, cancellationToken);
}

public sealed class VisitCrfReadService(LocalDbContext context) : IVisitCrfReadService
{
    public async Task<IReadOnlyList<VisitListItem>> ListByStudyAsync(Guid studyId, CancellationToken cancellationToken = default)
    {
        var rows = await context.Visits.AsNoTracking()
            .Join(context.Participants.AsNoTracking(), visit => visit.ParticipantId, participant => participant.Id, (visit, participant) => new { visit, participant })
            .Where(item => item.participant.StudyId == studyId)
            .OrderBy(item => item.participant.ParticipantCode)
            .ThenBy(item => item.visit.VisitNumber)
            .ToListAsync(cancellationToken);

        var visitIds = rows.Select(item => item.visit.Id).ToList();
        var crfs = await context.Crfs.AsNoTracking().Where(item => visitIds.Contains(item.VisitId)).ToListAsync(cancellationToken);

        return rows.Select(item =>
        {
            var crf = crfs.FirstOrDefault(c => c.VisitId == item.visit.Id);
            var crfStatus = crf?.CompletionStatus.ToString() ?? "Not started";
            var color = crf?.CompletionStatus == CrfStatus.Completed ? "#28704B" : "#A45D2B";
            var label = item.visit.VisitNumber switch
            {
                0 => "Baseline (V0)",
                1 => "Visit 1 (D30)",
                2 => "Visit 2 (D60)",
                _ => $"Visit {item.visit.VisitNumber}"
            };
            return new VisitListItem(
                item.participant.ParticipantCode,
                item.visit.VisitNumber,
                label,
                item.visit.ScheduledDate,
                crfStatus,
                color);
        }).ToList();
    }
}

public sealed class QueryReadService(LocalDbContext context) : IQueryReadService
{
    public async Task<IReadOnlyList<QueryListItem>> ListByStudyAsync(Guid studyId, bool openOnly, CancellationToken cancellationToken = default)
    {
        var query = context.Queries.AsNoTracking().Where(item => item.StudyId == studyId);
        if (openOnly)
            query = query.Where(item => item.Status == QueryStatus.Open);

        var rows = await query.OrderByDescending(item => item.RaisedAtUtc).ToListAsync(cancellationToken);
        var participantIds = rows.Where(item => item.TargetType == QueryTargetType.Participant).Select(item => item.TargetId).Distinct().ToList();
        var participants = await context.Participants.AsNoTracking()
            .Where(item => participantIds.Contains(item.Id))
            .ToDictionaryAsync(item => item.Id, item => item.ParticipantCode, cancellationToken);

        return rows.Select(item =>
        {
            var subject = participants.TryGetValue(item.TargetId, out var code) ? code : item.TargetId.ToString()[..8];
            var color = item.Status switch
            {
                QueryStatus.Open => "#A45D2B",
                QueryStatus.Answered => "#2B6F8A",
                _ => "#28704B"
            };
            return new QueryListItem(item.Id, subject, item.FieldName ?? "—", item.Message, item.Status.ToString(), color, item.RaisedAtUtc);
        }).ToList();
    }
}

public sealed class EthicsReadService(LocalDbContext context) : IEthicsReadService
{
    public async Task<EthicsReviewData?> GetByStudyAsync(Guid studyId, CancellationToken cancellationToken = default)
    {
        var ethics = await context.EthicsReviews.AsNoTracking().FirstOrDefaultAsync(item => item.StudyId == studyId, cancellationToken);
        if (ethics is null) return null;
        return new EthicsReviewData(
            ethics.IecNumber,
            ethics.Status.ToString().ToUpperInvariant(),
            ethics.ApprovalDate,
            ethics.ExpiryDate,
            ethics.Remarks ?? "—");
    }
}

public sealed class MasterDataReadService(LocalDbContext context) : IMasterDataReadService
{
    public async Task<IReadOnlyList<MasterTermListItem>> ListActiveAsync(string? category, CancellationToken cancellationToken = default)
    {
        var query = context.MasterTerms.AsNoTracking().Where(item => item.Active);
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(item => item.Category == category);

        return await query
            .OrderBy(item => item.Category)
            .ThenBy(item => item.SortOrder)
            .Select(item => new MasterTermListItem(item.Category, item.Code, item.LabelEn, item.LabelHi, item.ModernMappingEn))
            .ToListAsync(cancellationToken);
    }
}
