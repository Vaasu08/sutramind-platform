using SutraMind.Application.Abstractions;
using SutraMind.Application.Contracts;
using SutraMind.Domain.Entities;
using SutraMind.Domain.Enums;
using SutraMind.Infrastructure.Persistence;

namespace SutraMind.Infrastructure.Services;

public sealed class VisitScheduleService(LocalDbContext context) : IVisitScheduleService
{
    public async Task ScheduleAsync(ScheduleVisitRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        context.Visits.Add(new Visit
        {
            ParticipantId = request.ParticipantId,
            VisitNumber = request.VisitNumber,
            ScheduledDate = request.ScheduledDate,
            Status = VisitStatus.Scheduled,
            InvestigatorId = request.InvestigatorId,
            CreatedBy = createdBy
        });
        await context.SaveChangesAsync(cancellationToken);
    }
}
