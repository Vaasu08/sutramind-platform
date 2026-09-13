using SutraMind.Domain.Enums;

namespace SutraMind.Application.Authorization;

public static class PermissionPolicy
{
    private static readonly IReadOnlyDictionary<UserRole, IReadOnlySet<Permission>> Permissions = new Dictionary<UserRole, IReadOnlySet<Permission>>
    {
        [UserRole.Administrator] = Enum.GetValues<Permission>().ToHashSet(),
        [UserRole.PrincipalInvestigator] = new HashSet<Permission> { Permission.CreateStudy, Permission.EditStudy, Permission.ViewStudy, Permission.ViewParticipants, Permission.EditParticipant, Permission.ViewCrf, Permission.AnswerQuery, Permission.CloseQuery, Permission.ViewDashboard },
        [UserRole.StudyCoordinator] = new HashSet<Permission> { Permission.ViewStudy, Permission.ViewParticipants, Permission.CreateParticipant, Permission.EditParticipant, Permission.RecordBaseline, Permission.ScheduleVisit, Permission.RecordCrf, Permission.ViewCrf, Permission.AnswerQuery, Permission.ViewDashboard },
        [UserRole.Monitor] = new HashSet<Permission> { Permission.ViewStudy, Permission.ViewParticipants, Permission.ViewCrf, Permission.RaiseQuery, Permission.ViewDashboard },
        [UserRole.EthicsCommittee] = new HashSet<Permission> { Permission.ViewStudy, Permission.UpdateEthics },
        [UserRole.Pharmacovigilance] = new HashSet<Permission> { Permission.ViewStudy, Permission.ViewDashboard }
    };

    public static bool Allows(UserRole role, Permission permission) => Permissions.TryGetValue(role, out var allowed) && allowed.Contains(permission);
}
