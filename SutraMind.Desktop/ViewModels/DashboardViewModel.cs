using System.Collections.ObjectModel;
using System.Windows.Input;
using SutraMind.Application.Abstractions;
using SutraMind.Application.ReadModels;
using SutraMind.Desktop.Navigation;
using SutraMind.Desktop.Services;
using SutraMind.Application.Authorization;
using SutraMind.Domain.Enums;

namespace SutraMind.Desktop.ViewModels;

public sealed class DashboardViewModel : SectionViewModelBase
{
    private readonly ClinicalSession _session;
    private readonly IDashboardService _dashboardService;
    private readonly IShellNavigation _navigation;
    private string participantSummary = "Loading…";

    public DashboardViewModel(ClinicalSession session, IDashboardService dashboardService, IShellNavigation navigation)
    {
        _session = session;
        _dashboardService = dashboardService;
        _navigation = navigation;
        StartEnrollmentCommand = new PermissionCommand(() => session.Role, Permission.CreateParticipant, navigation.StartEnrollment);
        ScheduleVisitCommand = new PermissionCommand(() => session.Role, Permission.ScheduleVisit, navigation.StartScheduleVisit);
        OpenQueriesCommand = new RelayCommand(() => navigation.OpenQueries(openOnly: true));
    }

    public override WorkspaceSection Section => WorkspaceSection.Dashboard;

    public ObservableCollection<MetricCardData> Metrics { get; } = [];
    public ObservableCollection<VisitStageData> VisitStages { get; } = [];
    public ObservableCollection<ParticipantListItem> Participants { get; } = [];

    public string ParticipantSummary
    {
        get => participantSummary;
        private set => SetProperty(ref participantSummary, value);
    }

    public ICommand StartEnrollmentCommand { get; }
    public ICommand ScheduleVisitCommand { get; }
    public ICommand OpenQueriesCommand { get; }

    public string EnrollTooltip => PermissionPolicy.Allows(_session.Role, Permission.CreateParticipant)
        ? "Open the enrollment form for a new participant."
        : $"Your role ({ClinicalRoleMapper.ToDisplayName(_session.Role)}) cannot enroll participants.";

    public string ScheduleTooltip => PermissionPolicy.Allows(_session.Role, Permission.ScheduleVisit)
        ? "Schedule a visit for an enrolled participant."
        : $"Your role ({ClinicalRoleMapper.ToDisplayName(_session.Role)}) cannot schedule visits.";

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var data = await _dashboardService.GetDashboardAsync(_session.ActiveStudyId, cancellationToken);
        Replace(Metrics, data.Metrics);
        Replace(VisitStages, data.VisitStages);
        Replace(Participants, data.Participants);
        ParticipantSummary = data.ParticipantSummary;
    }

    private static void Replace<T>(ObservableCollection<T> target, IReadOnlyList<T> source)
    {
        target.Clear();
        foreach (var item in source)
            target.Add(item);
    }
}
