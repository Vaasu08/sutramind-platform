using SutraMind.Application.Contracts;
using SutraMind.Application.ReadModels;

namespace SutraMind.Application.Abstractions;

public interface IDashboardService
{
    Task<DashboardViewData> GetDashboardAsync(Guid studyId, CancellationToken cancellationToken = default);
}

public interface IStudyWorkspaceService
{
    Task<IReadOnlyList<StudyListItem>> ListStudiesAsync(CancellationToken cancellationToken = default);
    Task<StudyWorkspaceData?> GetStudyWorkspaceAsync(Guid studyId, CancellationToken cancellationToken = default);
}

public interface IParticipantReadService
{
    Task<IReadOnlyList<ParticipantListItem>> ListByStudyAsync(Guid studyId, string? search, CancellationToken cancellationToken = default);
}

public interface IVisitCrfReadService
{
    Task<IReadOnlyList<VisitListItem>> ListByStudyAsync(Guid studyId, CancellationToken cancellationToken = default);
}

public interface IQueryReadService
{
    Task<IReadOnlyList<QueryListItem>> ListByStudyAsync(Guid studyId, bool openOnly, CancellationToken cancellationToken = default);
}

public interface IEthicsReadService
{
    Task<EthicsReviewData?> GetByStudyAsync(Guid studyId, CancellationToken cancellationToken = default);
}

public interface IMasterDataReadService
{
    Task<IReadOnlyList<MasterTermListItem>> ListActiveAsync(string? category, CancellationToken cancellationToken = default);
}

public interface IOutboxService
{
    Task<int> CountPendingAsync(CancellationToken cancellationToken = default);
    Task<DateTimeOffset?> GetLastSyncAttemptUtcAsync(CancellationToken cancellationToken = default);
}

public interface IClinicalContextService
{
    Task<ClinicalContextSnapshot?> GetDefaultContextAsync(CancellationToken cancellationToken = default);
}

public sealed record ClinicalContextSnapshot(Guid StudyId, string StudyLabel, Guid SiteId, string SiteName, Guid ActorUserId, Guid DeviceId);
