using SutraMind.Domain.Enums;

namespace SutraMind.Application.Contracts;

public sealed record RaiseQueryRequest(Guid StudyId, QueryTargetType TargetType, Guid TargetId, string? FieldName, string Message);
public sealed record AnswerQueryRequest(string Answer);
public sealed record CloseQueryRequest(string ResolutionReason);
