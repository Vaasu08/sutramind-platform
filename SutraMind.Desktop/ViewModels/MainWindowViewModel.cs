using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using SutraMind.Application.Abstractions;
using SutraMind.Application.ReadModels;
using SutraMind.Desktop.Navigation;
using SutraMind.Desktop.Services;
using SutraMind.Desktop.Windows;
using SutraMind.Domain.Enums;
using SutraMind.Infrastructure.Persistence;

namespace SutraMind.Desktop.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase, IShellNavigation
{
    private readonly IServiceProvider _services;
    private readonly ClinicalSession _session;
    private readonly IOutboxService _outboxService;
    private readonly Dictionary<WorkspaceSection, SectionViewModelBase> _sections;
    private SectionViewModelBase? currentSectionViewModel;
    private WorkspaceSection selectedSection = WorkspaceSection.Dashboard;
    private string currentSection = "Dashboard";
    private string sectionDescription = "Your research operations at a glance";
    private string activeWorkflow = "Participant enrollment and Ayurveda baseline";
    private string pendingOperations = "Loading…";
    private string lastSync = "Last sync: Not yet synced";
    private string authenticatedUser = "coordinator@sutramind.local";
    private string userRole = "Study Coordinator";
    private string selectedStudy = string.Empty;

    public MainWindowViewModel(
        IServiceProvider services,
        ClinicalSession session,
        IOutboxService outboxService,
        IDashboardService dashboardService,
        IStudyWorkspaceService studyWorkspaceService,
        IParticipantReadService participantReadService,
        IVisitCrfReadService visitCrfReadService,
        IQueryReadService queryReadService,
        IEthicsReadService ethicsReadService,
        IMasterDataReadService masterDataReadService,
        LocalDbContext context)
    {
        _services = services;
        _session = session;
        _outboxService = outboxService;

        _sections = new Dictionary<WorkspaceSection, SectionViewModelBase>
        {
            [WorkspaceSection.Dashboard] = new DashboardViewModel(session, dashboardService, this),
            [WorkspaceSection.Studies] = new StudiesViewModel(session, studyWorkspaceService, this),
            [WorkspaceSection.Participants] = new ParticipantsViewModel(session, participantReadService, this),
            [WorkspaceSection.VisitsAndCrf] = new VisitsCrfViewModel(session, visitCrfReadService, this, context),
            [WorkspaceSection.DataQueries] = new DataQueriesViewModel(session, queryReadService, this),
            [WorkspaceSection.EthicsReview] = new EthicsReviewViewModel(session, ethicsReadService, this, context),
            [WorkspaceSection.MasterData] = new MasterDataViewModel(masterDataReadService, this, session, context)
        };

        SyncNowCommand = new RelayCommand(async () => await SyncNowAsync());
        LogoutCommand = new RelayCommand(() => LogoutRequested?.Invoke(this, EventArgs.Empty));

        NavItems =
        [
            CreateNav(WorkspaceSection.Dashboard, "  Dashboard"),
            CreateNav(WorkspaceSection.Studies, "  Studies"),
            CreateNav(WorkspaceSection.Participants, "  Participants"),
            CreateNav(WorkspaceSection.VisitsAndCrf, "  Visits and CRF"),
            CreateNav(WorkspaceSection.DataQueries, "  Data Queries"),
            CreateNav(WorkspaceSection.EthicsReview, "  Ethics Review"),
            CreateNav(WorkspaceSection.MasterData, "  Master Data")
        ];

        StudyOptions = new ObservableCollection<string>();
        _ = InitializeAsync();
    }

    public ObservableCollection<NavItemViewModel> NavItems { get; }
    public ObservableCollection<string> StudyOptions { get; }

    public SectionViewModelBase? CurrentSectionViewModel
    {
        get => currentSectionViewModel;
        private set => SetProperty(ref currentSectionViewModel, value);
    }

    public WorkspaceSection SelectedSection
    {
        get => selectedSection;
        private set => SetProperty(ref selectedSection, value);
    }

    public string SyncStatus => "Offline - local data available";
    public string CurrentSection
    {
        get => currentSection;
        private set => SetProperty(ref currentSection, value);
    }

    public string SectionDescription
    {
        get => sectionDescription;
        private set => SetProperty(ref sectionDescription, value);
    }

    public string ActiveWorkflow
    {
        get => activeWorkflow;
        private set => SetProperty(ref activeWorkflow, value);
    }

    public string PendingOperations
    {
        get => pendingOperations;
        private set => SetProperty(ref pendingOperations, value);
    }

    public string LastSync
    {
        get => lastSync;
        private set => SetProperty(ref lastSync, value);
    }

    public string AuthenticatedUser
    {
        get => authenticatedUser;
        private set => SetProperty(ref authenticatedUser, value);
    }

    public string UserRole
    {
        get => userRole;
        private set => SetProperty(ref userRole, value);
    }

    public string SelectedStudy
    {
        get => selectedStudy;
        set => SetProperty(ref selectedStudy, value);
    }

    public ICommand SyncNowCommand { get; }
    public ICommand LogoutCommand { get; }

    public event EventHandler? LogoutRequested;

    public void SetAuthenticatedUser(string email)
    {
        AuthenticatedUser = email;
        UserRole = ClinicalRoleMapper.ToDisplayName(ClinicalRoleMapper.FromEmail(email));
        _ = RefreshShellAsync();
    }

    public void NavigateTo(WorkspaceSection section) => _ = NavigateAsync(section);

    public void StartEnrollment()
    {
        var window = _services.GetRequiredService<EnrollParticipantWindowFactory>().Create();
        window.Owner = System.Windows.Application.Current.MainWindow;
        if (window.ShowDialog() == true)
            _ = RefreshShellAsync();
    }

    public void StartScheduleVisit()
    {
        NavigateTo(WorkspaceSection.VisitsAndCrf);
        var window = _services.GetRequiredService<ScheduleVisitWindowFactory>().Create();
        window.Owner = System.Windows.Application.Current.MainWindow;
        if (window.ShowDialog() == true)
            _ = RefreshShellAsync();
    }

    public void OpenQueries(bool openOnly)
    {
        if (_sections[WorkspaceSection.DataQueries] is DataQueriesViewModel queries)
            queries.OpenOnly = openOnly;
        NavigateTo(WorkspaceSection.DataQueries);
    }

    public void StartRecordCrf(Guid visitId, string visitLabel)
    {
        var window = _services.GetRequiredService<RecordCrfWindowFactory>().Create(visitId, visitLabel);
        window.Owner = System.Windows.Application.Current.MainWindow;
        if (window.ShowDialog() == true)
            _ = RefreshShellAsync();
    }

    public void StartCompleteVisit(Guid visitId, string visitLabel)
    {
        var window = _services.GetRequiredService<CompleteVisitWindowFactory>().Create(visitId, visitLabel);
        window.Owner = System.Windows.Application.Current.MainWindow;
        if (window.ShowDialog() == true)
            _ = RefreshShellAsync();
    }

    public void StartQueryAction(QueryActionMode mode, QueryListItem? item = null, Guid? entityId = null)
    {
        var window = _services.GetRequiredService<QueryActionWindowFactory>().Create(mode, item, entityId);
        window.Owner = System.Windows.Application.Current.MainWindow;
        if (window.ShowDialog() == true)
            _ = RefreshShellAsync();
    }

    public void StartCreateStudy()
    {
        var window = _services.GetRequiredService<CreateStudyWindowFactory>().Create();
        window.Owner = System.Windows.Application.Current.MainWindow;
        if (window.ShowDialog() == true)
            _ = RefreshShellAsync();
    }

    public void StartParticipantDetail(string participantCode)
    {
        var window = _services.GetRequiredService<ParticipantDetailWindowFactory>().Create(participantCode);
        window.Owner = System.Windows.Application.Current.MainWindow;
        if (window.ShowDialog() == true)
            _ = RefreshShellAsync();
    }

    public void StartUpdateEthics(Guid ethicsId, string iecNumber, string currentStatus)
    {
        var window = _services.GetRequiredService<UpdateEthicsWindowFactory>().Create(ethicsId, iecNumber, currentStatus);
        window.Owner = System.Windows.Application.Current.MainWindow;
        if (window.ShowDialog() == true)
            _ = RefreshShellAsync();
    }

    public void StartAddMasterTerm()
    {
        var window = _services.GetRequiredService<EditMasterTermWindowFactory>().CreateAdd();
        window.Owner = System.Windows.Application.Current.MainWindow;
        if (window.ShowDialog() == true)
            _ = RefreshShellAsync();
    }

    public void StartEditMasterTerm(MasterTermListItem existing, Guid termId)
    {
        var window = _services.GetRequiredService<EditMasterTermWindowFactory>().CreateEdit(existing, termId);
        window.Owner = System.Windows.Application.Current.MainWindow;
        if (window.ShowDialog() == true)
            _ = RefreshShellAsync();
    }

    public async Task SyncNowAsync()
    {
        var pending = await _outboxService.CountPendingAsync();
        PendingOperations = FormatPending(pending);
        LastSync = $"Last sync preview: {DateTime.Now:HH:mm}";
        ActiveWorkflow = pending == 0
            ? "No pending operations — ready when online"
            : $"Sync queued — {pending} operation(s) waiting for network";
    }

    public async Task RefreshShellAsync()
    {
        var studies = await _services.GetRequiredService<IStudyWorkspaceService>().ListStudiesAsync();
        StudyOptions.Clear();
        foreach (var study in studies)
            StudyOptions.Add(study.Label);

        if (StudyOptions.Count > 0 && string.IsNullOrWhiteSpace(SelectedStudy))
            SelectedStudy = StudyOptions[0];

        var pending = await _outboxService.CountPendingAsync();
        PendingOperations = FormatPending(pending);
        var lastAttempt = await _outboxService.GetLastSyncAttemptUtcAsync();
        LastSync = lastAttempt is null
            ? "Last sync: Not yet synced"
            : $"Last sync: {lastAttempt.Value.LocalDateTime:g}";

        await NavigateAsync(SelectedSection);
    }

    private NavItemViewModel CreateNav(WorkspaceSection section, string label) =>
        new(section, label, new RelayCommand(() => NavigateTo(section)));

    private async Task InitializeAsync() => await RefreshShellAsync();

    private async Task NavigateAsync(WorkspaceSection section)
    {
        SelectedSection = section;
        ApplySectionHeader(section);
        foreach (var item in NavItems)
            item.IsSelected = item.Section == section;

        if (!_sections.TryGetValue(section, out var viewModel))
            return;

        CurrentSectionViewModel = viewModel;
        await viewModel.LoadAsync();
    }

    private void ApplySectionHeader(WorkspaceSection section)
    {
        (CurrentSection, SectionDescription) = section switch
        {
            WorkspaceSection.Dashboard => ("Dashboard", "Your research operations at a glance"),
            WorkspaceSection.Studies => ("Studies", "Review protocols, sites, ethics, and recruitment targets"),
            WorkspaceSection.Participants => ("Participants", "Search and review the assigned participant registry"),
            WorkspaceSection.VisitsAndCrf => ("Visits and CRF", "Track scheduled visits and complete structured case report forms"),
            WorkspaceSection.DataQueries => ("Data Queries", "Review field-level discrepancies and resolutions"),
            WorkspaceSection.EthicsReview => ("Ethics Review", "Review IEC status, approval dates, and study remarks"),
            WorkspaceSection.MasterData => ("Master Data", "Manage controlled Ayurveda terminology used by forms"),
            _ => ("Dashboard", "Your research operations at a glance")
        };
        ActiveWorkflow = $"{CurrentSection} workspace selected";
    }

    private static string FormatPending(int count) =>
        count == 1 ? "1 pending operation" : $"{count} pending operations";
}
