using SutraMind.Application.Contracts;

namespace SutraMind.Application.Abstractions;

public interface IVisitScheduleService
{
    Task ScheduleAsync(ScheduleVisitRequest request, Guid createdBy, CancellationToken cancellationToken = default);
}
