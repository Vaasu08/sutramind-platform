namespace SutraMind.Domain.Enums;

public enum StudyStatus { Draft, Active, Closed }
public enum ParticipantStatus { Draft, Submitted, Enrolled, Completed, Withdrawn }
public enum VisitStatus { Scheduled, Completed, Missed }
public enum CrfStatus { Draft, Completed }
public enum QueryStatus { Open, Answered, Closed }
public enum QueryTargetType { Participant, Baseline, Visit, Crf }
public enum EthicsStatus { Pending, Submitted, Approved, Rejected, Expired }
public enum OperationState { Pending, Sending, Conflict, Rejected, Applied }
