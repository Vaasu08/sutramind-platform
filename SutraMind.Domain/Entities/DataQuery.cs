using SutraMind.Domain.Enums;

namespace SutraMind.Domain.Entities;

public sealed class DataQuery : SyncEntity
{
    public required Guid StudyId { get; set; }
    public QueryTargetType TargetType { get; set; }
    public required Guid TargetId { get; set; }
    public string? FieldName { get; set; }
    public required string Message { get; set; }
    public QueryStatus Status { get; set; } = QueryStatus.Open;
    public Guid RaisedBy { get; set; }
    public DateTimeOffset RaisedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public string? Answer { get; set; }
    public Guid? AnsweredBy { get; set; }
    public DateTimeOffset? AnsweredAtUtc { get; set; }
    public Guid? ClosedBy { get; set; }
    public DateTimeOffset? ClosedAtUtc { get; set; }
}
