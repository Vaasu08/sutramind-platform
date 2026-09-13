using System.Text.Json;
using SutraMind.Application.Abstractions;
using SutraMind.Application.Contracts;
using SutraMind.Domain.Entities;
using SutraMind.Domain.Enums;
using SutraMind.Infrastructure.Persistence;

namespace SutraMind.Infrastructure.Services;

public sealed class StudyWriteService(LocalDbContext context) : IStudyWriteService
{
    public async Task<Study> CreateAsync(CreateStudyRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.StudyCode))
            throw new ArgumentException("Study code is required.");
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Study title is required.");

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var study = new Study
        {
            StudyCode = request.StudyCode.Trim(),
            Title = request.Title.Trim(),
            Institution = request.Institution.Trim(),
            PrincipalInvestigatorId = createdBy,
            SampleSize = request.SampleSize,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = StudyStatus.Draft
        };
        context.Studies.Add(study);

        var site = new Site
        {
            StudyId = study.Id,
            SiteCode = $"{request.StudyCode.Trim()}-SITE-01",
            Name = request.Institution.Trim(),
            Address = request.Institution.Trim(),
            IsActive = true
        };
        context.Sites.Add(site);

        var protocol = new Protocol
        {
            StudyId = study.Id,
            Version = request.Protocol.Version,
            IsActive = true,
            ModernDiagnosis = request.Protocol.ModernDiagnosis,
            VyadhiCode = request.Protocol.VyadhiCode,
            InterventionName = request.Protocol.InterventionName,
            MedicineId = request.Protocol.MedicineId,
            DosageFormCode = request.Protocol.DosageFormCode,
            AnupanaCode = request.Protocol.AnupanaCode,
            TreatmentDurationDays = request.Protocol.TreatmentDurationDays,
            CreatedBy = createdBy
        };
        context.Protocols.Add(protocol);

        var ethics = new EthicsReview
        {
            StudyId = study.Id,
            IecNumber = request.Ethics.IecNumber,
            Status = request.Ethics.Status,
            UpdatedBy = createdBy
        };
        context.EthicsReviews.Add(ethics);

        context.OutboxOperations.Add(new OutboxOperation
        {
            EntityType = nameof(Study),
            EntityId = study.Id,
            Operation = OperationType.Create,
            PayloadJson = JsonSerializer.Serialize(study),
            ActorUserId = createdBy,
            DeviceId = Guid.Empty
        });

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return study;
    }
}
