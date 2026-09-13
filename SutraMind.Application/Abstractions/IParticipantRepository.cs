using SutraMind.Domain.Entities;

namespace SutraMind.Application.Abstractions;

public interface IParticipantRepository
{
    Task AddAsync(Participant participant, CancellationToken cancellationToken = default);
    Task<Participant?> GetAsync(Guid id, CancellationToken cancellationToken = default);
}
