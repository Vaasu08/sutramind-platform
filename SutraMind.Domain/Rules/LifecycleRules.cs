using SutraMind.Domain.Enums;

namespace SutraMind.Domain.Rules;

public static class LifecycleRules
{
    public static bool CanTransition(ParticipantStatus from, ParticipantStatus to) => (from, to) is (ParticipantStatus.Draft, ParticipantStatus.Submitted) or (ParticipantStatus.Submitted, ParticipantStatus.Enrolled) or (ParticipantStatus.Enrolled, ParticipantStatus.Completed) or (ParticipantStatus.Enrolled, ParticipantStatus.Withdrawn);
    public static bool CanTransition(QueryStatus from, QueryStatus to) => (from, to) is (QueryStatus.Open, QueryStatus.Answered) or (QueryStatus.Answered, QueryStatus.Closed);
    public static bool CanTransition(StudyStatus from, StudyStatus to) => (from, to) is (StudyStatus.Draft, StudyStatus.Active) or (StudyStatus.Active, StudyStatus.Closed);
}
