using SutraMind.Application.Abstractions;
using SutraMind.Domain.Enums;

namespace SutraMind.Desktop.Services;

public sealed class ClinicalSession
{
    public required ClinicalContextSnapshot Context { get; set; }
    public required string Email { get; set; }
    public required UserRole Role { get; set; }
    public Guid ActorUserId => Context.ActorUserId;
    public Guid DeviceId => Context.DeviceId;
    public Guid ActiveStudyId => Context.StudyId;
    public Guid ActiveSiteId => Context.SiteId;
}
