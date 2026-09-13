using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SutraMind.Application.Abstractions;
using SutraMind.Application.Contracts;
using SutraMind.Domain.Entities;
using SutraMind.Domain.Enums;
using SutraMind.Infrastructure.Persistence;

namespace SutraMind.Infrastructure.Services;

public sealed class CrfService(LocalDbContext context) : ICrfService
{
    public async Task SaveAsync(SaveCrfRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        var visit = await context.Visits.SingleAsync(v => v.Id == request.VisitId, cancellationToken);
        var existing = await context.Crfs.FirstOrDefaultAsync(c => c.VisitId == request.VisitId, cancellationToken);

        if (existing is not null)
        {
            existing.SystolicBp = request.SystolicBp;
            existing.DiastolicBp = request.DiastolicBp;
            existing.PulseBpm = request.PulseBpm;
            existing.WeightKg = request.WeightKg;
            existing.TemperatureC = request.TemperatureC;
            existing.AgniCode = request.AgniCode;
            existing.BalaCode = request.BalaCode;
            existing.Symptoms = request.Symptoms;
            existing.MedicineId = request.MedicineId;
            existing.Dose = request.Dose;
            existing.Frequency = request.Frequency;
            existing.Compliance = request.Compliance;
            existing.Remarks = request.Remarks;
            existing.CompletionStatus = request.CompletionStatus;
            existing.UpdatedAtUtc = DateTimeOffset.UtcNow;
            existing.LastModifiedBy = actorUserId;

            if (request.CompletionStatus == CrfStatus.Completed)
            {
                existing.CompletedBy = actorUserId;
                existing.CompletedAtUtc = DateTimeOffset.UtcNow;
            }
        }
        else
        {
            var crf = new Crf
            {
                VisitId = request.VisitId,
                SystolicBp = request.SystolicBp,
                DiastolicBp = request.DiastolicBp,
                PulseBpm = request.PulseBpm,
                WeightKg = request.WeightKg,
                TemperatureC = request.TemperatureC,
                AgniCode = request.AgniCode,
                BalaCode = request.BalaCode,
                Symptoms = request.Symptoms,
                MedicineId = request.MedicineId,
                Dose = request.Dose,
                Frequency = request.Frequency,
                Compliance = request.Compliance,
                Remarks = request.Remarks,
                CompletionStatus = request.CompletionStatus,
                LastModifiedBy = actorUserId
            };

            if (request.CompletionStatus == CrfStatus.Completed)
            {
                crf.CompletedBy = actorUserId;
                crf.CompletedAtUtc = DateTimeOffset.UtcNow;
            }

            context.Crfs.Add(crf);
        }

        if (request.CompletionStatus == CrfStatus.Completed && visit.Status == VisitStatus.Scheduled)
        {
            visit.Status = VisitStatus.Completed;
            visit.VisitDate = request.VisitDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        }

        context.OutboxOperations.Add(new OutboxOperation
        {
            EntityType = nameof(Crf),
            EntityId = existing?.Id ?? Guid.NewGuid(),
            Operation = existing is not null ? OperationType.Update : OperationType.Create,
            PayloadJson = JsonSerializer.Serialize(request),
            ActorUserId = actorUserId,
            DeviceId = Guid.Empty
        });

        await context.SaveChangesAsync(cancellationToken);
    }
}
