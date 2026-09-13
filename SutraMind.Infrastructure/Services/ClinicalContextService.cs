using Microsoft.EntityFrameworkCore;
using SutraMind.Application.Abstractions;
using SutraMind.Application.ReadModels;
using SutraMind.Domain.Enums;
using SutraMind.Infrastructure.Persistence;
using SutraMind.Infrastructure.Seed;

namespace SutraMind.Infrastructure.Services;

public sealed class ClinicalContextService(LocalDbContext context) : IClinicalContextService
{
    public async Task<ClinicalContextSnapshot?> GetDefaultContextAsync(CancellationToken cancellationToken = default)
    {
        var study = await context.Studies.AsNoTracking().OrderBy(item => item.StudyCode).FirstOrDefaultAsync(cancellationToken);
        if (study is null)
            return null;

        var site = await context.Sites.AsNoTracking()
            .Where(item => item.StudyId == study.Id && item.IsActive)
            .OrderBy(item => item.SiteCode)
            .FirstOrDefaultAsync(cancellationToken);

        if (site is null)
            return null;

        var protocol = await context.Protocols.AsNoTracking()
            .FirstOrDefaultAsync(item => item.StudyId == study.Id && item.IsActive, cancellationToken);
        var label = $"{study.StudyCode}  |  {protocol?.InterventionName ?? study.Title}";
        return new ClinicalContextSnapshot(study.Id, label, site.Id, site.Name, DemoIds.CoordinatorUserId, DemoIds.DeviceId);
    }
}

public sealed class OutboxService(LocalDbContext context) : IOutboxService
{
    public Task<int> CountPendingAsync(CancellationToken cancellationToken = default) =>
        context.OutboxOperations.AsNoTracking()
            .CountAsync(item => item.State == OperationState.Pending, cancellationToken);

    public Task<DateTimeOffset?> GetLastSyncAttemptUtcAsync(CancellationToken cancellationToken = default) =>
        context.SyncStates.AsNoTracking()
            .Select(item => (DateTimeOffset?)item.LastAttemptAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
}
