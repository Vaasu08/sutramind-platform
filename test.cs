using System;
using SutraMind.Domain.Enums;
using SutraMind.Application.Authorization;
using SutraMind.Desktop.Services;

class Program {
    static void Main() {
        var role = ClinicalRoleMapper.FromEmail(""pi@sutramind.local"");
        var hasPerm = PermissionPolicy.Allows(role, Permission.CreateStudy);
        Console.WriteLine($""Role: {role}, HasPerm: {hasPerm}"");
    }
}
