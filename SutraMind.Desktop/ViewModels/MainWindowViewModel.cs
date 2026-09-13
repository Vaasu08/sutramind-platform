using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SutraMind.Desktop.ViewModels;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    public string ProductName => "SutraMind";
    public string EnvironmentName => "Clinical research workspace";
    public string SyncStatus => "Offline - local data available";
    public string LastSync => "Last sync: Today, 09:42";
    public string PendingOperations => "3 pending operations";
    public string AuthenticatedUser { get; private set; } = "coordinator@sutramind.local";
    public string UserRole { get; private set; } = "Study Coordinator";
    public string CurrentSection { get; private set; } = "Dashboard";
    public string SectionDescription { get; private set; } = "Your research operations at a glance";
    private string activeWorkflow = "Participant enrollment and Ayurveda baseline";
    public string ActiveWorkflow
    {
        get => activeWorkflow;
        private set
        {
            if (activeWorkflow == value) return;
            activeWorkflow = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<string> StudyOptions { get; } = [
        "AMAVATA-001  |  Yogaraja Guggulu",
        "PRATISHYAYA-001  |  Haridra Khanda"
    ];
    public string SelectedStudy { get; set; } = "AMAVATA-001  |  Yogaraja Guggulu";
    public ObservableCollection<MetricCard> Metrics { get; } = [
        new("68 / 120", "Recruitment progress", "56.7% of target", "#174A45"),
        new("92.2%", "Visit completion", "142 of 154 visits", "#276C65"),
        new("98.6%", "Medicine adherence", "Across active cohort", "#9A7615"),
        new("APPROVED", "Ethics status", "AIIA-IEC-2026-001", "#28704B"),
        new("1", "Open query", "14 closed this cycle", "#A45D2B")
    ];
    public ObservableCollection<VisitStage> VisitStages { get; } = [
        new("Screening", "80 / 120", 0.67),
        new("Baseline (V0)", "68 / 68", 1.0),
        new("Visit 1 (D30)", "63 / 68", 0.93),
        new("Visit 2 (D60)", "50 / 68", 0.74),
        new("Visit 3 (D90)", "33 / 68", 0.49)
    ];
    public ObservableCollection<ParticipantRow> Participants { get; } = [
        new("AMV-001", "42 / F", "R-001", "Vata-Kapha", "Tikshnagni", "Madhyama", "Visit 2 complete", "#28704B"),
        new("AMV-002", "51 / M", "R-002", "Vata-Pitta", "Vishamagni", "Madhyama", "Visit 1 complete", "#28704B"),
        new("AMV-003", "38 / F", "R-003", "Pitta-Kapha", "Samagni", "Pravara", "Visit 1 complete", "#28704B"),
        new("AMV-004", "62 / M", "R-004", "Vata-Kapha", "Mandagni", "Avara", "Query pending", "#A45D2B"),
        new("AMV-005", "47 / F", "R-005", "Tridoshaja", "Vishamagni", "Madhyama", "Baseline done", "#2B6F8A")
    ];
    public ICommand StartEnrollmentCommand { get; }
    public ICommand OpenQueriesCommand { get; }
    public ICommand ScheduleVisitCommand { get; }
    public ICommand SyncNowCommand { get; }
    public ICommand DashboardCommand { get; }
    public ICommand StudiesCommand { get; }
    public ICommand ParticipantsCommand { get; }
    public ICommand VisitsCommand { get; }
    public ICommand EthicsCommand { get; }
    public ICommand MasterDataCommand { get; }
    public ICommand LogoutCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? LogoutRequested;

    public MainWindowViewModel()
    {
        StartEnrollmentCommand = new DelegateCommand(() => ActiveWorkflow = "New participant enrollment ready");
        OpenQueriesCommand = new DelegateCommand(() => ActiveWorkflow = "Query review queue selected");
        ScheduleVisitCommand = new DelegateCommand(() => ActiveWorkflow = "Visit scheduling selected");
        SyncNowCommand = new DelegateCommand(() => ActiveWorkflow = "Sync queued - waiting for network");
        DashboardCommand = new DelegateCommand(() => SelectSection("Dashboard", "Your research operations at a glance"));
        StudiesCommand = new DelegateCommand(() => SelectSection("Studies", "Review protocols, sites, ethics, and recruitment targets"));
        ParticipantsCommand = new DelegateCommand(() => SelectSection("Participants", "Search and review the assigned participant registry"));
        VisitsCommand = new DelegateCommand(() => SelectSection("Visits and CRF", "Track scheduled visits and complete structured case report forms"));
        EthicsCommand = new DelegateCommand(() => SelectSection("Ethics Review", "Review IEC status, approval dates, and study remarks"));
        MasterDataCommand = new DelegateCommand(() => SelectSection("Master Data", "Manage the controlled Ayurveda terminology used by forms"));
        LogoutCommand = new DelegateCommand(() => LogoutRequested?.Invoke(this, EventArgs.Empty));
    }

    public void SetAuthenticatedUser(string email)
    {
        AuthenticatedUser = email;
        UserRole = email.StartsWith("admin", StringComparison.OrdinalIgnoreCase) ? "Administrator" :
            email.StartsWith("pi@", StringComparison.OrdinalIgnoreCase) ? "Principal Investigator" :
            email.StartsWith("monitor", StringComparison.OrdinalIgnoreCase) ? "Monitor" :
            email.StartsWith("ethics", StringComparison.OrdinalIgnoreCase) ? "Ethics Committee" :
            email.StartsWith("pv@", StringComparison.OrdinalIgnoreCase) ? "Pharmacovigilance" : "Study Coordinator";
        OnPropertyChanged(nameof(AuthenticatedUser));
        OnPropertyChanged(nameof(UserRole));
    }

    private void SelectSection(string section, string description)
    {
        CurrentSection = section;
        SectionDescription = description;
        ActiveWorkflow = $"{section} workspace selected";
        OnPropertyChanged(nameof(CurrentSection));
        OnPropertyChanged(nameof(SectionDescription));
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public sealed record MetricCard(string Value, string Label, string Detail, string Accent);
    public sealed record VisitStage(string Stage, string Progress, double Completion);
    public sealed record ParticipantRow(string Code, string Demographic, string Randomization, string Prakriti, string Agni, string Bala, string Status, string StatusColor);

    private sealed class DelegateCommand(Action execute) : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => execute();
    }
}
