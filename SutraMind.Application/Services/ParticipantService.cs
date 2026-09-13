using SutraMind.Application.Abstractions;
using SutraMind.Application.Contracts;
using SutraMind.Domain.Entities;
using SutraMind.Domain.Enums;

namespace SutraMind.Application.Services;

public sealed class ParticipantService(IParticipantRepository participants)
{
    public async Task<Participant> EnrollAsync(
        string participantCode,
        Guid studyId,
        Guid siteId,
        string modernDiagnosis,
        string vyadhiCode,
        Guid createdBy,
        CancellationToken cancellationToken = default)
    {
        return await EnrollAsync(new EnrollParticipantRequest(
            participantCode,
            studyId,
            siteId,
            "Demo Participant",
            0,
            "UNKNOWN",
            modernDiagnosis,
            vyadhiCode,
            null,
            null,
            DateOnly.FromDateTime(DateTime.UtcNow),
            new AyurvedaBaselineInput("P6", null, "A2", "B2", "S2"),
            createdBy), cancellationToken);
    }

    public async Task<Participant> EnrollAsync(
        EnrollParticipantRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ParticipantCode))
            throw new ArgumentException("Participant code is required.", nameof(request.ParticipantCode));

        if (string.IsNullOrWhiteSpace(request.VyadhiCode))
            throw new ArgumentException("Vyadhi code is required.", nameof(request.VyadhiCode));

        var participant = new Participant
        {
            ParticipantCode = request.ParticipantCode.Trim(),
            StudyId = request.StudyId,
            SiteId = request.SiteId,
            Name = request.Name.Trim(),
            Age = request.Age,
            Gender = request.Gender.Trim(),
            ModernDiagnosis = request.ModernDiagnosis.Trim(),
            VyadhiCode = request.VyadhiCode.Trim(),
            DiseaseDurationMonths = request.DiseaseDurationMonths,
            RandomizationId = request.RandomizationId,
            EnrollmentDate = request.EnrollmentDate,
            Status = ParticipantStatus.Enrolled,
            CreatedBy = request.CreatedBy
        };

        var baseline = new AyurvedaBaseline
        {
            ParticipantId = participant.Id,
            PrakritiCode = request.Baseline.PrakritiCode,
            VikritiNotes = request.Baseline.VikritiNotes,
            AgniCode = request.Baseline.AgniCode,
            BalaCode = request.Baseline.BalaCode,
            SatvaCode = request.Baseline.SatvaCode,
            RecordedBy = request.CreatedBy
        };

        await participants.AddAsync(participant, baseline, cancellationToken);
        return participant;
    }
}
