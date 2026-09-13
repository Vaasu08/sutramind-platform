using SutraMind.Application.Contracts;
using SutraMind.Domain.Entities;
using SutraMind.Domain.Enums;

namespace SutraMind.Application.Abstractions;

public interface ICrfService
{
    Task SaveAsync(SaveCrfRequest request, Guid actorUserId, CancellationToken cancellationToken = default);
}

public interface IQueryWriteService
{
    Task RaiseAsync(RaiseQueryRequest request, Guid raisedBy, CancellationToken cancellationToken = default);
    Task AnswerAsync(Guid queryId, AnswerQueryRequest request, Guid answeredBy, CancellationToken cancellationToken = default);
    Task CloseAsync(Guid queryId, CloseQueryRequest request, Guid closedBy, CancellationToken cancellationToken = default);
}

public interface IVisitWriteService
{
    Task CompleteAsync(Guid visitId, DateOnly visitDate, Guid actorUserId, CancellationToken cancellationToken = default);
    Task MarkMissedAsync(Guid visitId, Guid actorUserId, CancellationToken cancellationToken = default);
}

public interface IStudyWriteService
{
    Task<Study> CreateAsync(CreateStudyRequest request, Guid createdBy, CancellationToken cancellationToken = default);
}

public interface IParticipantWriteService
{
    Task WithdrawAsync(Guid participantId, Guid actorUserId, CancellationToken cancellationToken = default);
    Task CompleteAsync(Guid participantId, Guid actorUserId, CancellationToken cancellationToken = default);
}

public interface IEthicsWriteService
{
    Task UpdateStatusAsync(Guid ethicsId, EthicsStatus newStatus, DateOnly? approvalDate, DateOnly? expiryDate, string? remarks, Guid actorUserId, CancellationToken cancellationToken = default);
}

public interface IMasterDataWriteService
{
    Task AddAsync(string category, string code, string labelEn, string labelHi, string? modernMapping, int sortOrder, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid termId, string labelEn, string labelHi, string? modernMapping, int sortOrder, bool active, CancellationToken cancellationToken = default);
}
