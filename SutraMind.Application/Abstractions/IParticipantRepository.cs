using SutraMind.Domain.Entities;

namespace SutraMind.Application.Abstractions;

public interface IParticipantRepository
{
    Task AddAsync(Participant participant, AyurvedaBaseline? baseline, CancellationToken cancellationToken = default);
    Task<Participant?> GetAsync(Guid id, CancellationToken cancellationToken = default);
}
