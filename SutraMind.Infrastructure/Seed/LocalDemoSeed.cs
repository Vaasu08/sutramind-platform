using Microsoft.EntityFrameworkCore;
using SutraMind.Domain.Entities;
using SutraMind.Domain.Enums;
using SutraMind.Infrastructure.Persistence;

namespace SutraMind.Infrastructure.Seed;

public static class LocalDemoSeed
{
    public static async Task EnsureSeededAsync(LocalDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Studies.AnyAsync(cancellationToken))
            return;

        var medicine = new Medicine
        {
            Id = DemoIds.MedicineId,
            MedicineCode = "ASU-042",
            AyurvedaName = "Yogaraja Guggulu",
            DosageFormCode = "DF01",
            WhoDrugCode = null,
            IsActive = true
        };

        var study = new Study
        {
            Id = DemoIds.StudyId,
            StudyCode = "AMAVATA-001",
            Title = "Yogaraja Guggulu in Amavata (Phase II)",
            Institution = "AIIA New Delhi",
            PrincipalInvestigatorId = DemoIds.PiUserId,
            SampleSize = 120,
            StartDate = new DateOnly(2026, 1, 15),
            EndDate = new DateOnly(2027, 1, 15),
            Status = StudyStatus.Active,
            CtriNumber = "CTRI/2026/08/045210"
        };

        var site = new Site
        {
            Id = DemoIds.SiteId,
            StudyId = DemoIds.StudyId,
            SiteCode = "AIIA-ND",
            Name = "AIIA New Delhi",
            Address = "New Delhi",
            IsActive = true
        };

        var protocol = new Protocol
        {
            Id = DemoIds.ProtocolId,
            StudyId = DemoIds.StudyId,
            Version = "1.0",
            IsActive = true,
            ModernDiagnosis = "Amavata (Rheumatoid Arthritis)",
            VyadhiCode = "V001",
            InterventionName = "Yogaraja Guggulu",
            MedicineId = DemoIds.MedicineId,
            DosageFormCode = "DF01",
            AnupanaCode = "AN01",
            TreatmentDurationDays = 90,
            Summary = "MedDRA Mapping: 10039073 • Schedule Y Phase II",
            CreatedBy = DemoIds.PiUserId
        };

        var ethics = new EthicsReview
        {
            Id = DemoIds.EthicsId,
            StudyId = DemoIds.StudyId,
            IecNumber = "AIIA-IEC-2026-001",
            Status = EthicsStatus.Approved,
            ApprovalDate = new DateOnly(2026, 1, 10),
            ExpiryDate = new DateOnly(2027, 1, 10),
            Remarks = "Blinded to identifiers"
        };

        context.Medicines.Add(medicine);
        context.Studies.Add(study);
        context.Sites.Add(site);
        context.Protocols.Add(protocol);
        context.EthicsReviews.Add(ethics);

        SeedMasterTerms(context);
        var participants = SeedParticipants(context);
        SeedVisitsAndQueries(context, participants);

        await context.SaveChangesAsync(cancellationToken);
    }

    private static void SeedMasterTerms(LocalDbContext context)
    {
        var terms = new (string Category, string Code, string En, string Hi, int Order)[]
        {
            ("Prakriti", "P6", "Vata-Kapha", "वात-कफ", 6),
            ("Agni", "A2", "Tikshnagni", "तीक्ष्णाग्नि", 2),
            ("Agni", "A3", "Vishamagni", "विषमाग्नि", 3),
            ("Agni", "A1", "Samagni", "समाग्नि", 1),
            ("Agni", "A4", "Mandagni", "मन्दाग्नि", 4),
            ("Bala", "B2", "Madhyama", "मध्यम", 2),
            ("Bala", "B3", "Pravara", "प्रवर", 3),
            ("Bala", "B1", "Avara", "अवर", 1),
            ("Anupana", "AN01", "Koshna Jala (Warm Water)", "कोष्ण जल", 1),
            ("DosageForm", "DF01", "Vati", "वटी", 1)
        };

        foreach (var term in terms)
        {
            context.MasterTerms.Add(new MasterTerm
            {
                Category = term.Category,
                Code = term.Code,
                LabelEn = term.En,
                LabelHi = term.Hi,
                SortOrder = term.Order,
                Active = true
            });
        }
    }

    private static List<(Participant Participant, AyurvedaBaseline Baseline)> SeedParticipants(LocalDbContext context)
    {
        var rows = new (string Code, int Age, string Gender, string Rand, string Prakriti, string Agni, string Bala, string Status)[]
        {
            ("AMV-001", 42, "F", "R-001", "P6", "A2", "B2", "Visit 2 complete"),
            ("AMV-002", 51, "M", "R-002", "P6", "A3", "B2", "Visit 1 complete"),
            ("AMV-003", 38, "F", "R-003", "P6", "A1", "B3", "Visit 1 complete"),
            ("AMV-004", 62, "M", "R-004", "P6", "A4", "B1", "Query pending"),
            ("AMV-005", 47, "F", "R-005", "P6", "A3", "B2", "Baseline done")
        };

        var result = new List<(Participant, AyurvedaBaseline)>();
        foreach (var row in rows)
        {
            var participant = new Participant
            {
                StudyId = DemoIds.StudyId,
                SiteId = DemoIds.SiteId,
                ParticipantCode = row.Code,
                Name = $"Demo {row.Code}",
                Age = row.Age,
                Gender = row.Gender,
                ModernDiagnosis = "Rheumatoid Arthritis",
                VyadhiCode = "V001",
                RandomizationId = row.Rand,
                EnrollmentDate = new DateOnly(2026, 2, 1),
                Status = ParticipantStatus.Enrolled,
                CreatedBy = DemoIds.CoordinatorUserId
            };

            var baseline = new AyurvedaBaseline
            {
                ParticipantId = participant.Id,
                PrakritiCode = row.Prakriti,
                AgniCode = row.Agni,
                BalaCode = row.Bala,
                SatvaCode = "S2",
                RecordedBy = DemoIds.CoordinatorUserId
            };

            context.Participants.Add(participant);
            context.AyurvedaBaselines.Add(baseline);
            result.Add((participant, baseline));
        }

        return result;
    }

    private static void SeedVisitsAndQueries(LocalDbContext context, List<(Participant Participant, AyurvedaBaseline Baseline)> participants)
    {
        foreach (var (participant, _) in participants)
        {
            for (var visitNumber = 0; visitNumber <= 2; visitNumber++)
            {
                var visit = new Visit
                {
                    ParticipantId = participant.Id,
                    VisitNumber = visitNumber,
                    ScheduledDate = new DateOnly(2026, 2, 10).AddDays(visitNumber * 30),
                    VisitDate = visitNumber < 2 ? new DateOnly(2026, 2, 10).AddDays(visitNumber * 30) : null,
                    Status = visitNumber < 2 ? VisitStatus.Completed : VisitStatus.Scheduled,
                    CreatedBy = DemoIds.CoordinatorUserId
                };
                context.Visits.Add(visit);

                if (visitNumber < 2)
                {
                    context.Crfs.Add(new Crf
                    {
                        VisitId = visit.Id,
                        CompletionStatus = CrfStatus.Completed,
                        CompletedBy = DemoIds.CoordinatorUserId,
                        CompletedAtUtc = DateTimeOffset.UtcNow
                    });
                }
            }
        }

        var queryParticipant = participants[3].Participant;
        context.Queries.Add(new DataQuery
        {
            StudyId = DemoIds.StudyId,
            TargetType = QueryTargetType.Participant,
            TargetId = queryParticipant.Id,
            FieldName = "Bala",
            Message = "Confirm baseline Bala assessment against source notes.",
            Status = QueryStatus.Open,
            RaisedBy = DemoIds.PiUserId
        });
    }
}
