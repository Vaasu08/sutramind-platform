namespace SutraMind.Application.Contracts;

public sealed record PageMeta(int Page, int PageSize, int TotalCount);
public sealed record ApiError(string Code, string Message, IReadOnlyDictionary<string, string[]> Fields);
public sealed record ApiEnvelope<T>(T Data, PageMeta? Meta = null);
public sealed record UserSummary(Guid Id, string Name, string Email, string Role);
public sealed record StudyAssignment(Guid StudyId, Guid? SiteId, string RoleInStudy);
public sealed record CurrentUserResponse(UserSummary User, IReadOnlyList<StudyAssignment> Assignments);
