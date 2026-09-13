using SutraMind.Application.Authorization;
using SutraMind.Domain.Enums;
using SutraMind.Domain.Rules;

namespace SutraMind.Tests;

public sealed class DomainRulesTests
{
    [Fact]
    public void Coordinator_can_record_crf_but_cannot_close_query()
    {
        Assert.True(PermissionPolicy.Allows(UserRole.StudyCoordinator, Permission.RecordCrf));
        Assert.False(PermissionPolicy.Allows(UserRole.StudyCoordinator, Permission.CloseQuery));
    }

    [Fact]
    public void Monitor_can_raise_query_but_cannot_edit_crf()
    {
        Assert.True(PermissionPolicy.Allows(UserRole.Monitor, Permission.RaiseQuery));
        Assert.False(PermissionPolicy.Allows(UserRole.Monitor, Permission.RecordCrf));
    }

    [Fact]
    public void Query_must_be_answered_before_it_can_be_closed()
    {
        Assert.False(LifecycleRules.CanTransition(QueryStatus.Open, QueryStatus.Closed));
        Assert.True(LifecycleRules.CanTransition(QueryStatus.Open, QueryStatus.Answered));
        Assert.True(LifecycleRules.CanTransition(QueryStatus.Answered, QueryStatus.Closed));
    }
}
