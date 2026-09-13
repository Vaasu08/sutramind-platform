using SutraMind.Domain.Enums;
using SutraMind.Infrastructure.Seed;

namespace SutraMind.Desktop.Services;

public static class ClinicalRoleMapper
{
    public static UserRole FromEmail(string email)
    {
        if (email.StartsWith("admin", StringComparison.OrdinalIgnoreCase)) return UserRole.Administrator;
        if (email.StartsWith("pi@", StringComparison.OrdinalIgnoreCase)) return UserRole.PrincipalInvestigator;
        if (email.StartsWith("monitor", StringComparison.OrdinalIgnoreCase)) return UserRole.Monitor;
        if (email.StartsWith("ethics", StringComparison.OrdinalIgnoreCase)) return UserRole.EthicsCommittee;
        if (email.StartsWith("pv@", StringComparison.OrdinalIgnoreCase)) return UserRole.Pharmacovigilance;
        return UserRole.StudyCoordinator;
    }

    public static Guid ActorUserIdFromEmail(string email) =>
        email.StartsWith("pi@", StringComparison.OrdinalIgnoreCase) ? DemoIds.PiUserId : DemoIds.CoordinatorUserId;

    public static string ToDisplayName(UserRole role) => role switch
    {
        UserRole.Administrator => "Administrator",
        UserRole.PrincipalInvestigator => "Principal Investigator",
        UserRole.Monitor => "Monitor",
        UserRole.EthicsCommittee => "Ethics Committee",
        UserRole.Pharmacovigilance => "Pharmacovigilance",
        _ => "Study Coordinator"
    };
}
